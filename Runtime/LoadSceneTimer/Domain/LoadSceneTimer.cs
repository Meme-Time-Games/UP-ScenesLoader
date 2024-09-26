using System;
using System.Threading.Tasks;

namespace ScenesLoaderSystem
{
    public class LoadSceneTimer
    {
        private readonly ISceneDataLoader _sceneDataLoader;
        private readonly int _duration;

        public LoadSceneTimer(int duration, ISceneDataLoader sceneDataLoader)
        {
            _duration = duration;
            _sceneDataLoader = sceneDataLoader;

            Execute();
        }

        private async void Execute()
        {
            await Task.Delay(TimeSpan.FromSeconds(_duration));

            _sceneDataLoader.Load();
        }
    }
}