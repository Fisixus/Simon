using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simon.Core.DI
{
    // --- Interfaces ---
    public interface ISpawnable { void OnSpawn(); }
    public interface IDespawnable { void OnDespawn(); }

    // --- Base Pool ---
    public abstract class PoolBase<T> : IInitializable
    {
        [Inject] protected DiContainer Container { get; set; }
        
        protected readonly Stack<T> _pool = new Stack<T>();
        public int MinimumCount { get; set; }
        public float ShrinkPeriod { get; set; }

        public abstract T Spawn();
        public abstract void Despawn(T item);

        public virtual void Initialize()
        {
            Prewarm();
        }

        protected virtual void Prewarm()
        {
            while (_pool.Count < MinimumCount)
            {
                _pool.Push(CreateInstanceForPool());
            }
        }

        protected abstract T CreateInstanceForPool();
        
        public virtual void Clear()
        {
            _pool.Clear();
        }

        protected void HandleSpawn(T item)
        {
            if (item is ISpawnable spawnable) spawnable.OnSpawn();
        }

        protected void HandleDespawn(T item)
        {
            if (item is IDespawnable despawnable) despawnable.OnDespawn();
        }
    }

    // --- Class Pool ---
    public abstract class ClassPool<T> : PoolBase<T> where T : class
    {
        public override T Spawn()
        {
            T item = _pool.Count > 0 ? _pool.Pop() : CreateInstanceForPool();
            Container.Inject(item);
            HandleSpawn(item);
            return item;
        }

        public override void Despawn(T item)
        {
            HandleDespawn(item);
            ResetInstance(item);
            _pool.Push(item);
        }

        protected override T CreateInstanceForPool() => CreateInstance();
        protected abstract T CreateInstance();
        protected abstract void ResetInstance(T item);
    }
    
    // --- Object Pool ---
    public abstract class ObjectPool<T> : PoolBase<T> where T : UnityEngine.Object
    {
        protected T Prefab;
        protected Transform Root;

        public void SetPrefab(T prefab) => Prefab = prefab;
        public void SetRoot(Transform root)
        {
            Root = root;
            if (Root == null) return;

            foreach (var item in _pool)
            {
                if (item is Component comp) comp.transform.SetParent(Root);
                else if (item is GameObject go) go.transform.SetParent(Root);
            }
        }

        public override T Spawn()
        {
            T item = _pool.Count > 0 ? _pool.Pop() : CreateInstanceForPool();
            
            if (item is Component comp) comp.gameObject.SetActive(true);
            else if (item is GameObject go) go.SetActive(true);

            HandleSpawn(item);
            return item;
        }

        public override void Despawn(T item)
        {
            HandleDespawn(item);

            if (item is Component comp)
            {
                comp.gameObject.SetActive(false);
                if (Root != null) comp.transform.SetParent(Root);
            }
            else if (item is GameObject go)
            {
                go.SetActive(false);
                if (Root != null) go.transform.SetParent(Root);
            }

            _pool.Push(item);
        }

        protected override T CreateInstanceForPool()
        {
            if (Root == null)
            {
                Root = new GameObject($"Pool_{typeof(T).Name}").transform;
                UnityEngine.Object.DontDestroyOnLoad(Root.gameObject);
            }
            T instance = UnityEngine.Object.Instantiate(Prefab, Root);
            
            if (instance is GameObject go)
            {
                Container.InjectGameObject(go);
                go.SetActive(false);
            }
            else if (instance is Component comp)
            {
                Container.InjectGameObject(comp.gameObject);
                comp.gameObject.SetActive(false);
            }
            
            return instance;
        }
    }
}
