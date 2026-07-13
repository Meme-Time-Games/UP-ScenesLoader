using System;
using System.Collections.Generic;
using ScenesLoaderSystem.Delays.Domain;

namespace ScenesLoaderSystem.Tests
{
    public class ManualDelayProvider : IDelayProvider
    {
        private readonly List<float> _requestedSeconds = new List<float>();
        private readonly Queue<Action> _pendingDelays = new Queue<Action>();
        private int _totalCancellations;

        public IReadOnlyList<float> RequestedSeconds => _requestedSeconds;
        public int TotalPendingDelays => _pendingDelays.Count;
        public int TotalCancellations => _totalCancellations;

        public void Wait(float seconds, Action onWaitDone)
        {
            _requestedSeconds.Add(seconds);
            _pendingDelays.Enqueue(onWaitDone);
        }

        public void WaitFrame(Action onFrameWaited)
        {
            _pendingDelays.Enqueue(onFrameWaited);
        }

        public void Cancel()
        {
            _totalCancellations++;
            _pendingDelays.Clear();
        }

        public void CompleteNextDelay()
        {
            _pendingDelays.Dequeue().Invoke();
        }
    }
}
