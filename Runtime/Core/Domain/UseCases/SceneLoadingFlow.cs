using System;
using System.Collections.Generic;
using CommandQueues.Core;
using ScenesLoaderSystem.Delays.Domain;

namespace ScenesLoaderSystem.Core.Domain
{
    public class SceneLoadingFlow
    {
        private readonly ISceneOperations _sceneOperations;
        private readonly IDelayProvider _delayProvider;
        private readonly SceneLoadingSettings _settings;

        private readonly List<SceneData> _openScenes = new List<SceneData>();
        private readonly List<INodeCommand> _nodeCommands = new List<INodeCommand>();
        private readonly Queue<SceneData> _scenesToOpenQueue = new Queue<SceneData>();
        private readonly Queue<SceneData> _scenesToRemoveQueue = new Queue<SceneData>();

        private SceneData _currentSceneData;
        private SceneData _loadingSceneData;
        private SceneData _nextCurrentSceneData;
        private CommandQueue _commandQueue;
        private Action _onTransitionSceneShown;
        private string _openTransitionSceneName;
        private string _loadingTransitionSceneName;
        private int _loadingIsFinishingEventIndex;
        private bool _isLoading;
        private bool _hasToKeepOpenScenes;
        private bool _hasToForceRemoveScenes;

        public Action OnTransitionSceneStartUnloaded { get; set; }
        public Action OnAllScenesLoaded { get; set; }

        public SceneLoadingFlow(ISceneOperations sceneOperations, IDelayProvider delayProvider,
            SceneLoadingSettings settings)
        {
            _sceneOperations = sceneOperations;
            _delayProvider = delayProvider;
            _settings = settings;

            _currentSceneData = settings.FirstOpenSceneData;

            _openScenes.Add(settings.FirstOpenSceneData);
        }

        public void LoadScene(SceneData sceneData)
        {
            _hasToKeepOpenScenes = false;

            StartLoading(sceneData);
        }

        public void LoadSceneKeepingOpenScenes(SceneData sceneData)
        {
            _hasToKeepOpenScenes = true;

            StartLoading(sceneData);
        }

        private void StartLoading(SceneData sceneData)
        {
            if (ReferenceEquals(sceneData, null))
                throw new Exception("SceneLoader Error: Trying to Load a null SceneData.");

            if (string.IsNullOrEmpty(sceneData.SceneName))
                throw new Exception("SceneLoader Error: Trying to Load a Scene with empty name.");

            if (_isLoading)
                return;

            _isLoading = true;
            _currentSceneData = sceneData;

            ShowTransitionScene(RemoveScenesBeforeOpening);
        }

        public void ReloadCurrentScene()
        {
            if (_isLoading)
                return;

            _isLoading = true;
            _hasToForceRemoveScenes = true;

            ShowTransitionScene(RemoveScenesBeforeOpening);
        }

        public void RemoveCurrentAndSetPrincipal(SceneData sceneData)
        {
            if (_isLoading)
                return;

            _isLoading = true;
            _nextCurrentSceneData = sceneData;

            ShowTransitionScene(RemoveCurrentScene);
        }

        private void ShowTransitionScene(Action onTransitionSceneShown)
        {
            _onTransitionSceneShown = onTransitionSceneShown;
            _loadingTransitionSceneName = GetTransitionSceneName();

            if (_openTransitionSceneName == _loadingTransitionSceneName)
            {
                TransitionSceneShown();
                return;
            }

            if (!string.IsNullOrEmpty(_openTransitionSceneName))
            {
                UnloadPreviousTransitionScene();
                return;
            }

            LoadTransitionScene();
        }

        private string GetTransitionSceneName()
        {
            if (!_currentSceneData.HasToUseLoadingScreen)
                return _settings.EmptySceneData.SceneName;

            if (ReferenceEquals(_currentSceneData.OverrideLoadingSceneData, null))
                return _settings.LoadingScreenSceneData.SceneName;

            return _currentSceneData.OverrideLoadingSceneData.SceneName;
        }

        private void UnloadPreviousTransitionScene()
        {
            string previousTransitionSceneName = _openTransitionSceneName;
            _openTransitionSceneName = null;

            _sceneOperations.UnloadSceneWithName(previousTransitionSceneName, LoadTransitionScene);
        }

        private void LoadTransitionScene()
        {
            _sceneOperations.LoadSceneWithName(_loadingTransitionSceneName, TransitionSceneLoaded);
        }

        private void TransitionSceneLoaded()
        {
            _openTransitionSceneName = _loadingTransitionSceneName;

            if (!_currentSceneData.HasToUseLoadingScreen)
            {
                TransitionSceneShown();
                return;
            }

            _delayProvider.Wait(_settings.TimeBeforeLoading, TransitionSceneShown);
        }

        private void TransitionSceneShown()
        {
            Action onTransitionSceneShown = _onTransitionSceneShown;
            _onTransitionSceneShown = null;

            onTransitionSceneShown.Invoke();
        }

        private void RemoveScenesBeforeOpening()
        {
            _scenesToRemoveQueue.Clear();

            EnqueueOpenScenesToRemove();
            EnqueueScenesToRemoveFromCurrentSceneData();

            RemoveNextScene();
        }

        private void EnqueueOpenScenesToRemove()
        {
            bool hasToRemoveOpenScenes = _currentSceneData.HasToCloseOthersScenes && !_hasToKeepOpenScenes;

            if (!hasToRemoveOpenScenes && !_hasToForceRemoveScenes)
                return;

            foreach (var openSceneData in _openScenes)
            {
                if (!IsThisSceneDataRemovable(openSceneData))
                    continue;

                _scenesToRemoveQueue.Enqueue(openSceneData);
            }
        }

