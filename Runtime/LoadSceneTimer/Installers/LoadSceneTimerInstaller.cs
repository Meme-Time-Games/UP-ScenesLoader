using DependencyInjector.Core;
using DependencyInjector.Installers;
using ScenesLoaderSystem.Delays.InterfaceAdapters;
using UnityEngine;

namespace ScenesLoaderSystem
{
    public class LoadSceneTimerInstaller : MonoInstaller
    {
        [Header("Config")]
        [SerializeField] private float _duration;

        [Inject] private ISceneDataLoader _sceneDataLoader;

        public override void Install(IDIContainer diContainer)
        {
            MonoDelayProvider delayProvider = gameObject.AddComponent<MonoDelayProvider>();

            LoadSceneTimer loadSceneTimer = new LoadSceneTimer(_sceneDataLoader, delayProvider);

            loadSceneTimer.StartTimerWithDuration(_duration);
        }
    }
}
