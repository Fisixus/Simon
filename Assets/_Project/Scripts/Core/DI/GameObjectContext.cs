using UnityEngine;

namespace Simon.Core.DI
{
    // GameObject Context: Created per GameObject, child of Scene/Project
    public class GameObjectContext : ContextBase
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
