using Commands.Core;
using DependencyInjector.Installers;
using ScenesLoaderSystem.Core.Domain;
using ScenesLoaderSystem.Core.InterfaceAdapters;
using ServiceLocatorPattern;
using UnityEngine;

namespace ScenesLoaderSystem
{
    public class RemoveSceneCommandInstaller : SingleMonoInstaller<ICommand>
    {
        [Header("References")]
        [SerializeField] private SceneDataSO _sceneDataToRemove;
        
        protected override ICommand GetData()
        {
            ISceneLoader sceneLoader = ServiceLocatorInstance.Instance.Get<ISceneLoader>();
            
            return new RemoveSceneCommand(sceneLoader, _sceneDataToRemove.GetSceneData());
        }
    }
}