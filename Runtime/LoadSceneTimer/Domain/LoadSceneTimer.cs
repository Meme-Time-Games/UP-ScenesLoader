using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace ScenesLoaderSystem
{
    public class LoadSceneTimer : MonoBehaviour
    {
        private ISceneDataLoader _sceneDataLoader;
        private int _duration;

        public void Install(int duration, ISceneDataLoader sceneDataLoader)
        {
            _duration = duration;
            _sceneDataLoader = sceneDataLoader;

            StartCoroutine(Timer());
        }

        public IEnumerator Timer()
        {
            yield return new WaitForSeconds(_duration);
            _sceneDataLoader.Load();
        }
    }
}