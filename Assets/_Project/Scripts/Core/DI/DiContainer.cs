using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace Simon.Core.DI
{
    public interface IInitializable
    {
        void Initialize();
    }

    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class InjectAttribute : Attribute { }

    public class DiContainer : IDisposable
    {
        private readonly Dictionary<Type, Binding> _bindings = new Dictionary<Type, Binding>();
        private readonly List<object> _singletons = new List<object>();
        private readonly HashSet<object> _injectedObjects = new HashSet<object>();
        private readonly DiContainer _parent;
        private readonly Stack<Type> _resolutionStack = new Stack<Type>();

        // Reflection Cache
        private static readonly Dictionary<Type, ConstructorInfo> _constructorCache = new Dictionary<Type, ConstructorInfo>();
        private static readonly Dictionary<Type, FieldInfo[]> _fieldCache = new Dictionary<Type, FieldInfo[]>();
        private static readonly Dictionary<Type, PropertyInfo[]> _propertyCache = new Dictionary<Type, PropertyInfo[]>();
        private static readonly Dictionary<Type, MethodInfo[]> _methodCache = new Dictionary<Type, MethodInfo[]>();

        public DiContainer(DiContainer parent = null)
        {
            _parent = parent;
        }

        public BindingCondition Bind<T>()
        {
            var binding = new Binding { ContractType = typeof(T), ImplementationType = typeof(T) };
            _bindings[typeof(T)] = binding;
            return new BindingCondition(binding, this);
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type type)
        {
            if (_resolutionStack.Contains(type))
            {
                throw new Exception($"Circular dependency detected for type {type.FullName}. Stack: {string.Join(" -> ", _resolutionStack.Select(t => t.Name))}");
            }

            _resolutionStack.Push(type);
            try
            {
                if (_bindings.TryGetValue(type, out var binding))
                {
                    if (binding.IsSingleton && binding.Instance != null)
                    {
                        Inject(binding.Instance);
                        if (!_singletons.Contains(binding.Instance))
                        {
                            _singletons.Add(binding.Instance);
                        }
                        return binding.Instance;
                    }

                    var instance = Instantiate(binding.ImplementationType);

                    if (binding.IsSingleton)
                    {
                        binding.Instance = instance;
                        if (!_singletons.Contains(instance))
                        {
                            _singletons.Add(instance);
                        }
                    }

                    return instance;
                }

                if (_parent != null)
                {
                    return _parent.Resolve(type);
                }

                if (type.IsInterface || type.IsAbstract)
                {
                    throw new Exception($"Cannot resolve interface or abstract type {type.FullName} without a binding.");
                }

                return Instantiate(type);
            }
            finally
            {
                _resolutionStack.Pop();
            }
        }

        public T Instantiate<T>()
        {
            return (T)Instantiate(typeof(T));
        }

        public object Instantiate(Type type)
        {
            if (!_constructorCache.TryGetValue(type, out var constructor))
            {
                constructor = type.GetConstructors()
                    .OrderByDescending(c => c.GetCustomAttribute<InjectAttribute>() != null)
                    .FirstOrDefault();
                _constructorCache[type] = constructor;
            }

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
            if (instance == null || _injectedObjects.Contains(instance)) return;
            _injectedObjects.Add(instance);
            
            var type = instance.GetType();

            // Field Injection
            if (!_fieldCache.TryGetValue(type, out var fields))
            {
                fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(f => f.GetCustomAttribute<InjectAttribute>() != null)
                    .ToArray();
                _fieldCache[type] = fields;
            }

            foreach (var field in fields)
            {
                field.SetValue(instance, Resolve(field.FieldType));
            }

            // Property Injection
            if (!_propertyCache.TryGetValue(type, out var props))
            {
                props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(p => p.GetCustomAttribute<InjectAttribute>() != null)
                    .ToArray();
                _propertyCache[type] = props;
            }

            foreach (var prop in props)
            {
                prop.SetValue(instance, Resolve(prop.PropertyType));
            }

            // Method Injection
            if (!_methodCache.TryGetValue(type, out var methods))
            {
                methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(m => m.GetCustomAttribute<InjectAttribute>() != null)
                    .ToArray();
                _methodCache[type] = methods;
            }

            foreach (var method in methods)
            {
                var parameters = method.GetParameters()
                    .Select(p => Resolve(p.ParameterType))
                    .ToArray();
                method.Invoke(instance, parameters);
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

            return Bind<TFactory>().FromInstance(factory);
        }

        // Pool Registration Helpers
        public PoolBindingCondition RegisterPool<T, TPool>() where TPool : class
        {
            var poolType = typeof(TPool);
            var pool = (TPool)Activator.CreateInstance(poolType);
            Bind<TPool>().FromInstance(pool);
            return new PoolBindingCondition(pool);
        }

        public PoolBindingCondition RegisterPool<T, TPool>(T prefab) where T : UnityEngine.Object
        {
            var poolType = typeof(TPool);
            var pool = (TPool)Activator.CreateInstance(poolType);
            
            var setPrefabMethod = poolType.GetMethod("SetPrefab");
            if (setPrefabMethod != null)
                setPrefabMethod.Invoke(pool, new object[] { prefab });

            Bind<TPool>().FromInstance(pool);
            return new PoolBindingCondition(pool);
        }

        public class PoolBindingCondition
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
            // First pass: Ensure all singletons (including FromInstance) are injected and NonLazy are instantiated
            foreach (var binding in _bindings.Values.ToList())
            {
                if (binding.IsSingleton && binding.Instance != null)
                {
                    Inject(binding.Instance);
                    if (!_singletons.Contains(binding.Instance))
                    {
                        _singletons.Add(binding.Instance);
                    }
                }
                else if (binding.IsNonLazy && binding.IsSingleton && binding.Instance == null)
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

        public class Binding
        {
            public Type ContractType;
            public Type ImplementationType;
            public bool IsSingleton;
            public bool IsNonLazy;
            public object Instance;
        }

        public class BindingCondition
        {
            private readonly Binding _binding;
            private readonly DiContainer _container;

            public BindingCondition(Binding binding, DiContainer container)
            {
                _binding = binding;
                _container = container;
            }

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

            public BindingCondition Expose<T>()
            {
                _container.Bind<T>().FromInstance(_binding.Instance);
                return this;
            }
        }
    }
}
