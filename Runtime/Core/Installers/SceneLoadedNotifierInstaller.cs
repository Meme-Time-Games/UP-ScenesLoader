using CommandQueues.Core;
using DependencyInjector.Core;
using DependencyInjector.Installers;
using ScenesLoaderSystem.Core.Domain;
using ServiceLocatorPattern;

namespace ScenesLoaderSystem
{
    public class SceneLoadedNotifierInstaller : MonoInstaller
    {
        [Inject] private ICommandQueue _commandQueue;
        
        private ISceneLoader _sceneLoader;

        public override void Install(IDIContainer diContainer)
        {
            _sceneLoader = ServiceLocatorInstance.Instance.Get<ISceneLoader>();
            
            new SceneLoadedNotifier(_sceneLoader, _commandQueue);
        }
    }
}