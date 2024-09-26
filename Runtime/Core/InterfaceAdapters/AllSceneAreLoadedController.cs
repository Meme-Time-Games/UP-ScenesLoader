using System;
using ScenesLoaderSystem.Core.Domain;

namespace ScenesLoaderSystem.OnAllSceneAreLoaded.Domain
{
    public abstract class AllSceneAreLoadedController : IDisposable
    {
        private readonly ISceneLoader _sceneLoader;

        protected AllSceneAreLoadedController(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
            
            _sceneLoader.OnAllScenesAreLoaded += AllSceneAreLoaded;
        }

        protected abstract void AllSceneAreLoaded();

        public void Dispose()
        {
            _sceneLoader.OnAllScenesAreLoaded -= AllSceneAreLoaded;
        }
    }
}