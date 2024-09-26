// using System.Threading.Tasks;
// using CommandQueues.Core;
// using Installers.Core;
// using ServiceLocatorPattern;
// using UnityEngine;
//
// namespace ScenesLoaderSystem.Core.Domain
// {
//     public class MonoSceneLoadedNotifier : MonoBehaviour
//     {
//         [Header("References")]
//         [SerializeField] private MonoInstaller<ICommandQueue> _commandQueueInstaller;
//
//         private async void Start()
//         {
//             await Task.Yield();
//             ServiceLocatorInstance.Instance.Get<ISceneLoader>().SetNodeCommandOfALoadedScene(_commandQueueInstaller.Data);
//         }
//     }
// }