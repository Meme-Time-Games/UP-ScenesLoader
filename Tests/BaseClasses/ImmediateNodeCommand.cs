using CommandQueues.Core;

namespace ScenesLoaderSystem.Tests
{
    public class ImmediateNodeCommand : NodeCommand
    {
        private int _totalExecutions;

        public int TotalExecutions => _totalExecutions;

        public override void Execute()
        {
            _totalExecutions++;

            NotifyDoneExecution();
        }
    }
}
