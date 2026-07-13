using System;

namespace ScenesLoaderSystem.Core.Domain
{
    public interface ISceneOperations
    {
        void LoadSceneWithName(string sceneName, Action onSceneLoaded);
        void UnloadSceneWithName(string sceneName, Action onSceneUnloaded);
        void SetActiveSceneWithName(string sceneName);
        bool IsSceneLoadedWithName(string sceneName);
    }
}
