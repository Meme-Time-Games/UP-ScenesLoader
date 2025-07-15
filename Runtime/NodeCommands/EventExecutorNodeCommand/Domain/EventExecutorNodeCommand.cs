using System;
using CommandQueues.Core;
using MVVM.Core;

namespace ScenesLoaderSystem
{
    public class EventExecutorNodeCommand : NodeCommand, IDisposable
    {
        private readonly IEventViewModel _onSceneLoadStartedEventViewModel;
        private readonly IEventViewModel _onSceneLoadDoneEventViewModel;

        public EventExecutorNodeCommand(IEventViewModel onSceneLoadStartedEventViewModel, IEventViewModel onSceneLoadDoneEventViewModel)
        {
            _onSceneLoadStartedEventViewModel = onSceneLoadStartedEventViewModel;
            _onSceneLoadDoneEventViewModel = onSceneLoadDoneEventViewModel;
            _onSceneLoadDoneEventViewModel.OnEventRaised += NotifyDoneExecution;
        }

        public override void Execute()
        {
            _onSceneLoadStartedEventViewModel.RaiseEvent();
            _onSceneLoadDoneEventViewModel.OnEventRaised -= NotifyDoneExecution;
        }

        public void Dispose()
        {
            _onSceneLoadDoneEventViewModel.OnEventRaised -= NotifyDoneExecution;
        }
    }
}