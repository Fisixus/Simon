using Simon.Core.DI;
using UnityEngine;

namespace Simon.Tests
{
    internal class TestItem : ISpawnable, IDespawnable
    {
        public int Id;

        public void OnSpawn() => Debug.Log($"[TestItem] {Id} Spawned/Recycled");
        public void OnDespawn() => Debug.Log($"[TestItem] {Id} Despawned");
    }

    internal class TestItemFactory : ClassFactory<TestItem>
    {
        private int _counter;
        protected override TestItem CreateInstance() => new TestItem { Id = ++_counter };
    }

    internal class TestItemPool : ClassPool<TestItem>
    {
        [Inject] private TestItemFactory _factory;

        protected override TestItem CreateInstance() => _factory.Spawn();
        protected override void ResetInstance(TestItem item) { }
    }
}
