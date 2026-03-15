using Simon.Core.DI;
using Simon.Core.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Simon.Tests
{
     public class TestSceneManager
    {
        [Inject] private SignalBus _signalBus;

        public void LoadMainMenu()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("TestMainMenu");
            Debug.Log("[TestSceneManager] Loading TestMainMenu scene...");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "TestMainMenu")
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
                Debug.Log("[TestSceneManager] Invoking TestLoadMainMenuSignal...");
                _signalBus.Invoke(new TestLoadMainMenuSignal());
            }
        }
    }
}
