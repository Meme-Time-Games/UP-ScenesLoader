using System;
using CommandQueues.Core;

namespace ScenesLoaderSystem.Tests
{
    public class FakeCommandQueue : ICommandQueue
    {
        private int _totalCommandsDone;
        private int _totalExecutions;

        public Action OnExecutionDone { get; set; }

        public int TotalCommandsDone => _totalCommandsDone;
        public int TotalExecutions => _totalExecutions;

        public void Execute()
        {
            _totalExecutions++;
        }

        public void NotifyCommandDone()
        {
            _totalCommandsDone++;
        }

        public void SetCommandQueue(ICommandQueue commandQueue)
        {
        }
    }
}
