using System;
using Simon.Core.DI;

namespace Simon.Core.MVC
{
    internal interface IMvcView 
    {
        void Initialize();
    }

    internal abstract class MvcView : UnityEngine.MonoBehaviour, IMvcView
    {
        public abstract void Initialize();
    }

    internal abstract class MvcController<TViewInterface, TModel> : IInitializable, IDisposable
        where TViewInterface : class, IMvcView
    {
        [Inject] public TViewInterface View { get; protected set; }
        public TModel Model { get; protected set; }

        public virtual void Setup(TModel model)
        {
            Model = model;
        }

        public virtual void Setup(TViewInterface view, TModel model)
        {
            View = view;
            Model = model;
        }

        public void Initialize()
        {
            OnInitialize();
        }

        public void Dispose()
        {
            OnDispose();
        }

        protected virtual void OnInitialize() {}
        protected virtual void OnDispose() {}
    }
}
