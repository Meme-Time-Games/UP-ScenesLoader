using System;
using MVVM.Core;
using NUnit.Framework;
using ScenesLoaderSystem.Core.Domain;

namespace ScenesLoaderSystem.Tests
{
    public class SceneLoaderTests
    {
        private const string LoadingScreenSceneName = "LoadingScreen";
        private const string EmptySceneName = "Empty";
        private const string FirstSceneName = "First";

        private FakeSceneOperations _sceneOperations;
        private ImmediateDelayProvider _delayProvider;
        private EventViewModel _onAllScenesLoadedEventViewModel;
        private EventViewModel _onLoadingIsFinishingEventViewModel;
        private SceneData _firstSceneData;
        private SceneLoader _sceneLoader;

        [SetUp]
        public void SetUp()
        {
            _sceneOperations = new FakeSceneOperations();
            _delayProvider = new ImmediateDelayProvider();
            _onAllScenesLoadedEventViewModel = new EventViewModel();
            _onLoadingIsFinishingEventViewModel = new EventViewModel();

            _firstSceneData = new SceneDataBuilder().WithSceneName(FirstSceneName).Build();

            _sceneLoader = new SceneLoader(_sceneOperations, _delayProvider, CreateSettings());
        }

        private SceneLoadingSettings CreateSettings()
        {
            SceneData loadingScreenSceneData = new SceneDataBuilder().WithSceneName(LoadingScreenSceneName).Build();
            SceneData emptySceneData = new SceneDataBuilder().WithSceneName(EmptySceneName).Build();

            return new SceneLoadingSettings(loadingScreenSceneData, emptySceneData, _firstSceneData,
                _onAllScenesLoadedEventViewModel, new IEventViewModel[] { _onLoadingIsFinishingEventViewModel },
                0.5f, 0.25f);
        }

        [Test]
        public void LoadScene_TheSceneNameIsEmpty_ThrowsAnException()
        {
            SceneData sceneData = new SceneDataBuilder().WithSceneName(string.Empty).Build();

            Assert.Throws<Exception>(() => _sceneLoader.LoadScene(sceneData));
        }

        [Test]
        public void LoadScene_TheSceneUsesALoadingScreen_LoadsTheLoadingScreenScene()
        {
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").WithLoadingScreen().Build();

            _sceneLoader.LoadScene(sceneData);

            Assert.AreEqual(1, _sceneOperations.GetTotalLoadsOfScene(LoadingScreenSceneName));
        }

        [Test]
        public void LoadScene_TheSceneDoesNotUseALoadingScreen_LoadsTheEmptyScene()
        {
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").Build();

            _sceneLoader.LoadScene(sceneData);

            Assert.AreEqual(1, _sceneOperations.GetTotalLoadsOfScene(EmptySceneName));
        }

        [Test]
        public void LoadScene_TheSceneOverridesTheLoadingScene_LoadsTheOverrideLoadingScene()
        {
            SceneData overrideLoadingSceneData = new SceneDataBuilder().WithSceneName("OverrideLoading").Build();
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").WithLoadingScreen()
                .WithOverrideLoadingScene(overrideLoadingSceneData).Build();

            _sceneLoader.LoadScene(sceneData);

            Assert.AreEqual(1, _sceneOperations.GetTotalLoadsOfScene("OverrideLoading"));
        }

        [Test]
        public void LoadScene_TheSceneIsLoaded_TheSceneIsOpened()
        {
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").Build();

            _sceneLoader.LoadScene(sceneData);

            Assert.IsTrue(_sceneLoader.IsThisSceneDataOpened(sceneData));
        }

        [Test]
        public void LoadScene_AnotherLoadIsRunning_IgnoresTheSecondRequest()
        {
            _sceneOperations.SetManualCompletion();
            SceneData firstSceneToLoad = new SceneDataBuilder().WithSceneName("Game").Build();
            SceneData secondSceneToLoad = new SceneDataBuilder().WithSceneName("Menu").Build();

            _sceneLoader.LoadScene(firstSceneToLoad);
            _sceneLoader.LoadScene(secondSceneToLoad);

            Assert.AreEqual(0, _sceneOperations.GetTotalLoadsOfScene("Menu"));
        }

