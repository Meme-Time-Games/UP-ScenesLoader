using ScenesLoaderSystem.Core.InterfaceAdapters;
using UnityEngine;

namespace ScenesLoaderSystem.Tests
{
    public class SceneDataSOBuilder
    {
        private string _sceneName = "Scene";
        private SceneDataSO[] _scenesDataToOpen = new SceneDataSO[0];
        private SceneDataSO[] _scenesDataToRemove = new SceneDataSO[0];

        public SceneDataSOBuilder WithSceneName(string sceneName)
        {
            _sceneName = sceneName;
            return this;
        }

        public SceneDataSOBuilder WithScenesToOpen(params SceneDataSO[] scenesDataToOpen)
        {
            _scenesDataToOpen = scenesDataToOpen;
            return this;
        }

        public SceneDataSO Build()
        {
            SceneDataSO sceneDataSO = ScriptableObject.CreateInstance<SceneDataSO>();

            PrivateFieldWriter.WriteWithName(sceneDataSO, "_sceneName", _sceneName);
            PrivateFieldWriter.WriteWithName(sceneDataSO, "_scenesDataToOpen", _scenesDataToOpen);
            PrivateFieldWriter.WriteWithName(sceneDataSO, "_scenesDataToRemove", _scenesDataToRemove);

            return sceneDataSO;
        }
    }
}
