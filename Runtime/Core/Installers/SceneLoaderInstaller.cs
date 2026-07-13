using DependencyInjector.Core;
using DependencyInjector.Installers;
using MVVM.Core;
using ScenesLoaderSystem.Core.Domain;
using ScenesLoaderSystem.Core.InterfaceAdapters;
using ScenesLoaderSystem.Delays.InterfaceAdapters;
using ServiceLocatorPattern;
using UnityEngine;

namespace ScenesLoaderSystem.Core.Installers
{
    public class SceneLoaderInstaller : MonoInstaller
    {
        [Header("References")]
        [SerializeField] private SceneDataSO _loadingScreenSceneDataSo;
        [SerializeField] private SceneDataSO _emptySceneDataSo;
        [SerializeField] private SceneDataSO _firstOpenSceneDataSo;
        [SerializeField] private EventViewModelSO _onAllSceneAreLoadedEventViewModelSO;
        [SerializeField] private EventViewModelSO[] _onLoadingIsFinishingEventViewModelsSO;

        [Header("Config")]
        [SerializeField] private float _timeBetweenLoadingFinishing = 0;
        [SerializeField] private float _timeBeforeLoading = 0;
        [SerializeField] private float _timeBeforeUnloadingTransitionScene = 0.5f;

        public override void Install(IDIContainer diContainer)
        {
            if (ServiceLocatorInstance.Instance.IsContained<ISceneLoader>())
                return;

            GameObject sceneLoaderGameObject = new GameObject("SceneLoader");
            DontDestroyOnLoad(sceneLoaderGameObject);

            MonoDelayProvider delayProvider = sceneLoaderGameObject.AddComponent<MonoDelayProvider>();

            SceneLoader sceneLoader =
                new SceneLoader(new SceneManagerOperations(), delayProvider, GetSceneLoadingSettings());

            sceneLoaderGameObject.AddComponent<SceneLoaderDisposer>().SetSceneLoader(sceneLoader);

            ServiceLocatorInstance.Instance.Add<ISceneLoader>(sceneLoader);
        }

        private SceneLoadingSettings GetSceneLoadingSettings()
        {
            return new SceneLoadingSettings(_loadingScreenSceneDataSo.GetSceneData(),
                _emptySceneDataSo.GetSceneData(), _firstOpenSceneDataSo.GetSceneData(),
                _onAllSceneAreLoadedEventViewModelSO.GetEventViewModel(), GetLoadingIsFinishingEventViewModels(),
                _timeBetweenLoadingFinishing, _timeBeforeLoading, _timeBeforeUnloadingTransitionScene);
        }

        private IEventViewModel[] GetLoadingIsFinishingEventViewModels()
        {
            int totalEventViewModels = _onLoadingIsFinishingEventViewModelsSO.Length;
            IEventViewModel[] onLoadingIsFinishingEventViewModels = new IEventViewModel[totalEventViewModels];

            for (int i = 0; i < totalEventViewModels; i++)
            {
                onLoadingIsFinishingEventViewModels[i] = _onLoadingIsFinishingEventViewModelsSO[i].GetEventViewModel();
            }

            return onLoadingIsFinishingEventViewModels;
        }
    }
}
