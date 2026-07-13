using System;
using NUnit.Framework;
using ScenesLoaderSystem.Core.Domain;
using ScenesLoaderSystem.Core.InterfaceAdapters;

namespace ScenesLoaderSystem.Tests
{
    public class SceneDataSOTests
    {
        [Test]
        public void GetSceneData_TheSceneDataSOIsConfigured_ReturnsASceneDataWithItsSceneName()
        {
            SceneDataSO sceneDataSO = new SceneDataSOBuilder().WithSceneName("Game").Build();

            SceneData sceneData = sceneDataSO.GetSceneData();

            Assert.AreEqual("Game", sceneData.SceneName);
        }

        [Test]
        public void GetSceneData_ItIsCalledTwice_ReturnsTheSameSceneData()
        {
            SceneDataSO sceneDataSO = new SceneDataSOBuilder().WithSceneName("Game").Build();

            SceneData sceneData = sceneDataSO.GetSceneData();

            Assert.AreSame(sceneData, sceneDataSO.GetSceneData());
        }

        [Test]
        public void GetSceneData_TheSceneDataSOHasAChildScene_ReturnsTheChildInTheScenesToOpen()
        {
            SceneDataSO childSceneDataSO = new SceneDataSOBuilder().WithSceneName("Child").Build();
            SceneDataSO sceneDataSO = new SceneDataSOBuilder().WithSceneName("Game")
                .WithScenesToOpen(childSceneDataSO).Build();

            SceneData[] scenesToOpen = sceneDataSO.GetSceneData().GetAllScenesToOpen();

            Assert.AreEqual(2, scenesToOpen.Length);
        }

        [Test]
        public void GetSceneData_TwoSceneDataSOsOpenEachOther_ThrowsAnException()
        {
            SceneDataSO firstSceneDataSO = new SceneDataSOBuilder().WithSceneName("First").Build();
            SceneDataSO secondSceneDataSO = new SceneDataSOBuilder().WithSceneName("Second")
                .WithScenesToOpen(firstSceneDataSO).Build();
            PrivateFieldWriter.WriteWithName(firstSceneDataSO, "_scenesDataToOpen", new[] { secondSceneDataSO });

            Assert.Throws<Exception>(() => firstSceneDataSO.GetSceneData());
        }
    }
}
