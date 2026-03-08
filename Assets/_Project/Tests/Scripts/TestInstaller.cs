using Simon.Core.DI;
using Simon.Core.Events;
using UnityEngine;

namespace Simon.Tests
{
    internal class TestInstaller : MonoInstaller
    {
        [SerializeField] private TestView _testView;

        public override void InstallBindings()
        {
            // 1. Bind Core Services
            Container.Bind<SignalBus>().FromInstance(new SignalBus()).AsSingle();
            
            // 2. Bind the View instance
            Container.Bind<TestView>().FromInstance(_testView).AsSingle();
            
            // 3. Bind the Controller as a singleton and make it NonLazy so it starts with the container
            Container.Bind<TestController>().AsSingle().NonLazy();
        }
    }
}
