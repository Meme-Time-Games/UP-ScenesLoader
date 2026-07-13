using NUnit.Framework;
using ScenesLoaderSystem.Core.Domain;

namespace ScenesLoaderSystem.Tests
{
    public class SceneDataTests
    {
        [Test]
        public void GetAllScenesDataToRemove_ScenesDataToRemoveIsNull_ReturnsEmptyArray()
        {
            SceneData sceneData = new SceneDataBuilder().WithNullScenesToRemove().Build();

            SceneData[] scenesToRemove = sceneData.GetAllScenesDataToRemove();

            Assert.IsEmpty(scenesToRemove);
        }

        [Test]
        public void GetAllScenesDataToRemove_HasTwoScenesToRemove_ReturnsBothScenes()
        {
            SceneData firstSceneToRemove = new SceneDataBuilder().WithSceneName("First").Build();
            SceneData secondSceneToRemove = new SceneDataBuilder().WithSceneName("Second").Build();
            SceneData sceneData = new SceneDataBuilder()
                .WithScenesToRemove(firstSceneToRemove, secondSceneToRemove).Build();

            SceneData[] scenesToRemove = sceneData.GetAllScenesDataToRemove();

            Assert.AreEqual(new[] { firstSceneToRemove, secondSceneToRemove }, scenesToRemove);
        }

        [Test]
        public void GetAllScenesToOpen_ScenesDataToOpenIsNull_ReturnsOnlyItself()
        {
            SceneData sceneData = new SceneDataBuilder().WithNullScenesToOpen().Build();

            SceneData[] scenesToOpen = sceneData.GetAllScenesToOpen();

            Assert.AreEqual(new[] { sceneData }, scenesToOpen);
        }

        [Test]
        public void GetAllScenesToOpen_HasAChildScene_ReturnsTheChildBeforeItself()
        {
            SceneData childSceneData = new SceneDataBuilder().WithSceneName("Child").Build();
            SceneData sceneData = new SceneDataBuilder().WithScenesToOpen(childSceneData).Build();

            SceneData[] scenesToOpen = sceneData.GetAllScenesToOpen();

            Assert.AreEqual(new[] { childSceneData, sceneData }, scenesToOpen);
        }

        [Test]
        public void GetAllScenesToOpen_TwoChildrenShareAScene_ReturnsTheSharedSceneOnce()
        {
            SceneData sharedSceneData = new SceneDataBuilder().WithSceneName("Shared").Build();
            SceneData firstChildSceneData = new SceneDataBuilder().WithSceneName("FirstChild")
                .WithScenesToOpen(sharedSceneData).Build();
            SceneData secondChildSceneData = new SceneDataBuilder().WithSceneName("SecondChild")
                .WithScenesToOpen(sharedSceneData).Build();
            SceneData sceneData = new SceneDataBuilder()
                .WithScenesToOpen(firstChildSceneData, secondChildSceneData).Build();

            SceneData[] scenesToOpen = sceneData.GetAllScenesToOpen();

            Assert.AreEqual(new[] { sharedSceneData, firstChildSceneData, secondChildSceneData, sceneData },
                scenesToOpen);
        }
    }
}
