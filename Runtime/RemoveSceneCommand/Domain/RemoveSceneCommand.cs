using Commands.Core;
using ScenesLoaderSystem.Core.Domain;

namespace ScenesLoaderSystem
{
    public class RemoveSceneCommand : ICommand
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly SceneData _sceneDataToRemove;

        public RemoveSceneCommand(ISceneLoader sceneLoader, SceneData sceneDataToRemove)
        {
            _sceneLoader = sceneLoader;
            _sceneDataToRemove = sceneDataToRemove;
        }

        public void Execute()
        {
            _sceneLoader.RemoveScene(_sceneDataToRemove);
        }
    }
}