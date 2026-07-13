using System.Collections;
using System.Collections.Generic;
using DependencyInjector.Core;
using MVVM.Core;
using NUnit.Framework;
using ScenesLoaderSystem.Core.Domain;
using ScenesLoaderSystem.Core.Installers;
using ServiceLocatorPattern;
using UnityEngine;
using UnityEngine.TestTools;

namespace ScenesLoaderSystem.Tests
{
    public class SceneLoaderInstallerTests
    {
        private readonly List<GameObject> _createdGameObjects = new List<GameObject>();

        [TearDown]
        public void TearDown()
        {
            if (ServiceLocatorInstance.Instance.IsContained<ISceneLoader>())
                ServiceLocatorInstance.Instance.Remove<ISceneLoader>();

            foreach (var createdGameObject in _createdGameObjects)
            {
                Object.Destroy(createdGameObject);
            }

            _createdGameObjects.Clear();
        }

        [UnityTest]
        public IEnumerator Install_TheSceneLoaderIsInstalledTwice_KeepsTheSceneLoaderRegistered()
        {
            InstallSceneLoader();

            InstallSceneLoader();

            yield return null;

            Assert.IsTrue(ServiceLocatorInstance.Instance.IsContained<ISceneLoader>());
        }

        [UnityTest]
        public IEnumerator Install_TheSceneLoaderIsInstalledTwice_KeepsTheFirstSceneLoader()
        {
            InstallSceneLoader();
            ISceneLoader firstSceneLoader = ServiceLocatorInstance.Instance.Get<ISceneLoader>();

            InstallSceneLoader();

            yield return null;

            Assert.AreSame(firstSceneLoader, ServiceLocatorInstance.Instance.Get<ISceneLoader>());
        }

        private void InstallSceneLoader()
        {
            GameObject installerGameObject = new GameObject("SceneLoaderInstaller");
            _createdGameObjects.Add(installerGameObject);

            SceneLoaderInstaller sceneLoaderInstaller = installerGameObject.AddComponent<SceneLoaderInstaller>();

            PrivateFieldWriter.WriteWithName(sceneLoaderInstaller, "_loadingScreenSceneDataSo",
                new SceneDataSOBuilder().WithSceneName("LoadingScreen").Build());
            PrivateFieldWriter.WriteWithName(sceneLoaderInstaller, "_emptySceneDataSo",
                new SceneDataSOBuilder().WithSceneName("Empty").Build());
            PrivateFieldWriter.WriteWithName(sceneLoaderInstaller, "_firstOpenSceneDataSo",
                new SceneDataSOBuilder().WithSceneName("First").Build());
            PrivateFieldWriter.WriteWithName(sceneLoaderInstaller, "_onAllSceneAreLoadedEventViewModelSO",
                ScriptableObject.CreateInstance<EventViewModelSO>());
            PrivateFieldWriter.WriteWithName(sceneLoaderInstaller, "_onLoadingIsFinishingEventViewModelsSO",
                new EventViewModelSO[0]);

            sceneLoaderInstaller.Install(new DIContainer());
        }
    }
}
