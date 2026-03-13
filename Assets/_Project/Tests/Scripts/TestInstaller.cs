using Simon.Core.DI;
using Simon.Core.MVC;
using UnityEngine;

namespace Simon.Tests
{
    internal class TestInstaller : PrefabInstaller
    {
        [SerializeField] private TestView _testViewPrefab;
        [SerializeField] private TestBullet _bulletPrefab;
        [SerializeField] private Viewer _viewer;

        public override void InstallBindings()
        {
            // 0. Bind the Viewer instance
            if (_viewer != null)
                Container.Bind<Viewer>().FromInstance(_viewer).AsSingle();

            // 1. Bind View Factory and expose its Interface
            Container.RegisterFactory<TestView, TestViewFactory>(_testViewPrefab)
                     .Expose<IViewSource<ITestView>>();

            // 2. Register Pools
            Container.RegisterPool<TestItem, TestItemPool>().WithMinimumCount(5);
            Container.RegisterPool<TestBullet, TestBulletPool>(_bulletPrefab)
                     .WithRoot(transform)
                     .WithMinimumCount(10);
            
            // 3. Bind the Controller
            Container.Bind<TestController>().AsSingle().NonLazy();
        }
    }
}
