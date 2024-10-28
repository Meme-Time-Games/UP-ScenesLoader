using System;
using DependencyInjector.Installers;
using ScenesLoaderSystem.Core.Domain;
using ServiceLocatorPattern;
using UnityEngine;

namespace ScenesLoaderSystem.Installers
{
    public class AllSceneLoadedInjectorInitializer : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private BaseMonoInjector _monoInjector;

        private ISceneLoader _sceneLoader;

        private void Awake()
        {
            _sceneLoader = ServiceLocatorInstance.Instance.Get<ISceneLoader>();

            if(null == _sceneLoader)
                throw new Exception("The ISceneLoader is not instantiated.");

            _sceneLoader.OnAllScenesAreLoaded += Inject;
        }

        private void Inject()
        {
            _monoInjector.InjectAll();
        }

        private void OnDestroy()
        {
            _sceneLoader.OnAllScenesAreLoaded -= Inject;
        }
    }
}