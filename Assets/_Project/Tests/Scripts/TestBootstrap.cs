using Simon.Core.DI;
using Simon.Core.Events;
using UnityEngine;

namespace Simon.Tests
{
    internal class TestBootstrap : MonoBehaviour
    {
        [SerializeField] private TestView _testView;

        private DiContainer _container;
        private TestController _testController;

        private void Start()
        {
            // 1. Initialize DI Container
            _container = new DiContainer();
            
            // 2. Register Core Services (Zenject-like fluent API)
            _container.Bind<SignalBus>().FromInstance(new SignalBus());
            
            // 3. Setup Models (Not bound as they are data containers)
            var testModel = new TestModel { Value = 100 };
            
            // 4. Setup Controller (using DI)
            _testController = (TestController)_container.Instantiate(typeof(TestController));
            _testController.Setup(_testView, testModel);
            
            // 5. Initialize View
            _testView.Initialize();

            // 6. Initialize Controller (Zenject-like)
            _testController.Initialize();
            
            Debug.Log("[TestBootstrap] Test project initialized with Zenject-like DI!");
        }

        private void OnDestroy()
        {
            _testController?.Dispose();
            _container?.Dispose();
        }
    }
}
