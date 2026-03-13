using Simon.Core.DI;
using UnityEngine;

namespace Simon.Tests
{
    public class TestGameManager : IInitializable
    {
        [Inject] private TestSceneManager _sceneManager;

        public void Initialize()
        {
            Debug.Log("[TestGameManager] Starting Game...");
            _sceneManager.LoadMainMenu();
        }
    }
}
