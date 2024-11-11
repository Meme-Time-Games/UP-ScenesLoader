using Commands.Core;
using DependencyInjector.Core;
using DependencyInjector.Installers;

namespace ScenesLoaderSystem
{
    public class SceneLoaderCommandInstaller : SingleMonoInstaller<ICommand>
    {
        [Inject] private ISceneDataLoader _sceneDataLoader;

        protected override ICommand GetData()
        {
            return new SceneLoaderCommand(_sceneDataLoader);
        }
    }
}