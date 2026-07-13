using ScenesLoaderSystem.Core.Domain;
using ServiceLocatorPattern;
using UnityEngine;

namespace ScenesLoaderSystem.Core.Installers
{
    public class SceneLoaderDisposer : MonoBehaviour
    {
        private ISceneLoader _sceneLoader;

        public void SetSceneLoader(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        private void OnDestroy()
        {
            if (!ServiceLocatorInstance.Instance.IsContained<ISceneLoader>())
                return;

            if (!ReferenceEquals(ServiceLocatorInstance.Instance.Get<ISceneLoader>(), _sceneLoader))
                return;

            ServiceLocatorInstance.Instance.Remove<ISceneLoader>();
        }
    }
}
