using CommandQueues.Core;
using ScenesLoaderSystem.Delays.InterfaceAdapters;
using UnityEngine;

namespace ScenesLoaderSystem
{
    public class TimerNodeCommandInstaller : NodeCommandInstaller
    {
        [Header("Config")]
        [SerializeField] private float _secondsToWait;

        protected override INodeCommand GetData()
        {
            MonoDelayProvider delayProvider = gameObject.AddComponent<MonoDelayProvider>();

            return new TimerNodeCommand(_secondsToWait, delayProvider);
        }
    }
}
