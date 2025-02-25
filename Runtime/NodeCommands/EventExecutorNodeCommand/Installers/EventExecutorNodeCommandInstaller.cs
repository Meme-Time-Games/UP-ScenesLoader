using CommandQueues.Core;
using MVVM.Core;
using UnityEngine;

namespace ScenesLoaderSystem
{
    public class EventExecutorNodeCommandInstaller : NodeCommandInstaller
    {
        [SerializeField] private EventViewModelSO _onSceneLoadStartedEventViewModel;
        [SerializeField] private EventViewModelSO _onSceneLoadDoneEventViewModel;
        
        protected override INodeCommand GetData()
        {
            return new EventExecutorNodeCommand(_onSceneLoadStartedEventViewModel.GetEventViewModel(), _onSceneLoadDoneEventViewModel.GetEventViewModel());
        }
    }
}