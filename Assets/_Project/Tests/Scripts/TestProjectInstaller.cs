using Simon.Core.DI;
using Simon.Core.Events;
using UnityEngine;

namespace Simon.Tests
{
    internal class TestProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<SignalBus>().FromInstance(new SignalBus()).AsSingle(); 

            Container.Bind<TestSceneManager>().AsSingle();

            Container.Bind<TestGameManager>().AsSingle().NonLazy();
        }
    }
}
