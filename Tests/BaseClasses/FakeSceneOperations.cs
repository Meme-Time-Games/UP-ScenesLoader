using System;
using System.Collections.Generic;
using ScenesLoaderSystem.Core.Domain;

namespace ScenesLoaderSystem.Tests
{
    public class FakeSceneOperations : ISceneOperations
    {
        private readonly List<string> _loadedScenes = new List<string>();
        private readonly List<string> _unloadedScenes = new List<string>();
        private readonly List<string> _activatedScenes = new List<string>();
        private readonly List<string> _scenesInTheHierarchy = new List<string>();
        private readonly Queue<Action> _pendingOperations = new Queue<Action>();
        private string _sceneNameThatFailsToLoad;
        private bool _hasManualCompletion;

        public IReadOnlyList<string> LoadedScenes => _loadedScenes;
        public IReadOnlyList<string> UnloadedScenes => _unloadedScenes;
        public IReadOnlyList<string> ActivatedScenes => _activatedScenes;
        public int TotalPendingOperations => _pendingOperations.Count;

        public void SetManualCompletion()
        {
            _hasManualCompletion = true;
        }

        public void SetSceneAsAlreadyLoaded(string sceneName)
        {
            _scenesInTheHierarchy.Add(sceneName);
        }

        public void SetSceneThatFailsToLoad(string sceneName)
        {
            _sceneNameThatFailsToLoad = sceneName;
        }

        public void LoadSceneWithName(string sceneName, Action onSceneLoaded)
        {
            if (sceneName == _sceneNameThatFailsToLoad)
                throw new Exception($"FakeSceneOperations: the Scene {sceneName} is not in the Build Settings.");

            _loadedScenes.Add(sceneName);
            _scenesInTheHierarchy.Add(sceneName);

            CompleteOperation(onSceneLoaded);
        }

        public void UnloadSceneWithName(string sceneName, Action onSceneUnloaded)
        {
            _unloadedScenes.Add(sceneName);
            _scenesInTheHierarchy.Remove(sceneName);

            CompleteOperation(onSceneUnloaded);
        }

        public void SetActiveSceneWithName(string sceneName)
        {
            _activatedScenes.Add(sceneName);
        }

        public bool IsSceneLoadedWithName(string sceneName)
        {
            return _scenesInTheHierarchy.Contains(sceneName);
        }

        private void CompleteOperation(Action onOperationDone)
        {
            if (_hasManualCompletion)
            {
                _pendingOperations.Enqueue(onOperationDone);
                return;
            }

            onOperationDone?.Invoke();
        }

        public void CompleteNextOperation()
        {
            Action onOperationDone = _pendingOperations.Dequeue();

            onOperationDone?.Invoke();
        }

        public void CompleteAllOperations()
        {
            while (_pendingOperations.Count > 0)
            {
                CompleteNextOperation();
            }
        }

        public int GetTotalLoadsOfScene(string sceneName)
        {
            int totalLoads = 0;

            foreach (var loadedScene in _loadedScenes)
            {
                if (loadedScene != sceneName)
                    continue;

                totalLoads++;
            }

            return totalLoads;
        }

        public int GetTotalUnloadsOfScene(string sceneName)
        {
            int totalUnloads = 0;

            foreach (var unloadedScene in _unloadedScenes)
            {
                if (unloadedScene != sceneName)
                    continue;

                totalUnloads++;
            }

            return totalUnloads;
        }
    }
}
