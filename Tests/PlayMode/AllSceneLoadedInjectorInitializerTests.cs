using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using ScenesLoaderSystem.Core.Domain;
using ScenesLoaderSystem.Installers;
using ServiceLocatorPattern;
using UnityEngine;
using UnityEngine.TestTools;

namespace ScenesLoaderSystem.Tests
{
    public class AllSceneLoadedInjectorInitializerTests
    {
        private readonly List<GameObject> _createdGameObjects = new List<GameObject>();

        private FakeSceneLoader _sceneLoader;
        private FakeMonoInjector _monoInjector;

        [SetUp]
        public void SetUp()
        {
            _sceneLoader = new FakeSceneLoader();

            ServiceLocatorInstance.Instance.Add<ISceneLoader>(_sceneLoader);
        }

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
        public IEnumerator OnAllScenesLoaded_TheEventIsRaisedTwice_InjectsOnlyOnce()
        {
            CreateInjectorInitializer();

            yield return null;

            _sceneLoader.RaiseAllScenesLoaded();
            _sceneLoader.RaiseAllScenesLoaded();

            Assert.AreEqual(1, _monoInjector.TotalInjections);
        }

        [UnityTest]
        public IEnumerator OnAllScenesLoaded_TheEventIsRaisedOnce_InjectsTheInjector()
        {
            CreateInjectorInitializer();

            yield return null;

            _sceneLoader.RaiseAllScenesLoaded();

            Assert.AreEqual(1, _monoInjector.TotalInjections);
        }

        private void CreateInjectorInitializer()
        {
            GameObject injectorGameObject = new GameObject("Injector");
            _createdGameObjects.Add(injectorGameObject);

            _monoInjector = injectorGameObject.AddComponent<FakeMonoInjector>();

            GameObject initializerGameObject = new GameObject("AllSceneLoadedInjectorInitializer");
            _createdGameObjects.Add(initializerGameObject);

            AllSceneLoadedInjectorInitializer injectorInitializer =
                initializerGameObject.AddComponent<AllSceneLoadedInjectorInitializer>();

            PrivateFieldWriter.WriteWithName(injectorInitializer, "_monoInjector", _monoInjector);
        }
    }
}
