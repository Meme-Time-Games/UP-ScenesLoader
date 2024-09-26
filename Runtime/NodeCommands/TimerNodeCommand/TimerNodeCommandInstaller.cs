using CommandQueues.Core;
using UnityEngine;

namespace ScenesLoaderSystem
{
    public class TimerNodeCommandInstaller : NodeCommandInstaller
    {
        [Header("Config")]
        [SerializeField] private float _secondsToWait;
        
        protected override INodeCommand GetData()
        {
            return new TimerNodeCommand(_secondsToWait);
        }
    }
}