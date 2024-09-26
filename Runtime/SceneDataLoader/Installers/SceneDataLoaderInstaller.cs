using DependencyInjector.Installers;
using ScenesLoaderSystem.Core.Domain;
using ScenesLoaderSystem.Core.InterfaceAdapters;
using ServiceLocatorPattern;
using UnityEngine;

namespace ScenesLoaderSystem
{
    public class SceneDataLoaderInstaller : SingleMonoInstaller<ISceneDataLoader>
    {
        [Header("References")]
        [SerializeField] private SceneDataSO _sceneDataSO;

        protected override ISceneDataLoader GetData()
        {
            ISceneLoader sceneLoader = ServiceLocatorInstance.Instance.Get<ISceneLoader>();

            return new SceneDataLoader(sceneLoader, _sceneDataSO.GetSceneData());
        }
    }
}