using DependencyInjector.Core;
using DependencyInjector.Installers;
using MVVM.Core;
using ScenesLoaderSystem.Core.Domain;
using ScenesLoaderSystem.Core.InterfaceAdapters;
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

        public override void Install(IDIContainer diContainer)
        {
            if(ServiceLocatorInstance.Instance.IsContained<ISceneLoader>())
                ServiceLocatorInstance.Instance.Remove<ISceneLoader>();

            IEventViewModel[] onLoadingIsFinishingEventViewModels = new IEventViewModel[_onLoadingIsFinishingEventViewModelsSO.Length];
            for (int i = 0; i < _onLoadingIsFinishingEventViewModelsSO.Length; i++)
            {
                onLoadingIsFinishingEventViewModels[i] = _onLoadingIsFinishingEventViewModelsSO[i].GetEventViewModel();           
            }
            
            SceneLoader sceneLoader = new GameObject("SceneLoader").AddComponent<SceneLoader>();
            sceneLoader.Config(_loadingScreenSceneDataSo.GetSceneData(), _firstOpenSceneDataSo.GetSceneData(), _emptySceneDataSo.GetSceneData(),
                _onAllSceneAreLoadedEventViewModelSO.GetEventViewModel(), onLoadingIsFinishingEventViewModels, _timeBeforeLoading);

            Transform sceneLoaderTransform = new GameObject("MonoSceneLoader").AddComponent<MonoSceneLoaderDestroyer>().transform;
            sceneLoader.transform.SetParent(sceneLoaderTransform);

            ServiceLocatorInstance.Instance.Add<ISceneLoader>(sceneLoader);
        }
    }
}