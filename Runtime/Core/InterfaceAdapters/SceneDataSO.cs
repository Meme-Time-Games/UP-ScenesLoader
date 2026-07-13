using System;
using ScenesLoaderSystem.Core.Domain;
using UnityEngine;

namespace ScenesLoaderSystem.Core.InterfaceAdapters
{
    [CreateAssetMenu(fileName = "SceneData", menuName = "ScriptableObjects/ScenesLoader/Data/SceneData")]
    public class SceneDataSO : ScriptableObject
    {
        [Header("Config")]
        [SerializeField] private string _sceneName;
        [SerializeField] private bool _hasToUseLoadingScreen = true; 
        [SerializeField] private bool _isLockedScene;
        [SerializeField] private bool _hasToRemoveLockedScenes;
        [SerializeField] private bool _isPrincipal = true;
        [SerializeField] private bool _hasToCloseOthersScenes = true;
        [SerializeField] private bool _hasToKeepOpen;
        [SerializeField] private bool _hasToKeepLoadingOpen;
        
        [Header("References")]
        [SerializeField] private SceneDataSO[] _scenesDataToOpen;
        [SerializeField] private SceneDataSO[] _scenesDataToRemove;
        [SerializeField] private SceneDataSO _overrideLoadingSceneDataSO;
        
        private SceneData _currentSceneData;
        private bool _isBuildingSceneData;

        public SceneData GetSceneData()
        {
            if (!ReferenceEquals(_currentSceneData, null))
                return _currentSceneData;

            if (_isBuildingSceneData)
                throw new Exception($"SceneDataSO Error: The SceneDataSO {name} is opened by a SceneDataSO that it opens.");

            _isBuildingSceneData = true;

            try
            {
                _currentSceneData = BuildSceneData();
            }
            finally
            {
                _isBuildingSceneData = false;
            }

            return _currentSceneData;
        }

        private SceneData BuildSceneData()
        {
            SceneData[] sceneDatasToOpen = GetSceneData(_scenesDataToOpen);
            SceneData[] sceneDatasToRemove = GetSceneData(_scenesDataToRemove);

            return new SceneData(_sceneName, _hasToUseLoadingScreen, _isLockedScene,
                _hasToRemoveLockedScenes, _isPrincipal, _hasToCloseOthersScenes, _hasToKeepOpen,
                sceneDatasToOpen, sceneDatasToRemove, _hasToKeepLoadingOpen,
                _overrideLoadingSceneDataSO?.GetSceneData());
        }

        private SceneData[] GetSceneData(SceneDataSO[] sceneDataSo)
        {
            int totalScenesData = sceneDataSo.Length;
            SceneData[] scenesData = new SceneData[totalScenesData];

            for (int i = 0; i < totalScenesData; i++)
            {
                scenesData[i] = sceneDataSo[i].GetSceneData();
            }

            return scenesData;
        }
    }
}