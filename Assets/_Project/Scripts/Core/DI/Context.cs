using UnityEngine;

namespace Simon.Core.DI
{
    // Context: Created per scene (previously SceneContext)
    public class Context : ContextBase
    {
        private void Awake()
        {
            var parent = ProjectContext.Instance != null ? ProjectContext.Instance.Container : null;
            InstallBindings(parent);
        }
    }
}
