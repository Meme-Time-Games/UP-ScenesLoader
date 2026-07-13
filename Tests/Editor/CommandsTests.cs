using MVVM.Core;
using NUnit.Framework;
using ScenesLoaderSystem.Core.Domain;

namespace ScenesLoaderSystem.Tests
{
    public class CommandsTests
    {
        private FakeSceneLoader _sceneLoader;
        private SceneData _sceneData;

        [SetUp]
        public void SetUp()
        {
            _sceneLoader = new FakeSceneLoader();
            _sceneData = new SceneDataBuilder().WithSceneName("Game").Build();
        }

        [Test]
        public void Execute_TheSceneLoaderCommandIsExecuted_LoadsTheSceneDataOfTheLoader()
        {
            SceneDataLoader sceneDataLoader = new SceneDataLoader(_sceneLoader, _sceneData);
            SceneLoaderCommand sceneLoaderCommand = new SceneLoaderCommand(sceneDataLoader);

            sceneLoaderCommand.Execute();

            Assert.AreEqual(new[] { _sceneData }, _sceneLoader.LoadedScenesData);
        }

        [Test]
        public void Execute_TheRemoveSceneCommandIsExecuted_RemovesTheSceneData()
        {
            RemoveSceneCommand removeSceneCommand = new RemoveSceneCommand(_sceneLoader, _sceneData);

            removeSceneCommand.Execute();

            Assert.AreEqual(new[] { _sceneData }, _sceneLoader.RemovedScenesData);
        }

        [Test]
        public void Load_TheRemoveCurrentAndSetPrincipalSceneIsLoaded_RemovesTheCurrentScene()
        {
            RemoveCurrentAndSetPrincipalScene removeCurrentAndSetPrincipalScene =
                new RemoveCurrentAndSetPrincipalScene(_sceneLoader, _sceneData);

            removeCurrentAndSetPrincipalScene.Load();

            Assert.AreEqual(new[] { _sceneData }, _sceneLoader.RemovedScenesData);
        }

        [Test]
        public void Load_TheSceneDataLoaderLoads_LoadsItsSceneData()
        {
            SceneDataLoader sceneDataLoader = new SceneDataLoader(_sceneLoader, _sceneData);

            sceneDataLoader.Load();

            Assert.AreEqual(new[] { _sceneData }, _sceneLoader.LoadedScenesData);
        }

        [Test]
        public void IsThisSceneDataOpened_TheSceneDataIsNotOpened_ReturnsFalse()
        {
            SceneDataLoader sceneDataLoader = new SceneDataLoader(_sceneLoader, _sceneData);

            Assert.IsFalse(sceneDataLoader.IsThisSceneDataOpened());
        }

        [Test]
        public void IsThisSceneDataOpened_TheSceneDataIsOpened_ReturnsTrue()
        {
            SceneDataLoader sceneDataLoader = new SceneDataLoader(_sceneLoader, _sceneData);
            sceneDataLoader.Load();

            Assert.IsTrue(sceneDataLoader.IsThisSceneDataOpened());
        }

        [Test]
        public void Execute_TheEventExecutorNodeCommandIsExecuted_RaisesTheSceneLoadStartedEvent()
        {
            int totalEventsRaised = 0;
            EventViewModel onSceneLoadStarted = new EventViewModel();
            EventViewModel onSceneLoadDone = new EventViewModel();
            onSceneLoadStarted.OnEventRaised += () => totalEventsRaised++;
            EventExecutorNodeCommand eventExecutorNodeCommand =
                new EventExecutorNodeCommand(onSceneLoadStarted, onSceneLoadDone);

            eventExecutorNodeCommand.Execute();

            Assert.AreEqual(1, totalEventsRaised);
        }

        [Test]
        public void RaiseEvent_TheSceneLoadDoneEventIsRaised_NotifiesTheCommandQueue()
        {
            FakeCommandQueue commandQueue = new FakeCommandQueue();
            EventViewModel onSceneLoadStarted = new EventViewModel();
            EventViewModel onSceneLoadDone = new EventViewModel();
            EventExecutorNodeCommand eventExecutorNodeCommand =
                new EventExecutorNodeCommand(onSceneLoadStarted, onSceneLoadDone);
            eventExecutorNodeCommand.SetCommandQueue(commandQueue);

            onSceneLoadDone.RaiseEvent();

            Assert.AreEqual(1, commandQueue.TotalCommandsDone);
        }

        [Test]
        public void Dispose_TheSceneLoadDoneEventIsRaisedAfterDisposing_DoesNotNotifyTheCommandQueue()
        {
            FakeCommandQueue commandQueue = new FakeCommandQueue();
            EventViewModel onSceneLoadStarted = new EventViewModel();
            EventViewModel onSceneLoadDone = new EventViewModel();
            EventExecutorNodeCommand eventExecutorNodeCommand =
                new EventExecutorNodeCommand(onSceneLoadStarted, onSceneLoadDone);
            eventExecutorNodeCommand.SetCommandQueue(commandQueue);

            eventExecutorNodeCommand.Dispose();
            onSceneLoadDone.RaiseEvent();

            Assert.AreEqual(0, commandQueue.TotalCommandsDone);
        }
    }
}
