using UnityEngine;
using Simon.Core.DI;

namespace Simon.Core.MVC
{
    internal abstract class MvcView : MonoBehaviour
    {
        public abstract void Initialize();
    }

    internal abstract class MvcController<TView, TModel> : IInitializable, IDisposable
        where TView : MvcView
    {
        public TView View { get; protected set; }
        public TModel Model { get; protected set; }

        public virtual void Setup(TView view, TModel model)
        {
            View = view;
            Model = model;
        }

        public virtual void Initialize() {}
        public virtual void Dispose() {}
    }
}
