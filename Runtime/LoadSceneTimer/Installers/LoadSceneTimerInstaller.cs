using DependencyInjector.Core;
using DependencyInjector.Installers;
using UnityEngine;

namespace ScenesLoaderSystem
{
    public class LoadSceneTimerInstaller :  MonoInstaller
    {
        [Header("Config")]
        [SerializeField] private int _duration;
        
        [Inject]
        private ISceneDataLoader _sceneDataLoader;
        
        public override void Install(IDIContainer diContainer)
        {
            new LoadSceneTimer(_duration, _sceneDataLoader);
        }
    }
}