using MVVM.Core;

namespace ScenesLoaderSystem.Core.Domain
{
    public class SceneLoadingSettings
    {
        private readonly SceneData _loadingScreenSceneData;
        private readonly SceneData _emptySceneData;
        private readonly SceneData _firstOpenSceneData;
        private readonly IEventViewModel _onAllScenesLoadedEventViewModel;
        private readonly IEventViewModel[] _onLoadingIsFinishingEventViewModels;
        private readonly float _timeBetweenLoadingFinishing;
        private readonly float _timeBeforeLoading;

        public SceneData LoadingScreenSceneData => _loadingScreenSceneData;
        public SceneData EmptySceneData => _emptySceneData;
        public SceneData FirstOpenSceneData => _firstOpenSceneData;
        public IEventViewModel OnAllScenesLoadedEventViewModel => _onAllScenesLoadedEventViewModel;
        public IEventViewModel[] OnLoadingIsFinishingEventViewModels => _onLoadingIsFinishingEventViewModels;
        public float TimeBetweenLoadingFinishing => _timeBetweenLoadingFinishing;
        public float TimeBeforeLoading => _timeBeforeLoading;

        public SceneLoadingSettings(SceneData loadingScreenSceneData, SceneData emptySceneData,
            SceneData firstOpenSceneData, IEventViewModel onAllScenesLoadedEventViewModel,
            IEventViewModel[] onLoadingIsFinishingEventViewModels, float timeBetweenLoadingFinishing,
            float timeBeforeLoading)
        {
            _loadingScreenSceneData = loadingScreenSceneData;
            _emptySceneData = emptySceneData;
            _firstOpenSceneData = firstOpenSceneData;
            _onAllScenesLoadedEventViewModel = onAllScenesLoadedEventViewModel;
            _onLoadingIsFinishingEventViewModels = onLoadingIsFinishingEventViewModels;
            _timeBetweenLoadingFinishing = timeBetweenLoadingFinishing;
            _timeBeforeLoading = timeBeforeLoading;
        }
    }
}
