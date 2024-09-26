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

        public void Load(bool dontRemoveOpenScenes = false)
        {
            _sceneLoader.LoadScene(_sceneData, dontRemoveOpenScenes);
        }

        public void RemoveCurrentAndSetPrincipalSceneData()
        {
            _sceneLoader.RemoveCurrentAndSetPrincipal(_sceneData);
        }

        public bool IsThiSceneDataOpened()
        {
            return _sceneLoader.IsThisSceneDataOpened(_sceneData);
        }
    }
}