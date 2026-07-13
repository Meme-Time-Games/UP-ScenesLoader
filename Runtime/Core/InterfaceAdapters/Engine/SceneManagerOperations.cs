using System;
using ScenesLoaderSystem.Core.Domain;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ScenesLoaderSystem.Core.InterfaceAdapters
{
    public class SceneManagerOperations : ISceneOperations
    {
        public void LoadSceneWithName(string sceneName, Action onSceneLoaded)
        {
            AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            if (ReferenceEquals(loadSceneOperation, null))
                throw new Exception($"SceneLoader Error: The Scene {sceneName} is not in the Build Settings.");

            loadSceneOperation.completed += loadedOperation => onSceneLoaded?.Invoke();
        }

        public void UnloadSceneWithName(string sceneName, Action onSceneUnloaded)
        {
            AsyncOperation unloadSceneOperation = SceneManager.UnloadSceneAsync(sceneName);

            if (ReferenceEquals(unloadSceneOperation, null))
            {
                onSceneUnloaded?.Invoke();
                return;
            }

            unloadSceneOperation.completed += unloadedOperation => onSceneUnloaded?.Invoke();
        }

        public void SetActiveSceneWithName(string sceneName)
        {
            Scene sceneToActivate = SceneManager.GetSceneByName(sceneName);

            SceneManager.SetActiveScene(sceneToActivate);
        }

        public bool IsSceneLoadedWithName(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);

            return scene.isLoaded;
        }
    }
}
