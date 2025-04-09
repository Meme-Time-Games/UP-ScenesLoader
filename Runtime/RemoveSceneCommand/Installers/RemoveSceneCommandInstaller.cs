using Commands.Core;
using DependencyInjector.Installers;
using ScenesLoaderSystem.Core.Domain;
using ServiceLocatorPattern;
using UnityEngine;

namespace ScenesLoaderSystem
{
    public class RemoveSceneCommandInstaller : SingleMonoInstaller<ICommand>
    {
        [Header("References")]
        [SerializeField] private SceneData _sceneDataToRemove;
        
        protected override ICommand GetData()
        {
            ISceneLoader sceneLoader = ServiceLocatorInstance.Instance.Get<ISceneLoader>();
            
            return new RemoveSceneCommand(sceneLoader, _sceneDataToRemove);
        }
    }
}