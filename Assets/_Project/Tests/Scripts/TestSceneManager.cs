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
            SceneManager.LoadScene("TestMainMenu");
            Debug.Log("[TestSceneManager] Invoking TestLoadMainMenuSignal...");
            _signalBus.Invoke(new TestLoadMainMenuSignal());
        }
    }
}
