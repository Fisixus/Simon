using System.Collections.Generic;
using UnityEngine;

namespace Simon.Core.DI
{
    public abstract class ContextBase : MonoBehaviour
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

            // 0. Auto-discover MonoInstallers if list is empty
            if (_monoInstallers.Count == 0)
            {
                _monoInstallers.AddRange(GetComponents<MonoInstaller>());
            }

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
}
