using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandQueues.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ScenesLoaderSystem.Core.Domain
{
    public class SceneLoader : MonoBehaviour, ISceneLoader
    {
        private SceneData _loadingScreenSceneData;
        private SceneData _emptySceneData;

        private SceneData _currentSceneData;
        private List<SceneData> _openScenes = new List<SceneData>();
        private Queue<SceneData> _scenesToOpenQueue;
        private List<INodeCommand> _nodeCommands;
        private CommandQueue _commandQueue;

        public Action OnTransitionSceneStartUnloaded { get; set; }
        public Action OnAllScenesAreLoaded { get; set; }

        public void Config(SceneData loadingScreenSceneData, SceneData firstOpenSceneData, SceneData emptySceneData)
        {
            _loadingScreenSceneData = loadingScreenSceneData;
            _emptySceneData = emptySceneData;

            _openScenes.Add(firstOpenSceneData);
        }

        public void RemoveCurrentAndSetPrincipal(SceneData currentSceneData)
        {
            StartCoroutine(RemoveCurrentAndSetPrincipalAsync(currentSceneData));
        }

        public IEnumerator RemoveCurrentAndSetPrincipalAsync(SceneData currentSceneData)
        {
            yield return StartCoroutine(LoadLoadingScreenAsync());

            yield return StartCoroutine(RemoveScene(_currentSceneData));
            _openScenes.Remove(_currentSceneData);

            _currentSceneData = currentSceneData;

            AllSceneLoaded();
        }

        public void LoadScene(SceneData sceneData, bool dontRemoveOpenScenes = false)
        {
            if (string.IsNullOrEmpty(sceneData.SceneName))
                throw new Exception("SceneLoader Error: Trying to Load a Scene with empty name.");
            
            StartCoroutine(LoadSceneAsync(sceneData, dontRemoveOpenScenes));
        }

        public IEnumerator LoadSceneAsync(SceneData sceneData, bool dontRemoveOpenScenes = false)
        {
            _currentSceneData = sceneData;
            
            if(sceneData.HasToUseLoadingScreen)
                yield return StartCoroutine(LoadLoadingScreenAsync());
            else
                yield return StartCoroutine(LoadEmptyScreenAsync());

            bool hasToCloseOtherScenes = sceneData.HasToCloseOthersScenes;
            bool mustRemoveOpenScenes = !dontRemoveOpenScenes;

            if (hasToCloseOtherScenes && mustRemoveOpenScenes)
                yield return StartCoroutine(RemoveScenes(_currentSceneData.HasToRemoveLockedScenes));

            yield return StartCoroutine(RemoveScenesFromCurrentSceneData());

            OpenScenes();
        }
        
        private IEnumerator RemoveScenes(bool removeLockedScenes)
        {
            for (int i = 0; i < _openScenes.Count; i++)
            {
                if (_openScenes[i].IsLockedScene && !removeLockedScenes || _openScenes[i].HasToKeepOpen)
                    continue;

                yield return StartCoroutine(RemoveScene(_openScenes[i]));

                _openScenes.Remove(_openScenes[i]);

                i--;
            }
        }

        private IEnumerator RemoveScene(SceneData openScene)
        {
            AsyncOperation removeSceneOperation = SceneManager.UnloadSceneAsync(openScene.SceneName);

            while (!removeSceneOperation.isDone)
            {
                yield return null;
            }
        }
        
        private IEnumerator LoadLoadingScreenAsync()
        {
            if (_currentSceneData.HasToUseLoadingScreen == false)
                yield return null;

            yield return StartCoroutine(LoadSceneAsync(_loadingScreenSceneData.SceneName));

            yield return new WaitForEndOfFrame();
        }
        
        private IEnumerator LoadEmptyScreenAsync()
        {
            yield return StartCoroutine(LoadSceneAsync(_emptySceneData.SceneName));
        }
        
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            AsyncOperation loadLoadingOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            while (!loadLoadingOperation.isDone)
            {
                yield return null;
            }
        }

        private IEnumerator RemoveScenesFromCurrentSceneData()
        {
            SceneData[] sceneDatas = _currentSceneData.GetAllScenesDataToRemove();

            if (ReferenceEquals(sceneDatas, null))
                yield return null;

            foreach (var sceneData in sceneDatas)
            {
                if (!_openScenes.Contains(sceneData))
                    continue;

                yield return StartCoroutine(RemoveScene(sceneData));
                _openScenes.Remove(sceneData);
            }
        }

        private void OpenScenes()
        {
            SceneData[] scenesToLoad = _currentSceneData.GetAllScenesToOpen();
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

        public async void SetNodeCommandOfALoadedScene(INodeCommand nodeCommand)
        {
            //Added for security reasons because not always load the scene correctly, so we need to wait the main thread
            await Task.Delay(10);

            if (nodeCommand != null)
                _nodeCommands.Add(nodeCommand);

            if (_scenesToOpenQueue.Count <= 0)
            {
                InitializeScenes();
                return;
            }

            OpenNextScene();
        }

        private void InitializeScenes()
        {
            if (_nodeCommands.Count <= 0)
            {
                AllSceneLoaded();
                return;
            }

            _commandQueue = new CommandQueue(_nodeCommands.ToArray());
            _commandQueue.OnExecutionDone += AllSceneLoaded;
            _commandQueue.Execute();
        }

        private async void AllSceneLoaded()
        {
            if (_commandQueue != null)
                _commandQueue.OnExecutionDone -= AllSceneLoaded;

            OnTransitionSceneStartUnloaded?.Invoke();
            
            await Task.Delay(TimeSpan.FromSeconds(1));
            
            SetPrincipalScene();

            //Added for security reasons because not always load the scene correctly, so we need to wait the main thread
            await Task.Delay(10);

            UnloadTransitionScenes();

            OnAllScenesAreLoaded?.Invoke();
        }

        private void UnloadTransitionScenes()
        {
            int totalScene = SceneManager.sceneCount;
            for (int i = 0; i < totalScene; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (scene.name == _loadingScreenSceneData.SceneName || scene.name == _emptySceneData.SceneName)
                    SceneManager.UnloadSceneAsync(scene);
            }
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
            yield return StartCoroutine( LoadSceneAsync(_loadingScreenSceneData.SceneName));
            yield return StartCoroutine( LoadSceneAsync(_emptySceneData.SceneName));

            yield return StartCoroutine(RemoveScenes(_currentSceneData.HasToRemoveLockedScenes));
            
            LoadScene(_currentSceneData);
        }
    }
}