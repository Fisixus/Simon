using System.Collections.Generic;
using UnityEngine;

namespace Simon.Core.DI
{
    internal abstract class ContextBase : MonoBehaviour
    {
        [SerializeField] protected List<MonoInstaller> _installers = new List<MonoInstaller>();

        public DiContainer Container { get; protected set; }

        protected virtual void InstallBindings(DiContainer parent = null)
        {
            Container = new DiContainer(parent);
            
            // Register container to itself
            Container.Bind<DiContainer>().FromInstance(Container);

            foreach (var installer in _installers)
            {
                if (installer == null) continue;
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

    internal class ProjectContext : ContextBase
    {
        private static ProjectContext _instance;
        public static ProjectContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<ProjectContext>();
                    if (_instance == null)
                    {
                        var go = new GameObject("ProjectContext");
                        _instance = go.AddComponent<ProjectContext>();
                    }
                }
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
            InstallBindings();
        }
    }

    internal class Context : ContextBase
    {
        private void Awake()
        {
            // Inherit from ProjectContext if it exists
            InstallBindings(ProjectContext.Instance.Container);
        }
    }
}
