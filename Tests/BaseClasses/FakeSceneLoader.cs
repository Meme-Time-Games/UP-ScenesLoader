using System;
using System.Collections.Generic;
using CommandQueues.Core;
using ScenesLoaderSystem.Core.Domain;

namespace ScenesLoaderSystem.Tests
{
    public class FakeSceneLoader : ISceneLoader
    {
        private readonly List<SceneData> _loadedScenesData = new List<SceneData>();
        private readonly List<SceneData> _removedScenesData = new List<SceneData>();
        private readonly List<SceneData> _openScenesData = new List<SceneData>();
        private int _totalReloads;

        public Action OnTransitionSceneStartUnloaded { get; set; }
        public Action OnAllScenesLoaded { get; set; }

        public IReadOnlyList<SceneData> LoadedScenesData => _loadedScenesData;
        public IReadOnlyList<SceneData> RemovedScenesData => _removedScenesData;
        public int TotalReloads => _totalReloads;

        public void LoadScene(SceneData sceneData)
        {
            _loadedScenesData.Add(sceneData);
            _openScenesData.Add(sceneData);
        }

        public void LoadSceneKeepingOpenScenes(SceneData sceneData)
        {
            _loadedScenesData.Add(sceneData);
            _openScenesData.Add(sceneData);
        }

        public void RemoveScene(SceneData sceneData)
        {
            _removedScenesData.Add(sceneData);
            _openScenesData.Remove(sceneData);
        }

        public void SetNodeCommandOfALoadedScene(INodeCommand nodeCommand)
        {
        }

        public void RemoveCurrentAndSetPrincipal(SceneData currentSceneData)
        {
            _removedScenesData.Add(currentSceneData);
        }

        public bool IsThisSceneDataOpened(SceneData sceneData)
        {
            return _openScenesData.Contains(sceneData);
        }

        public void ReloadCurrentScene()
        {
            _totalReloads++;
        }

        public void RaiseAllScenesLoaded()
        {
            OnAllScenesLoaded?.Invoke();
        }
    }
}
