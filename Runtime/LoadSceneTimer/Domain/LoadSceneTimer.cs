using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace ScenesLoaderSystem
{
    public class LoadSceneTimer : MonoBehaviour
    {
        private ISceneDataLoader _sceneDataLoader;
        private float _duration;

        public void Install(float duration, ISceneDataLoader sceneDataLoader)
        {
            _duration = duration;
            _sceneDataLoader = sceneDataLoader;

            StartCoroutine(Timer());
        }

        private IEnumerator Timer()
        {
            yield return new WaitForSeconds(_duration);
            _sceneDataLoader.Load();
        }
    }
}