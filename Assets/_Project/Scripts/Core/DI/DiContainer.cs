using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace Simon.Core.DI
{
    internal interface IInitializable
    {
        void Initialize();
    }

    internal interface IDisposable
    {
        void Dispose();
    }

    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    internal class InjectAttribute : Attribute { }

    internal class DiContainer : IDisposable
    {
        private readonly Dictionary<Type, Binding> _bindings = new Dictionary<Type, Binding>();
        private readonly List<object> _singletons = new List<object>();
        private readonly DiContainer _parent;

        public DiContainer(DiContainer parent = null)
        {
            _parent = parent;
        }

        public BindingCondition Bind<T>()
        {
            var binding = new Binding { ContractType = typeof(T), ImplementationType = typeof(T) };
            _bindings[typeof(T)] = binding;
            return new BindingCondition(binding);
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type type)
        {
            if (_bindings.TryGetValue(type, out var binding))
            {
                if (binding.IsSingleton && binding.Instance != null)
                {
                    return binding.Instance;
                }

                var instance = Instantiate(binding.ImplementationType);

                if (binding.IsSingleton)
                {
                    binding.Instance = instance;
                    _singletons.Add(instance);
                }

                return instance;
            }

            if (_parent != null)
            {
                return _parent.Resolve(type);
            }

            return Instantiate(type);
        }

        public T Instantiate<T>()
        {
            return (T)Instantiate(typeof(T));
        }

        public object Instantiate(Type type)
        {
            var constructor = type.GetConstructors()
                .OrderByDescending(c => c.GetCustomAttribute<InjectAttribute>() != null)
                .FirstOrDefault();

            object instance;
            if (constructor == null || constructor.GetParameters().Length == 0)
            {
                instance = Activator.CreateInstance(type);
            }
            else
            {
                var parameters = constructor.GetParameters()
                    .Select(p => Resolve(p.ParameterType))
                    .ToArray();
                instance = constructor.Invoke(parameters);
            }

            Inject(instance);
            return instance;
        }

        public void Inject(object instance)
        {
            if (instance == null) return;
            var type = instance.GetType();
            
            // Field Injection
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<InjectAttribute>() != null);

            foreach (var field in fields)
            {
                field.SetValue(instance, Resolve(field.FieldType));
            }
            
            // Property Injection
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<InjectAttribute>() != null);

            foreach (var prop in props)
            {
                prop.SetValue(instance, Resolve(prop.PropertyType));
            }
        }

        public void InjectGameObject(GameObject gameObject)
        {
            var components = gameObject.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var comp in components)
            {
                Inject(comp);
            }
        }

        // Factory Registration Helpers
        public BindingCondition RegisterFactory<T, TFactory>() where TFactory : class
        {
            return Bind<TFactory>().AsSingle();
        }

        public BindingCondition RegisterFactory<T, TFactory>(T prefab) where T : UnityEngine.Object
        {
            var factoryType = typeof(TFactory);
            var factory = (TFactory)Activator.CreateInstance(factoryType);
            
            var setPrefabMethod = factoryType.GetMethod("SetPrefab");
            if (setPrefabMethod != null)
                setPrefabMethod.Invoke(factory, new object[] { prefab });

            return Bind<TFactory>().FromInstance(factory).AsSingle();
        }

        // Pool Registration Helpers
        public PoolBindingCondition RegisterPool<T, TPool>() where TPool : class
        {
            var poolType = typeof(TPool);
            var pool = (TPool)Activator.CreateInstance(poolType);
            Bind<TPool>().FromInstance(pool).AsSingle();
            return new PoolBindingCondition(pool);
        }

        public PoolBindingCondition RegisterPool<T, TPool>(T prefab) where T : UnityEngine.Object
        {
            var poolType = typeof(TPool);
            var pool = (TPool)Activator.CreateInstance(poolType);
            
            var setPrefabMethod = poolType.GetMethod("SetPrefab");
            if (setPrefabMethod != null)
                setPrefabMethod.Invoke(pool, new object[] { prefab });

            Bind<TPool>().FromInstance(pool).AsSingle();
            return new PoolBindingCondition(pool);
        }

        internal class PoolBindingCondition
        {
            private readonly object _pool;
            public PoolBindingCondition(object pool) => _pool = pool;

            public PoolBindingCondition WithMinimumCount(int count)
            {
                var prop = _pool.GetType().GetProperty("MinimumCount");
                if (prop != null) prop.SetValue(_pool, count);
                return this;
            }

            public PoolBindingCondition WithShrinkPeriod(float period)
            {
                var prop = _pool.GetType().GetProperty("ShrinkPeriod");
                if (prop != null) prop.SetValue(_pool, period);
                return this;
            }
        }

        public void Initialize()
        {
            // First pass: Instantiate all NonLazy singletons
            foreach (var binding in _bindings.Values.ToList())
            {
                if (binding.IsNonLazy && binding.IsSingleton && binding.Instance == null)
                {
                    Resolve(binding.ContractType);
                }
            }

            // Second pass: Initialize all IInitializable singletons that exist
            foreach (var singleton in _singletons.ToList())
            {
                if (singleton is IInitializable initializable)
                {
                    initializable.Initialize();
                }
            }
        }

        public void Dispose()
        {
            foreach (var singleton in _singletons)
            {
                if (singleton is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            _singletons.Clear();
            _bindings.Clear();
        }

        internal class Binding
        {
            public Type ContractType;
            public Type ImplementationType;
            public bool IsSingleton;
            public bool IsNonLazy;
            public object Instance;
        }

        internal class BindingCondition
        {
            private readonly Binding _binding;
            public BindingCondition(Binding binding) => _binding = binding;

            public BindingCondition To<T>()
            {
                _binding.ImplementationType = typeof(T);
                return this;
            }

            public BindingCondition AsSingle()
            {
                _binding.IsSingleton = true;
                return this;
            }

            public BindingCondition NonLazy()
            {
                _binding.IsNonLazy = true;
                return this;
            }

            public BindingCondition FromInstance(object instance)
            {
                _binding.Instance = instance;
                _binding.IsSingleton = true;
                return this;
            }
        }
    }
}
