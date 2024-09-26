using ScenesLoaderSystem.Core.Domain;
using ServiceLocatorPattern;
using SingletonPattern;

namespace ScenesLoaderSystem.Core.Installers
{
    public class MonoSceneLoaderDestroyer : MonoSingleton<MonoSceneLoaderDestroyer>
    {
        private void Awake()
        {
            InitializeSingleton();
            PassTroughScenes();
        }

        private void OnDestroy()
        {
            ServiceLocatorInstance.Instance.Remove<ISceneLoader>();
        }
    }
}