        [Test]
        public void LoadScene_TheSceneClosesTheOtherScenes_UnloadsThePreviousScene()
        {
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").ClosingOtherScenes().Build();

            _sceneLoader.LoadScene(sceneData);

            Assert.Contains(FirstSceneName, (System.Collections.ICollection)_sceneOperations.UnloadedScenes);
        }

        [Test]
        public void LoadScene_TheSceneClosesTheOtherScenesAndThePreviousSceneIsLocked_KeepsThePreviousSceneOpen()
        {
            SceneData lockedSceneData = new SceneDataBuilder().WithSceneName("Locked").AsLockedScene().Build();
            _sceneLoader.LoadScene(lockedSceneData);
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").ClosingOtherScenes().Build();

            _sceneLoader.LoadScene(sceneData);

            Assert.IsTrue(_sceneLoader.IsThisSceneDataOpened(lockedSceneData));
        }

        [Test]
        public void LoadScene_TheSceneRemovesTheLockedScenes_UnloadsThePreviousLockedScene()
        {
            SceneData lockedSceneData = new SceneDataBuilder().WithSceneName("Locked").AsLockedScene().Build();
            _sceneLoader.LoadScene(lockedSceneData);
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").ClosingOtherScenes()
                .RemovingLockedScenes().Build();

            _sceneLoader.LoadScene(sceneData);

            Assert.IsFalse(_sceneLoader.IsThisSceneDataOpened(lockedSceneData));
        }

        [Test]
        public void LoadScene_ThePreviousSceneHasToKeepOpen_KeepsThePreviousSceneOpen()
        {
            SceneData keptOpenSceneData = new SceneDataBuilder().WithSceneName("Kept").KeepingOpen().Build();
            _sceneLoader.LoadScene(keptOpenSceneData);
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").ClosingOtherScenes().Build();

            _sceneLoader.LoadScene(sceneData);

            Assert.IsTrue(_sceneLoader.IsThisSceneDataOpened(keptOpenSceneData));
        }

        [Test]
        public void LoadSceneKeepingOpenScenes_TheSceneClosesTheOtherScenes_KeepsThePreviousSceneOpen()
        {
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").ClosingOtherScenes().Build();

            _sceneLoader.LoadSceneKeepingOpenScenes(sceneData);

            Assert.IsTrue(_sceneLoader.IsThisSceneDataOpened(_firstSceneData));
        }

        [Test]
        public void LoadScene_TheSceneDataHasScenesToRemove_UnloadsThoseScenes()
        {
            SceneData sceneToRemoveData = new SceneDataBuilder().WithSceneName("ToRemove").Build();
            _sceneLoader.LoadScene(sceneToRemoveData);
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game")
                .WithScenesToRemove(sceneToRemoveData).Build();

            _sceneLoader.LoadScene(sceneData);

            Assert.IsFalse(_sceneLoader.IsThisSceneDataOpened(sceneToRemoveData));
        }

        [Test]
        public void LoadScene_TheScenesDataToRemoveIsNull_LoadsTheScene()
        {
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").WithNullScenesToRemove().Build();

            _sceneLoader.LoadScene(sceneData);

            Assert.AreEqual(1, _sceneOperations.GetTotalLoadsOfScene("Game"));
        }

        [Test]
        public void LoadScene_TheSceneIsAlreadyOpened_DoesNotLoadItAgain()
        {
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").Build();
            _sceneLoader.LoadScene(sceneData);

            _sceneLoader.LoadScene(sceneData);

            Assert.AreEqual(1, _sceneOperations.GetTotalLoadsOfScene("Game"));
        }

