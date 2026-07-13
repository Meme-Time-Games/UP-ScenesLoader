using System;
using CommandQueues.Core;
using ScenesLoaderSystem.Delays.Domain;

namespace ScenesLoaderSystem
{
    public class TimerNodeCommand : NodeCommand, IDisposable
    {
        private readonly float _secondsToWait;
        private readonly IDelayProvider _delayProvider;

        public TimerNodeCommand(float secondsToWait, IDelayProvider delayProvider)
        {
            _secondsToWait = secondsToWait;
            _delayProvider = delayProvider;
        }

        public override void Execute()
        {
            _delayProvider.Wait(_secondsToWait, NotifyDoneExecution);
        }

        public void Dispose()
        {
            _delayProvider.Cancel();
        }
    }
}
