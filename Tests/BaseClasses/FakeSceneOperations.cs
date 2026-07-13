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
        private readonly Queue<Action> _pendingOperations = new Queue<Action>();
        private bool _hasManualCompletion;

        public IReadOnlyList<string> LoadedScenes => _loadedScenes;
        public IReadOnlyList<string> UnloadedScenes => _unloadedScenes;
        public IReadOnlyList<string> ActivatedScenes => _activatedScenes;
        public int TotalPendingOperations => _pendingOperations.Count;

        public void SetManualCompletion()
        {
            _hasManualCompletion = true;
        }

        public void LoadSceneWithName(string sceneName, Action onSceneLoaded)
        {
            _loadedScenes.Add(sceneName);

            CompleteOperation(onSceneLoaded);
        }

        public void UnloadSceneWithName(string sceneName, Action onSceneUnloaded)
        {
            _unloadedScenes.Add(sceneName);

            CompleteOperation(onSceneUnloaded);
        }

        public void SetActiveSceneWithName(string sceneName)
        {
            _activatedScenes.Add(sceneName);
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
    }
}
