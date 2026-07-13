using ScenesLoaderSystem.Delays.Domain;

namespace ScenesLoaderSystem
{
    public class LoadSceneTimer
    {
        private readonly ISceneDataLoader _sceneDataLoader;
        private readonly IDelayProvider _delayProvider;

        public LoadSceneTimer(ISceneDataLoader sceneDataLoader, IDelayProvider delayProvider)
        {
            _sceneDataLoader = sceneDataLoader;
            _delayProvider = delayProvider;
        }

        public void StartTimerWithDuration(float duration)
        {
            _delayProvider.Wait(duration, LoadScene);
        }

        private void LoadScene()
        {
            _sceneDataLoader.Load();
        }
    }
}