        [Test]
        public void LoadScene_TheSceneDataHasScenesToOpen_LoadsTheChildSceneBeforeTheScene()
        {
            SceneData childSceneData = new SceneDataBuilder().WithSceneName("Child").Build();
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game")
                .WithScenesToOpen(childSceneData).Build();

            _sceneLoader.LoadScene(sceneData);

            Assert.AreEqual(new[] { EmptySceneName, "Child", "Game" }, _sceneOperations.LoadedScenes);
        }

        [Test]
        public void LoadScene_AllTheScenesAreLoaded_UnloadsTheTransitionScene()
        {
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").WithLoadingScreen().Build();

            _sceneLoader.LoadScene(sceneData);

            Assert.Contains(LoadingScreenSceneName, (System.Collections.ICollection)_sceneOperations.UnloadedScenes);
        }

        [Test]
        public void LoadScene_AllTheScenesAreLoaded_SetsThePrincipalSceneAsTheActiveScene()
        {
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").AsPrincipalScene().Build();

            _sceneLoader.LoadScene(sceneData);

            Assert.AreEqual(new[] { "Game" }, _sceneOperations.ActivatedScenes);
        }

        [Test]
        public void LoadScene_AllTheScenesAreLoaded_RaisesTheAllScenesLoadedEvent()
        {
            int totalEventsRaised = 0;
            _onAllScenesLoadedEventViewModel.OnEventRaised += () => totalEventsRaised++;
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").Build();

            _sceneLoader.LoadScene(sceneData);

            Assert.AreEqual(1, totalEventsRaised);
        }

        [Test]
        public void LoadScene_TheLoadingIsFinishing_RaisesTheLoadingIsFinishingEvents()
        {
            int totalEventsRaised = 0;
            _onLoadingIsFinishingEventViewModel.OnEventRaised += () => totalEventsRaised++;
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").WithLoadingScreen().Build();

            _sceneLoader.LoadScene(sceneData);

            Assert.AreEqual(1, totalEventsRaised);
        }

        [Test]
        public void LoadScene_TheSceneKeepsTheLoadingOpen_DoesNotUnloadTheLoadingScreen()
        {
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").WithLoadingScreen()
                .KeepingLoadingOpen().Build();

            _sceneLoader.LoadScene(sceneData);

            Assert.IsEmpty(_sceneOperations.UnloadedScenes);
        }

        [Test]
        public void LoadScene_TheSceneKeepsTheLoadingOpen_RaisesTheAllScenesLoadedEvent()
        {
            int totalEventsRaised = 0;
            _onAllScenesLoadedEventViewModel.OnEventRaised += () => totalEventsRaised++;
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").WithLoadingScreen()
                .KeepingLoadingOpen().Build();

            _sceneLoader.LoadScene(sceneData);

            Assert.AreEqual(1, totalEventsRaised);
        }

        [Test]
        public void LoadScene_ThePreviousSceneKeptTheLoadingOpen_DoesNotLoadTheLoadingScreenTwice()
        {
            SceneData keepingLoadingSceneData = new SceneDataBuilder().WithSceneName("Keeping").WithLoadingScreen()
                .KeepingLoadingOpen().Build();
            _sceneLoader.LoadScene(keepingLoadingSceneData);
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").WithLoadingScreen().Build();

            _sceneLoader.LoadScene(sceneData);

            Assert.AreEqual(1, _sceneOperations.GetTotalLoadsOfScene(LoadingScreenSceneName));
        }

