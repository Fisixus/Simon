using Simon.Core.DI;
using Simon.Core.Events;
using UnityEngine;

namespace Simon.Tests
{
    internal class TestProjectInstaller : MonoInstaller
    {
        [SerializeField] private Camera _mainCamera;

        public override void InstallBindings()
        {
            Container.Bind<SignalBus>().FromInstance(new SignalBus()).AsSingle(); 

            if (_mainCamera != null)
            {
                _mainCamera.transform.SetParent(transform);
                Container.Bind<Camera>().FromInstance(_mainCamera).AsSingle();
            }

            Container.Bind<TestSceneManager>().AsSingle();

            Container.Bind<TestGameManager>().AsSingle().NonLazy();
        }
    }
}
