using UnityEngine;

namespace Simon.Core.DI
{
    public class SceneContext : ContextBase
    {
        private void Awake()
        {
            var parent = ProjectContext.Instance != null ? ProjectContext.Instance.Container : null;
            InstallBindings(parent);
        }
    }
}
