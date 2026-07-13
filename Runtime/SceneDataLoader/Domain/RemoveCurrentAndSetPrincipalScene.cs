using ScenesLoaderSystem.Core.Domain;

namespace ScenesLoaderSystem
{
    public class RemoveCurrentAndSetPrincipalScene : ISceneDataLoader
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly SceneData _sceneData;

        public RemoveCurrentAndSetPrincipalScene(ISceneLoader sceneLoader, SceneData sceneData)
        {
            _sceneLoader = sceneLoader;
            _sceneData = sceneData;
        }

        public void Load()
        {
            _sceneLoader.RemoveCurrentAndSetPrincipal(_sceneData);
        }

        public void LoadKeepingOpenScenes()
        {
            _sceneLoader.RemoveCurrentAndSetPrincipal(_sceneData);
        }
    }
}
