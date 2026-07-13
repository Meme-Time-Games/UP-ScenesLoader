using NUnit.Framework;

namespace ScenesLoaderSystem.Tests
{
    public class TimerNodeCommandTests
    {
        private ManualDelayProvider _delayProvider;
        private FakeCommandQueue _commandQueue;
        private TimerNodeCommand _timerNodeCommand;

        [SetUp]
        public void SetUp()
        {
            _delayProvider = new ManualDelayProvider();
            _commandQueue = new FakeCommandQueue();
            _timerNodeCommand = new TimerNodeCommand(2f, _delayProvider);
            _timerNodeCommand.SetCommandQueue(_commandQueue);
        }

        [Test]
        public void Execute_WhenTheDelayIsDone_NotifiesTheCommandQueue()
        {
            _timerNodeCommand.Execute();

            _delayProvider.CompleteNextDelay();

            Assert.AreEqual(1, _commandQueue.TotalCommandsDone);
        }

        [Test]
        public void Execute_WhenTheDelayIsNotDone_DoesNotNotifyTheCommandQueue()
        {
            _timerNodeCommand.Execute();

            Assert.AreEqual(0, _commandQueue.TotalCommandsDone);
        }

        [Test]
        public void Execute_WhenItIsExecuted_WaitsTheConfiguredSeconds()
        {
            _timerNodeCommand.Execute();

            Assert.AreEqual(new[] { 2f }, _delayProvider.RequestedSeconds);
        }

        [Test]
        public void Dispose_WhenTheDelayIsNotDone_CancelsTheDelay()
        {
            _timerNodeCommand.Execute();

            _timerNodeCommand.Dispose();

            Assert.AreEqual(0, _delayProvider.TotalPendingDelays);
        }
    }
}
