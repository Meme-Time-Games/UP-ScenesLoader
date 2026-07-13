using System;
using System.Collections.Generic;
using ScenesLoaderSystem.Delays.Domain;

namespace ScenesLoaderSystem.Tests
{
    public class ImmediateDelayProvider : IDelayProvider
    {
        private readonly List<float> _requestedSeconds = new List<float>();

        public IReadOnlyList<float> RequestedSeconds => _requestedSeconds;

        public void Wait(float seconds, Action onWaitDone)
        {
            _requestedSeconds.Add(seconds);

            onWaitDone?.Invoke();
        }

        public void WaitFrame(Action onFrameWaited)
        {
            onFrameWaited?.Invoke();
        }

        public void Cancel()
        {
        }
    }
}
