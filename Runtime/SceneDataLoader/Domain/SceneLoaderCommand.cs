using Commands.Core;

namespace ScenesLoaderSystem
{
    public class SceneLoaderCommand : ICommand
    {
        private readonly ISceneDataLoader _sceneLoader;

        public SceneLoaderCommand(ISceneDataLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public void Execute()
        {
            _sceneLoader.Load();
        }
    }
}