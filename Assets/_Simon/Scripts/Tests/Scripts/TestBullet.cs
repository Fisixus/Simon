using Simon.Core.DI;
using UnityEngine;

namespace Simon.Tests
{
    internal class TestBullet : MonoBehaviour, ISpawnable, IDespawnable
    {
        public void OnSpawn() => Debug.Log($"[TestBullet] {gameObject.name} Spawned");
        public void OnDespawn() => Debug.Log($"[TestBullet] {gameObject.name} Despawned");
    }

    internal class TestBulletPool : ObjectPool<TestBullet>
    {
        protected override TestBullet CreateInstanceForPool()
        {
            var item = base.CreateInstanceForPool();
            item.gameObject.name = $"Bullet_{_pool.Count}";
            return item;
        }
    }
}
