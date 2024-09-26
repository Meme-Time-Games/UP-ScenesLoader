using CommandQueues.Core;

namespace ScenesLoaderSystem.Core.Domain
{
    public class SceneLoadedNotifier
    {
        public SceneLoadedNotifier(ISceneLoader sceneLoader, ICommandQueue commandQueue)
        {
            sceneLoader.SetNodeCommandOfALoadedScene(commandQueue);
        }
    }
}