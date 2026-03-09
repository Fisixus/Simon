using UnityEngine;
using System;

namespace Simon.Core.DI
{
    // --- MVC Integration ---
    internal interface IViewSource<T>
    {
        T Spawn(Transform parent);
        void Despawn(T item);
        bool HasSafeArea { get; }
    }

    // --- Class Factories ---
    internal abstract class ClassFactory<T>
    {
        [Inject] protected DiContainer Container { get; set; }

        public virtual T Spawn()
        {
            var instance = CreateInstance();
            Container.Inject(instance);
            return instance;
        }

        protected abstract T CreateInstance();
    }

    internal abstract class ClassFactory<T, TModel>
    {
        [Inject] protected DiContainer Container { get; set; }

        public virtual T Spawn(TModel model)
        {
            var instance = CreateInstance(model);
            Container.Inject(instance);
            return instance;
        }

        protected abstract T CreateInstance(TModel model);
    }

    // --- Object Factory ---
    internal abstract class ObjectFactory<T> where T : UnityEngine.Object
    {
        [Inject] protected DiContainer Container { get; set; }
        protected T Prefab;

        public void SetPrefab(T prefab) => Prefab = prefab;

        public virtual T Spawn(Transform parent = null)
        {
            T instance = UnityEngine.Object.Instantiate(Prefab, parent);
            
            if (instance is GameObject go)
                Container.InjectGameObject(go);
            else if (instance is Component comp)
                Container.InjectGameObject(comp.gameObject);

            return instance;
        }
    }
}
