using Simon.Core.DI;
using Simon.Core.Events;
using UnityEngine;

namespace Simon.Tests
{
    internal class TestInstaller : MonoInstaller
    {
        [SerializeField] private TestView _testViewPrefab;
        [SerializeField] private TestBullet _bulletPrefab;

        public override void InstallBindings()
        {
            // 1. Ensure Core Services are present in Test
            var projectInstaller = new GameObject("ProjectInstaller_Test").AddComponent<ProjectInstaller>();
            Container.Inject(projectInstaller);
            projectInstaller.InstallBindings();

            // 2. Bind View Factory and expose its Interface
            Container.RegisterFactory<TestView, TestViewFactory>(_testViewPrefab)
                     .Expose<IViewSource<ITestView>>();

            // 3. Register Pools
            Container.RegisterPool<TestItem, TestItemPool>().WithMinimumCount(5);
            Container.RegisterPool<TestBullet, TestBulletPool>(_bulletPrefab).WithMinimumCount(10);
            
            // 4. Bind the Controller
            Container.Bind<TestController>().AsSingle().NonLazy();
        }
    }
}