        [Test]
        public void SetNodeCommandOfALoadedScene_NoSceneWasLoadedYet_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _sceneLoader.SetNodeCommandOfALoadedScene(new ImmediateNodeCommand()));
        }

        [Test]
        public void LoadScene_TheSceneRegistersANodeCommand_ExecutesTheNodeCommand()
        {
            _sceneOperations.SetManualCompletion();
            ImmediateNodeCommand nodeCommand = new ImmediateNodeCommand();
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").Build();
            _sceneLoader.LoadScene(sceneData);
            _sceneOperations.CompleteNextOperation();

            _sceneLoader.SetNodeCommandOfALoadedScene(nodeCommand);
            _sceneOperations.CompleteNextOperation();

            Assert.AreEqual(1, nodeCommand.TotalExecutions);
        }

        [Test]
        public void LoadScene_TheSceneHasNoNodeCommand_LoadsTheNextScene()
        {
            SceneData childSceneData = new SceneDataBuilder().WithSceneName("Child").Build();
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game")
                .WithScenesToOpen(childSceneData).Build();

            _sceneLoader.LoadScene(sceneData);

            Assert.AreEqual(1, _sceneOperations.GetTotalLoadsOfScene("Game"));
        }

        [Test]
        public void ReloadCurrentScene_TheCurrentSceneUsesALoadingScreen_LoadsTheLoadingScreenOnce()
        {
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").WithLoadingScreen().Build();
            _sceneLoader.LoadScene(sceneData);

            _sceneLoader.ReloadCurrentScene();

            Assert.AreEqual(2, _sceneOperations.GetTotalLoadsOfScene(LoadingScreenSceneName));
        }

        [Test]
        public void ReloadCurrentScene_TheCurrentSceneDoesNotCloseTheOtherScenes_LoadsTheCurrentSceneAgain()
        {
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").Build();
            _sceneLoader.LoadScene(sceneData);

            _sceneLoader.ReloadCurrentScene();

            Assert.AreEqual(2, _sceneOperations.GetTotalLoadsOfScene("Game"));
        }

        [Test]
        public void RemoveScene_TheSceneIsNotOpened_DoesNotUnloadIt()
        {
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").Build();

            _sceneLoader.RemoveScene(sceneData);

            Assert.IsEmpty(_sceneOperations.UnloadedScenes);
        }

        [Test]
        public void RemoveScene_TheSceneIsOpened_UnloadsIt()
        {
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").Build();
            _sceneLoader.LoadScene(sceneData);

            _sceneLoader.RemoveScene(sceneData);

            Assert.IsFalse(_sceneLoader.IsThisSceneDataOpened(sceneData));
        }

        [Test]
        public void RemoveCurrentAndSetPrincipal_TheCurrentSceneIsOpened_UnloadsTheCurrentScene()
        {
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").Build();
            _sceneLoader.LoadScene(sceneData);

            _sceneLoader.RemoveCurrentAndSetPrincipal(_firstSceneData);

            Assert.IsFalse(_sceneLoader.IsThisSceneDataOpened(sceneData));
        }

        [Test]
        public void RemoveCurrentAndSetPrincipal_TheCurrentSceneIsOpened_RaisesTheAllScenesLoadedEvent()
        {
            int totalEventsRaised = 0;
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").Build();
            _sceneLoader.LoadScene(sceneData);
            _onAllScenesLoadedEventViewModel.OnEventRaised += () => totalEventsRaised++;

            _sceneLoader.RemoveCurrentAndSetPrincipal(_firstSceneData);

            Assert.AreEqual(1, totalEventsRaised);
        }

        [Test]
        public void LoadScene_AllTheScenesAreLoaded_InvokesTheOnAllScenesLoadedAction()
        {
            int totalActionsInvoked = 0;
            _sceneLoader.OnAllScenesLoaded += () => totalActionsInvoked++;
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").Build();

            _sceneLoader.LoadScene(sceneData);

            Assert.AreEqual(1, totalActionsInvoked);
        }

        [Test]
        public void LoadScene_TheSceneUsesALoadingScreen_WaitsTheTimeBeforeLoading()
        {
            SceneData sceneData = new SceneDataBuilder().WithSceneName("Game").WithLoadingScreen().Build();

            _sceneLoader.LoadScene(sceneData);

            Assert.Contains(0.25f, (System.Collections.ICollection)_delayProvider.RequestedSeconds);
        }
    }
}
