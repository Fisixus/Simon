using Simon.Core.DI;
using Simon.Core.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Simon.Tests
{
    internal class TestProjectInstaller : MonoInstaller
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private EventSystem _eventSystem;

        public override void InstallBindings()
        {
            Container.Bind<SignalBus>().FromInstance(new SignalBus()).AsSingle(); 

            if (_mainCamera != null)
            {
                _mainCamera.transform.SetParent(transform);
                Container.Bind<Camera>().FromInstance(_mainCamera).AsSingle();
            }
            if (_eventSystem != null)
            {
                _eventSystem.transform.SetParent(transform);
                Container.Bind<EventSystem>().FromInstance(_eventSystem).AsSingle();
            }

            Container.Bind<TestSceneManager>().AsSingle();

            Container.Bind<TestGameManager>().AsSingle().NonLazy();
        }
    }
}
