using ScenesLoaderSystem.Core.Domain;
using ServiceLocatorPattern;
using UnityEngine;

namespace ScenesLoaderSystem.Core.Installers
{
    public class SceneLoaderDisposer : MonoBehaviour
    {
        private void OnDestroy()
        {
            if (!ServiceLocatorInstance.Instance.IsContained<ISceneLoader>())
                return;

            ServiceLocatorInstance.Instance.Remove<ISceneLoader>();
        }
    }
}
