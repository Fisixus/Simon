using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simon.Core.DI
{
    // --- Interfaces ---
    internal interface ISpawnable { void OnSpawn(); }
    internal interface IDespawnable { void OnDespawn(); }

    // --- Base Pool ---
    internal abstract class PoolBase<T>
    {
        [Inject] protected DiContainer Container { get; set; }
        
        protected readonly Stack<T> _pool = new Stack<T>();
        public int MinimumCount { get; set; }
        public float ShrinkPeriod { get; set; }

        public abstract T Spawn();
        public abstract void Despawn(T item);
        
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
    internal abstract class ClassPool<T> : PoolBase<T> where T : class
    {
        public override T Spawn()
        {
            T item = _pool.Count > 0 ? _pool.Pop() : CreateInstance();
            Container.Inject(item);
            HandleSpawn(item);
            return item;
        }

        public override void Despawn(T item)
        {
            HandleDespawn(item);
            _pool.Push(item);
        }

        protected abstract T CreateInstance();
        protected abstract void ResetInstance(T item);
    }

    internal abstract class ClassPool<T, TModel> : PoolBase<T> where T : class
    {
        public virtual T Spawn(TModel model)
        {
            T item = _pool.Count > 0 ? _pool.Pop() : CreateInstance(model);
            Container.Inject(item);
            HandleSpawn(item);
            return item;
        }

        public override T Spawn() => throw new NotSupportedException("Use Spawn(TModel model) instead.");

        public override void Despawn(T item)
        {
            HandleDespawn(item);
            _pool.Push(item);
        }

        public virtual void Despawn(T item, TModel model)
        {
            HandleDespawn(item);
            ResetInstance(item, model);
            _pool.Push(item);
        }

        protected abstract T CreateInstance(TModel model);
        protected abstract void ResetInstance(T item, TModel model);
    }

    // --- Object Pool ---
    internal abstract class ObjectPool<T> : PoolBase<T> where T : UnityEngine.Object
    {
        protected T Prefab;
        protected Transform Root;

        public void SetPrefab(T prefab) => Prefab = prefab;

        public override T Spawn()
        {
            T item = _pool.Count > 0 ? _pool.Pop() : CreateInstance();
            
            if (item is Component comp) comp.gameObject.SetActive(true);
            else if (item is GameObject go) go.SetActive(true);

            HandleSpawn(item);
            return item;
        }

        public override void Despawn(T item)
        {
            HandleDespawn(item);

            if (item is Component comp) comp.gameObject.SetActive(false);
            else if (item is GameObject go) go.SetActive(false);

            _pool.Push(item);
        }

        protected virtual T CreateInstance()
        {
            if (Root == null) Root = new GameObject($"Pool_{typeof(T).Name}").transform;
            T instance = UnityEngine.Object.Instantiate(Prefab, Root);
            
            if (instance is GameObject go) Container.InjectGameObject(go);
            else if (instance is Component comp) Container.InjectGameObject(comp.gameObject);
            
            return instance;
        }
    }
}
