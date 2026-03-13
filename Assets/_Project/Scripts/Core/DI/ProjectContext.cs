using UnityEngine;

namespace Simon.Core.DI
{
    // Project Context: Root context, created once at startup
    public class ProjectContext : ContextBase
    {
        private static ProjectContext _instance;
        private bool _isInitialized;

        public static ProjectContext Instance 
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = Object.FindFirstObjectByType<ProjectContext>();
                    if (_instance == null)
                    {
                        // Try to load from Resources first
                        var prefab = Resources.Load<ProjectContext>("ProjectContext");
                        if (prefab != null)
                        {
                            _instance = Object.Instantiate(prefab);
                            _instance.name = "ProjectContext";
                        }
                        else
                        {
                            var go = new GameObject("ProjectContext");
                            _instance = go.AddComponent<ProjectContext>();
                        }
                    }
                }
                _instance.EnsureInitialized();
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
            transform.SetParent(null);
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
}
