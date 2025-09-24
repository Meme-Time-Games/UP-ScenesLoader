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
        [SerializeField] private EventViewModelSO _onLoadingDoneEventViewModelSO;

        [Header("Config")]
        [SerializeField] private int _loadingDoneDelay = 0;

        public override void Install(IDIContainer diContainer)
        {
            if(ServiceLocatorInstance.Instance.IsContained<ISceneLoader>())
                ServiceLocatorInstance.Instance.Remove<ISceneLoader>();

            SceneLoader sceneLoader = new GameObject("SceneLoader").AddComponent<SceneLoader>();
            sceneLoader.Config(_loadingScreenSceneDataSo.GetSceneData(), _firstOpenSceneDataSo.GetSceneData(), _emptySceneDataSo.GetSceneData(),
                _onAllSceneAreLoadedEventViewModelSO.GetEventViewModel(), _onLoadingDoneEventViewModelSO.GetEventViewModel(), _loadingDoneDelay);

            Transform sceneLoaderTransform = new GameObject("MonoSceneLoader").AddComponent<MonoSceneLoaderDestroyer>().transform;
            sceneLoader.transform.SetParent(sceneLoaderTransform);

            ServiceLocatorInstance.Instance.Add<ISceneLoader>(sceneLoader);
        }
    }
}