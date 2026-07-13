using ScenesLoaderSystem.Core.Domain;

namespace ScenesLoaderSystem
{
    public class SceneDataLoader : ISceneDataLoader
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly SceneData _sceneData;

        public SceneDataLoader(ISceneLoader sceneLoader, SceneData sceneData)
        {
            _sceneLoader = sceneLoader;
            _sceneData = sceneData;
        }

        public void Load()
        {
            _sceneLoader.LoadScene(_sceneData);
        }

        public void LoadKeepingOpenScenes()
        {
            _sceneLoader.LoadSceneKeepingOpenScenes(_sceneData);
        }

        public void RemoveCurrentAndSetPrincipalSceneData()
        {
            _sceneLoader.RemoveCurrentAndSetPrincipal(_sceneData);
        }

        public bool IsThisSceneDataOpened()
        {
            return _sceneLoader.IsThisSceneDataOpened(_sceneData);
        }
    }
}
