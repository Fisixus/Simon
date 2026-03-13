using Simon.Core.DI;
using Simon.Core.Events;
using UnityEngine;

namespace Simon.Tests
{
    public class TestSceneManager
    {
        [Inject] private SignalBus _signalBus;

        public void LoadMainMenu()
        {
            Debug.Log("[TestSceneManager] Loading Main Menu...");
            _signalBus.Invoke(new TestLoadMainMenuSignal());
        }
    }
}
