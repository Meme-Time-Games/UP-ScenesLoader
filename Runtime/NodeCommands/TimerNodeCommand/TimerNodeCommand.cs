using System;
using System.Threading.Tasks;
using CommandQueues.Core;

namespace ScenesLoaderSystem
{
    public class TimerNodeCommand : NodeCommand
    {
        private readonly float _secondsToWait;

        public TimerNodeCommand(float secondsToWait)
        {
            _secondsToWait = secondsToWait;
        }

        public override void Execute()
        {
            WaitTime();
        }

        private async void WaitTime()
        {
            await Task.Delay(TimeSpan.FromSeconds(_secondsToWait));
            NotifyDoneExecution();
        }
    }
}