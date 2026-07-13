using System.Collections.Generic;

namespace ScenesLoaderSystem.Core.Domain
{
    public class SceneData
    {
        private readonly string _sceneName;
        private readonly bool _hasToUseLoadingScreen; 
        private readonly bool _isLockedScene;
        private readonly bool _hasToRemoveLockedScenes;
        private readonly bool _isPrincipal;
        private readonly bool _hasToCloseOthersScenes;
        private readonly bool _hasToKeepOpen;
        private readonly bool _hasToKeepLoadingOpen;
        private readonly SceneData _overrideLoadingSceneData;

        private readonly SceneData[] _scenesDataToOpen;
        private readonly SceneData[] _scenesDataToRemove;

        public string SceneName => _sceneName;
        public bool HasToUseLoadingScreen => _hasToUseLoadingScreen;
        public bool IsLockedScene => _isLockedScene;
        public bool HasToRemoveLockedScenes => _hasToRemoveLockedScenes;
        public bool IsPrincipal => _isPrincipal;
        public bool HasToCloseOthersScenes => _hasToCloseOthersScenes;
        public bool HasToKeepOpen => _hasToKeepOpen;
        public bool HasToKeepLoadingOpen => _hasToKeepLoadingOpen;
        public SceneData OverrideLoadingSceneData => _overrideLoadingSceneData;

        public SceneData(string sceneName, bool hasToUseLoadingScreen, bool isLockedScene, bool hasToRemoveLockedScenes, bool isPrincipal, bool hasToCloseOthersScenes, bool hasToKeepOpen, SceneData[] scenesDataToOpen, SceneData[] scenesDataToRemove, bool hasToKeepLoadingOpen, SceneData overrideLoadingSceneData)
        {
            _sceneName = sceneName;
            _hasToUseLoadingScreen = hasToUseLoadingScreen;
            _isLockedScene = isLockedScene;
            _hasToRemoveLockedScenes = hasToRemoveLockedScenes;
            _isPrincipal = isPrincipal;
            _hasToCloseOthersScenes = hasToCloseOthersScenes;
            _hasToKeepOpen = hasToKeepOpen;
            _scenesDataToOpen = scenesDataToOpen;
            _scenesDataToRemove = scenesDataToRemove;
            _hasToKeepLoadingOpen = hasToKeepLoadingOpen;
            _overrideLoadingSceneData = overrideLoadingSceneData;
        }

        public SceneData[] GetAllScenesToOpen()
        {
            if (ReferenceEquals(_scenesDataToOpen, null))
                return new[] { this };

            List<SceneData> scenesToOpen = new List<SceneData>();

            foreach (var sceneData in _scenesDataToOpen)
            {
                SceneData[] scenesIntoSceneData = sceneData.GetAllScenesToOpen();

                foreach (var sceneDataInto in scenesIntoSceneData)
                {
                    if (scenesToOpen.Contains(sceneDataInto))
                        continue;

                    scenesToOpen.Add(sceneDataInto);
                }
            }

            scenesToOpen.Add(this);

            return scenesToOpen.ToArray();
        }

        public SceneData[] GetAllScenesDataToRemove()
        {
            if (ReferenceEquals(_scenesDataToRemove, null))
                return new SceneData[0];

            return (SceneData[])_scenesDataToRemove.Clone();
        }
    }
}