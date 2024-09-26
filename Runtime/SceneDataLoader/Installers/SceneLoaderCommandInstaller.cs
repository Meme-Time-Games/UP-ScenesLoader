using Commands.Core;
using DependencyInjector.Core;
using DependencyInjector.Installers;

namespace ScenesLoaderSystem
{
    public class SceneLoaderCommandInstaller : MultipleMonoInstaller<ICommand>
    {
        private ISceneDataLoader _sceneDataLoader;

        [Inject]
        public void InjectSceneDataLoader(ISceneDataLoader sceneDataLoader)
        {
            _sceneDataLoader = sceneDataLoader;
        }

        protected override ICommand GetData()
        {
            return new SceneLoaderCommand(_sceneDataLoader);
        }
    }
}