        private bool IsThisSceneDataRemovable(SceneData sceneData)
        {
            if (sceneData.HasToKeepOpen)
                return false;

            if (sceneData.IsLockedScene && !_currentSceneData.HasToRemoveLockedScenes)
                return false;

            return true;
        }

        private void EnqueueScenesToRemoveFromCurrentSceneData()
        {
            foreach (var sceneDataToRemove in _currentSceneData.GetAllScenesDataToRemove())
            {
                if (!_openScenes.Contains(sceneDataToRemove))
                    continue;

                if (_scenesToRemoveQueue.Contains(sceneDataToRemove))
                    continue;

                _scenesToRemoveQueue.Enqueue(sceneDataToRemove);
            }
        }

        private void RemoveNextScene()
        {
            if (_scenesToRemoveQueue.Count <= 0)
            {
                OpenScenes();
                return;
            }

            SceneData sceneDataToRemove = _scenesToRemoveQueue.Dequeue();
            _openScenes.Remove(sceneDataToRemove);

            _sceneOperations.UnloadSceneWithName(sceneDataToRemove.SceneName, RemoveNextScene);
        }

        private void OpenScenes()
        {
            _scenesToOpenQueue.Clear();

            foreach (var sceneDataToOpen in _currentSceneData.GetAllScenesToOpen())
            {
                _scenesToOpenQueue.Enqueue(sceneDataToOpen);
            }

            OpenNextScene();
        }

        private void OpenNextScene()
        {
            while (_scenesToOpenQueue.Count > 0)
            {
                SceneData sceneDataToOpen = _scenesToOpenQueue.Dequeue();

                if (_openScenes.Contains(sceneDataToOpen))
                    continue;

                _loadingSceneData = sceneDataToOpen;
                _nodeCommands.Clear();

                _sceneOperations.LoadSceneWithName(sceneDataToOpen.SceneName, SceneLoaded);
                return;
            }

            FinishLoading();
        }

        private void SceneLoaded()
        {
            _delayProvider.WaitFrame(SceneReady);
        }

        private void SceneReady()
        {
            _openScenes.Add(_loadingSceneData);

            if (_nodeCommands.Count <= 0)
            {
                OpenNextScene();
                return;
            }

            _commandQueue = new CommandQueue(_nodeCommands.ToArray());
            _commandQueue.OnExecutionDone += SceneInitialized;
            _commandQueue.Execute();
        }

        private void SceneInitialized()
        {
            _commandQueue.OnExecutionDone -= SceneInitialized;
            _commandQueue = null;

            OpenNextScene();
        }

        public void SetNodeCommandOfALoadedScene(INodeCommand nodeCommand)
        {
            if (ReferenceEquals(nodeCommand, null))
                return;

            _nodeCommands.Add(nodeCommand);
        }

        private void RemoveCurrentScene()
        {
            SceneData sceneDataToRemove = _currentSceneData;
            _currentSceneData = _nextCurrentSceneData;
            _nextCurrentSceneData = null;

            _openScenes.Remove(sceneDataToRemove);

            _sceneOperations.UnloadSceneWithName(sceneDataToRemove.SceneName, FinishLoading);
        }

        public void RemoveScene(SceneData sceneData)
        {
            if (!IsThisSceneDataOpened(sceneData))
                return;

            _openScenes.Remove(sceneData);

            _sceneOperations.UnloadSceneWithName(sceneData.SceneName, null);
        }

        public bool IsThisSceneDataOpened(SceneData sceneData)
        {
            return _openScenes.Contains(sceneData);
        }

        private void FinishLoading()
        {
            OnTransitionSceneStartUnloaded?.Invoke();

            SetPrincipalScene();

            if (_currentSceneData.HasToKeepLoadingOpen)
            {
                CompleteLoading();
                return;
            }

            _loadingIsFinishingEventIndex = 0;

            RaiseNextLoadingIsFinishingEvent();
        }

        private void SetPrincipalScene()
        {
            foreach (var openSceneData in _openScenes)
            {
                if (!openSceneData.IsPrincipal)
                    continue;

                _sceneOperations.SetActiveSceneWithName(openSceneData.SceneName);
            }
        }

        private void RaiseNextLoadingIsFinishingEvent()
        {
            if (_loadingIsFinishingEventIndex >= _settings.OnLoadingIsFinishingEventViewModels.Length)
            {
                UnloadTransitionScene();
                return;
            }

            _settings.OnLoadingIsFinishingEventViewModels[_loadingIsFinishingEventIndex].RaiseEvent();
            _loadingIsFinishingEventIndex++;

            _delayProvider.Wait(_settings.TimeBetweenLoadingFinishing, RaiseNextLoadingIsFinishingEvent);
        }

        private void UnloadTransitionScene()
        {
            if (string.IsNullOrEmpty(_openTransitionSceneName))
            {
                CompleteLoading();
                return;
            }

            string transitionSceneName = _openTransitionSceneName;
            _openTransitionSceneName = null;

            _sceneOperations.UnloadSceneWithName(transitionSceneName, CompleteLoading);
        }

        private void CompleteLoading()
        {
            _isLoading = false;
            _hasToKeepOpenScenes = false;
            _hasToForceRemoveScenes = false;

            _settings.OnAllScenesLoadedEventViewModel.RaiseEvent();

            OnAllScenesLoaded?.Invoke();
        }
    }
}
