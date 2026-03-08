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
            
            // 2. Bind the View instance to its Interface
            Container.Bind<ITestView>().FromInstance(_testView).AsSingle();
            
            // 3. Bind the Controller
            Container.Bind<TestController>().AsSingle().NonLazy();
        }
    }
}
