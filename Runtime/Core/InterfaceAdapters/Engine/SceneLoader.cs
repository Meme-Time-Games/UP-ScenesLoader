using System;
using System.Collections;
using System.Collections.Generic;
using CommandQueues.Core;
using MVVM.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ScenesLoaderSystem.Core.Domain
{
    public class SceneLoader : MonoBehaviour, ISceneLoader
    {
        private SceneData _loadingScreenSceneData;
        private SceneData _emptySceneData;
        private IEventViewModel _onAllSceneAreLoadedEventViewModel;
        private IEventViewModel[] _onLoadingIsFinishingEventViewModels;

        private SceneData _currentSceneData;
        private List<SceneData> _openScenes = new List<SceneData>();
        private Queue<SceneData> _scenesToOpenQueue;
        private List<INodeCommand> _nodeCommands;
        private CommandQueue _commandQueue;
        private WaitForEndOfFrame _waitForEndOfFrame;
        private WaitForSeconds _waitForOneSecond;
        private WaitForSeconds _timeBetweenLoadingFinishingWaitForSeconds;
        private WaitForSeconds _timeBeforeLoadingWaitForSeconds;
        
        private float _loadingProgress;
        private bool _isLoading;
        private float _loadingPercentagePerScene;
        
        public Action OnTransitionSceneStartUnloaded { get; set; }
        public Action OnAllScenesAreLoaded { get; set; }

        public void Config(SceneData loadingScreenSceneData, SceneData firstOpenSceneData, SceneData emptySceneData, IEventViewModel onAllSceneAreLoadedEventViewModel,
            IEventViewModel[] onLoadingIsFinishingEventViewModels, float timeBetweenLoadingFinishing = 0, float timeBeforeLoadingWaitForSeconds = 0)
        {
            _loadingScreenSceneData = loadingScreenSceneData;
            _emptySceneData = emptySceneData;
            _onAllSceneAreLoadedEventViewModel = onAllSceneAreLoadedEventViewModel;
            _onLoadingIsFinishingEventViewModels = onLoadingIsFinishingEventViewModels;
            
            _timeBetweenLoadingFinishingWaitForSeconds = new WaitForSeconds(timeBetweenLoadingFinishing);
            _timeBeforeLoadingWaitForSeconds = new WaitForSeconds(timeBeforeLoadingWaitForSeconds);
            _waitForEndOfFrame = new WaitForEndOfFrame();
            _waitForOneSecond = new WaitForSeconds(0.25f);

            _openScenes.Add(firstOpenSceneData);
        }

        public void RemoveCurrentAndSetPrincipal(SceneData currentSceneData)
        {
            StartCoroutine(RemoveCurrentAndSetPrincipalAsync(currentSceneData));
        }

        private IEnumerator RemoveCurrentAndSetPrincipalAsync(SceneData currentSceneData)
        {
            yield return StartCoroutine(LoadLoadingScreen());

            yield return StartCoroutine(RemoveSceneAsync(_currentSceneData));
            _openScenes.Remove(_currentSceneData);

            _currentSceneData = currentSceneData;

            AllSceneLoaded();
        }
        
        private IEnumerator LoadLoadingScreen()
        {
            if (!_currentSceneData.HasToUseLoadingScreen)
                yield break;
            
            if(_isLoading)
                yield break;

            _isLoading = true;
            
            string loadingScreenSceneName = _loadingScreenSceneData.SceneName;
            if(_currentSceneData.OverrideLoadingSceneData != null)
                loadingScreenSceneName = _currentSceneData.OverrideLoadingSceneData.SceneName;
                
            yield return StartCoroutine(LoadSceneAsync(loadingScreenSceneName));

            yield return _timeBeforeLoadingWaitForSeconds;
        }
        
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            AsyncOperation loadLoadingOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            while (!loadLoadingOperation.isDone)
            {
                yield return null;
            }
        }

        public void LoadScene(SceneData sceneData, bool dontRemoveOpenScenes = false)
        {
            if (string.IsNullOrEmpty(sceneData.SceneName))
                throw new Exception("SceneLoader Error: Trying to Load a Scene with empty name.");
            
            StartCoroutine(LoadSceneAsync(sceneData, dontRemoveOpenScenes));
        }
        
        private IEnumerator LoadSceneAsync(SceneData sceneData, bool dontRemoveOpenScenes = false)
        {
            _currentSceneData = sceneData;
            _loadingProgress = 0;
            
            if(sceneData.HasToUseLoadingScreen)
                yield return StartCoroutine(LoadLoadingScreen());
            else
                yield return StartCoroutine(LoadEmptyScreenAsync());

            bool hasToCloseOtherScenes = sceneData.HasToCloseOthersScenes;
            bool mustRemoveOpenScenes = !dontRemoveOpenScenes;

            if (hasToCloseOtherScenes && mustRemoveOpenScenes)
                yield return StartCoroutine(RemoveScenes(_currentSceneData.HasToRemoveLockedScenes));

            yield return StartCoroutine(RemoveScenesFromCurrentSceneData());

            OpenScenes();
        }
        
        private IEnumerator LoadEmptyScreenAsync()
        {
            yield return StartCoroutine(LoadSceneAsync(_emptySceneData.SceneName));
        }
        
        private IEnumerator RemoveScenes(bool removeLockedScenes)
        {
            for (int i = _openScenes.Count - 1; i >= 0; i--)
            {
                SceneData openSceneData = _openScenes[i];
                
                if (openSceneData.IsLockedScene && !removeLockedScenes || openSceneData.HasToKeepOpen)
                {
                    continue;
                }

                yield return StartCoroutine(RemoveSceneAsync(openSceneData));
                
                _openScenes.Remove(openSceneData);
            }
        }

        private IEnumerator RemoveSceneAsync(SceneData openScene)
        {
            AsyncOperation removeSceneOperation = SceneManager.UnloadSceneAsync(openScene.SceneName);

            if (removeSceneOperation != null)
            {
                yield return removeSceneOperation;
            }
        }

        private IEnumerator RemoveScenesFromCurrentSceneData()
        {
            SceneData[] scenes = _currentSceneData.GetAllScenesDataToRemove();

            if (ReferenceEquals(scenes, null))
                yield return null;

            foreach (var sceneData in scenes)
            {
                if (!_openScenes.Contains(sceneData))
                    continue;

                yield return StartCoroutine(RemoveSceneAsync(sceneData));
                _openScenes.Remove(sceneData);
            }
        }
        
        public void RemoveScene(SceneData sceneData)
        {
            if (!IsThisSceneDataOpened(sceneData))
                return;
            
            StartCoroutine(RemoveSceneAsync(sceneData));

            _openScenes.Remove(sceneData);
        }

        private void OpenScenes()
        {
            SceneData[] scenesToLoad = _currentSceneData.GetAllScenesToOpen();
            
            _loadingPercentagePerScene = 1f / scenesToLoad.Length;
            _scenesToOpenQueue = new Queue<SceneData>();
            _nodeCommands = new List<INodeCommand>();

            foreach (var sceneData in scenesToLoad)
            {
                _scenesToOpenQueue.Enqueue(sceneData);
            }

            OpenNextScene();
        }

        private void OpenNextScene()
        {
            while (true)
            {
                if (_scenesToOpenQueue.Count <= 0) 
                    return;

                SceneData sceneData = _scenesToOpenQueue.Dequeue();

                if (_openScenes.Contains(sceneData))
                    continue;
                
                OpenScene(sceneData);
                break;
            }
        }

        private void OpenScene(SceneData sceneData)
        {
            SceneManager.LoadSceneAsync(sceneData.SceneName, LoadSceneMode.Additive);

            _openScenes.Add(sceneData);
        }

        public void SetNodeCommandOfALoadedScene(INodeCommand nodeCommand)
        {
            StartCoroutine(SetNodeCommandOfALoadedSceneCoroutine(nodeCommand));
        }

        private IEnumerator SetNodeCommandOfALoadedSceneCoroutine(INodeCommand nodeCommand)
        {
            _loadingProgress += _loadingPercentagePerScene;
            
            yield return _waitForEndOfFrame;
            
            if (nodeCommand != null)
            {
                _nodeCommands.Add(nodeCommand);
                InitializeScene();
                yield break;
            }
            
            if (_scenesToOpenQueue.Count <= 0)
            {
                AllSceneLoaded();
                yield break;
            }

            OpenNextScene();
        }

        private void InitializeScene()
        {
            _commandQueue = new CommandQueue(_nodeCommands.ToArray());
            _commandQueue.OnExecutionDone += InitializeNextScene;
            _commandQueue.Execute();
        }

        private void InitializeNextScene()
        {
            _commandQueue.OnExecutionDone -= InitializeNextScene;
            
            _nodeCommands.Clear();
            if (_scenesToOpenQueue.Count <= 0)
            {
                AllSceneLoaded();
                return;
            }

            OpenNextScene();
        }

        private void AllSceneLoaded()
        {
            StartCoroutine(AllSceneLoadedCoroutine());
        }

        private IEnumerator AllSceneLoadedCoroutine()
        {
            _loadingProgress = 1;
            
            if (_commandQueue != null)
                _commandQueue.OnExecutionDone -= AllSceneLoaded;

            OnTransitionSceneStartUnloaded?.Invoke();

            yield return _waitForOneSecond;
            
            SetPrincipalScene();

            //Added for security reasons because not always load the scene correctly, so we need to wait the main thread
            yield return _waitForOneSecond;
            
            if(!_currentSceneData.HasToKeepLoadingOpen)
                yield return UnloadTransitionScenes();
            
            _onAllSceneAreLoadedEventViewModel.RaiseEvent();

            OnAllScenesAreLoaded?.Invoke();
        }

        private IEnumerator UnloadTransitionScenes()
        {
            foreach (var loadingIsFinishingEventViewModel in _onLoadingIsFinishingEventViewModels)
            {
                loadingIsFinishingEventViewModel.RaiseEvent();
                yield return _timeBetweenLoadingFinishingWaitForSeconds;
            }
            
            string overrideLoadingScreenSceneName = _currentSceneData.OverrideLoadingSceneData?.SceneName;
            
            int totalScene = SceneManager.sceneCount;
            for (int i = 0; i < totalScene; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (scene.name == _loadingScreenSceneData.SceneName || 
                    scene.name == _emptySceneData.SceneName ||
                    scene.name == overrideLoadingScreenSceneName)
                    SceneManager.UnloadSceneAsync(scene);
            }

            _isLoading = false;
        }

        private void SetPrincipalScene()
        {
            foreach (var sceneData in _openScenes)
            {
                if (!sceneData.IsPrincipal)
                    continue;

                SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneData.SceneName));
            }
        }

        public bool IsThisSceneDataOpened(SceneData sceneData)
        {
            return _openScenes.Contains(sceneData);
        }

        public void ReloadCurrentScene()
        {
            StartCoroutine(ReloadCurrentSceneAsync());
        }
        
        private IEnumerator ReloadCurrentSceneAsync()
        {
            string loadingScreenSceneName = _loadingScreenSceneData.SceneName;
            if(_currentSceneData.OverrideLoadingSceneData != null)
                loadingScreenSceneName = _currentSceneData.OverrideLoadingSceneData.SceneName;
            
            yield return StartCoroutine(LoadSceneAsync(loadingScreenSceneName));
            
            yield return StartCoroutine(LoadSceneAsync(_emptySceneData.SceneName));
            yield return StartCoroutine(RemoveScenes(_currentSceneData.HasToRemoveLockedScenes));
            
            LoadScene(_currentSceneData);
        }
    }
}