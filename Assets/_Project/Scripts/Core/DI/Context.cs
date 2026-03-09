using System.Collections.Generic;
using UnityEngine;

namespace Simon.Core.DI
{
    internal abstract class ContextBase : MonoBehaviour
    {
        [SerializeField] protected List<MonoInstaller> _monoInstallers = new List<MonoInstaller>();
        [SerializeField] protected List<ScriptableObjectInstaller> _scriptableObjectInstallers = new List<ScriptableObjectInstaller>();
        [SerializeField] protected List<PrefabInstaller> _prefabInstallers = new List<PrefabInstaller>();
        
        // Plain C# installers (non-Mono)
        protected List<Installer> _installers = new List<Installer>();

        public DiContainer Container { get; protected set; }
        public bool HasViewer;

        protected virtual void InstallBindings(DiContainer parent = null)
        {
            Container = new DiContainer(parent);
            Container.Bind<DiContainer>().FromInstance(Container);

            // 1. Process Prefab Installers (Instantiate and Install)
            foreach (var prefab in _prefabInstallers)
            {
                if (prefab == null) continue;
                var instance = Instantiate(prefab, transform);
                Container.Inject(instance);
                instance.InstallBindings();
            }

            // 2. Process Mono Installers
            foreach (var mono in _monoInstallers)
            {
                if (mono == null) continue;
                Container.Inject(mono);
                mono.InstallBindings();
            }

            // 3. Process Scriptable Object Installers
            foreach (var so in _scriptableObjectInstallers)
            {
                if (so == null) continue;
                Container.Inject(so);
                so.InstallBindings();
            }

            // 4. Process C# Installers
            foreach (var installer in _installers)
            {
                Container.Inject(installer);
                installer.InstallBindings();
            }

            Container.Initialize();
        }

        protected virtual void OnDestroy()
        {
            Container?.Dispose();
        }
    }

    // Project Context: Root context, created once at startup (includes Bootstrap logic for now)
    internal class ProjectContext : ContextBase
    {
        private static ProjectContext _instance;
        private bool _isInitialized;

        public static ProjectContext Instance 
        {
            get 
            {
                if (_instance == null) _instance = FindFirstObjectByType<ProjectContext>();
                if (_instance != null) _instance.EnsureInitialized();
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            EnsureInitialized();
        }

        private void EnsureInitialized()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            InstallBindings();
        }
    }

    // Context: Created per scene (previously SceneContext)
    internal class Context : ContextBase
    {
        private void Awake()
        {
            var parent = ProjectContext.Instance != null ? ProjectContext.Instance.Container : null;
            InstallBindings(parent);
        }
    }

    // GameObject Context: Created per GameObject, child of Scene/Project
    internal class GameObjectContext : ContextBase
    {
        private void Awake()
        {
            // Find first parent context in hierarchy
            ContextBase parentContext = null;
            Transform current = transform.parent;
            while (current != null)
            {
                parentContext = current.GetComponentInParent<ContextBase>();
                if (parentContext != null) break;
                current = current.parent;
            }

            var parentContainer = parentContext != null ? parentContext.Container : 
                                 (Object.FindFirstObjectByType<Context>()?.Container ?? 
                                  ProjectContext.Instance?.Container);

            InstallBindings(parentContainer);
        }
    }
}
