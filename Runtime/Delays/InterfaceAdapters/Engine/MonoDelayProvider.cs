using System;
using System.Collections;
using ScenesLoaderSystem.Delays.Domain;
using UnityEngine;

namespace ScenesLoaderSystem.Delays.InterfaceAdapters
{
    public class MonoDelayProvider : MonoBehaviour, IDelayProvider
    {
        private Coroutine _runningDelay;

        public void Wait(float seconds, Action onWaitDone)
        {
            Cancel();

            _runningDelay = StartCoroutine(WaitCoroutine(seconds, onWaitDone));
        }

        private IEnumerator WaitCoroutine(float seconds, Action onWaitDone)
        {
            yield return new WaitForSecondsRealtime(seconds);

            _runningDelay = null;

            onWaitDone?.Invoke();
        }

        public void WaitFrame(Action onFrameWaited)
        {
            Cancel();

            _runningDelay = StartCoroutine(WaitFrameCoroutine(onFrameWaited));
        }

        private IEnumerator WaitFrameCoroutine(Action onFrameWaited)
        {
            yield return null;

            _runningDelay = null;

            onFrameWaited?.Invoke();
        }

        public void Cancel()
        {
            bool isDestroyed = this == null;
            if (isDestroyed)
                return;

            if (ReferenceEquals(_runningDelay, null))
                return;

            StopCoroutine(_runningDelay);

            _runningDelay = null;
        }
    }
}
