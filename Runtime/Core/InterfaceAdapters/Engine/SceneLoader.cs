using System;
using CommandQueues.Core;

namespace ScenesLoaderSystem.Core.Domain
{
    public class SceneLoader : ISceneLoader
    {
        private readonly SceneLoadingFlow _sceneLoadingFlow;

        public Action OnTransitionSceneStartUnloaded
        {
            get => _sceneLoadingFlow.OnTransitionSceneStartUnloaded;
            set => _sceneLoadingFlow.OnTransitionSceneStartUnloaded = value;
        }

        public Action OnAllScenesAreLoaded
        {
            get => _sceneLoadingFlow.OnAllScenesLoaded;
            set => _sceneLoadingFlow.OnAllScenesLoaded = value;
        }

        public SceneLoader(SceneLoadingFlow sceneLoadingFlow)
        {
            _sceneLoadingFlow = sceneLoadingFlow;
        }

        public void LoadScene(SceneData sceneData, bool dontRemoveOpenScenes = false)
        {
            if (dontRemoveOpenScenes)
            {
                _sceneLoadingFlow.LoadSceneKeepingOpenScenes(sceneData);
                return;
            }

            _sceneLoadingFlow.LoadScene(sceneData);
        }

        public void RemoveScene(SceneData sceneData)
        {
            _sceneLoadingFlow.RemoveScene(sceneData);
        }

        public void SetNodeCommandOfALoadedScene(INodeCommand nodeCommand)
        {
            _sceneLoadingFlow.SetNodeCommandOfALoadedScene(nodeCommand);
        }

        public void RemoveCurrentAndSetPrincipal(SceneData currentSceneData)
        {
            _sceneLoadingFlow.RemoveCurrentAndSetPrincipal(currentSceneData);
        }

        public bool IsThisSceneDataOpened(SceneData sceneData)
        {
            return _sceneLoadingFlow.IsThisSceneDataOpened(sceneData);
        }

        public void ReloadCurrentScene()
        {
            _sceneLoadingFlow.ReloadCurrentScene();
        }
    }
}
