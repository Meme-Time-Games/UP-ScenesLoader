using System;

namespace ScenesLoaderSystem.Delays.Domain
{
    public interface IDelayProvider
    {
        void Wait(float seconds, Action onWaitDone);
        void WaitFrame(Action onFrameWaited);
        void Cancel();
    }
}
