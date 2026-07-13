using ScenesLoaderSystem.Core.Domain;

namespace ScenesLoaderSystem.Tests
{
    public class SceneDataBuilder
    {
        private string _sceneName = "Scene";
        private bool _hasToUseLoadingScreen;
        private bool _isLockedScene;
        private bool _hasToRemoveLockedScenes;
        private bool _isPrincipal;
        private bool _hasToCloseOthersScenes;
        private bool _hasToKeepOpen;
        private bool _hasToKeepLoadingOpen;
        private SceneData _overrideLoadingSceneData;
        private SceneData[] _scenesDataToOpen = new SceneData[0];
        private SceneData[] _scenesDataToRemove = new SceneData[0];

        public SceneDataBuilder WithSceneName(string sceneName)
        {
            _sceneName = sceneName;
            return this;
        }

        public SceneDataBuilder WithLoadingScreen()
        {
            _hasToUseLoadingScreen = true;
            return this;
        }

        public SceneDataBuilder KeepingLoadingOpen()
        {
            _hasToKeepLoadingOpen = true;
            return this;
        }

        public SceneDataBuilder WithOverrideLoadingScene(SceneData overrideLoadingSceneData)
        {
            _overrideLoadingSceneData = overrideLoadingSceneData;
            return this;
        }

        public SceneDataBuilder AsLockedScene()
        {
            _isLockedScene = true;
            return this;
        }

        public SceneDataBuilder RemovingLockedScenes()
        {
            _hasToRemoveLockedScenes = true;
            return this;
        }

        public SceneDataBuilder AsPrincipalScene()
        {
            _isPrincipal = true;
            return this;
        }

        public SceneDataBuilder ClosingOtherScenes()
        {
            _hasToCloseOthersScenes = true;
            return this;
        }

        public SceneDataBuilder KeepingOpen()
        {
            _hasToKeepOpen = true;
            return this;
        }

        public SceneDataBuilder WithScenesToOpen(params SceneData[] scenesDataToOpen)
        {
            _scenesDataToOpen = scenesDataToOpen;
            return this;
        }

        public SceneDataBuilder WithScenesToRemove(params SceneData[] scenesDataToRemove)
        {
            _scenesDataToRemove = scenesDataToRemove;
            return this;
        }

        public SceneDataBuilder WithNullScenesToOpen()
        {
            _scenesDataToOpen = null;
            return this;
        }

        public SceneDataBuilder WithNullScenesToRemove()
        {
            _scenesDataToRemove = null;
            return this;
        }

        public SceneData Build()
        {
            return new SceneData(_sceneName, _hasToUseLoadingScreen, _isLockedScene, _hasToRemoveLockedScenes,
                _isPrincipal, _hasToCloseOthersScenes, _hasToKeepOpen, _scenesDataToOpen, _scenesDataToRemove,
                _hasToKeepLoadingOpen, _overrideLoadingSceneData);
        }
    }
}
