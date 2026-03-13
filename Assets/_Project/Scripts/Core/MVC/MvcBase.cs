using System;
using Simon.Core.DI;
using UnityEngine;

namespace Simon.Core.MVC
{
    public interface IMvcView 
    {
        void Initialize();
    }

    public abstract class MvcView : MonoBehaviour, IMvcView
    {
        public abstract void Initialize();
    }

    public abstract class ViewModel
    {
        public bool ForceOpen;
        public int QueuePriority;

        public ViewModel(bool forceOpen = false, int queuePriority = 0)
        {
            ForceOpen = forceOpen;
            QueuePriority = queuePriority;
        }
    }

    public abstract class MvcController<TViewInterface, TModel> : IInitializable, IDisposable
        where TViewInterface : class, IMvcView
        where TModel : class
    {
        [Inject] protected Viewer Viewer { get; set; }
        [Inject] protected IViewSource<TViewInterface> ViewSource { get; set; }

        public TViewInterface View { get; protected set; }
        public TModel Model { get; protected set; }
        public ViewOperator ViewOperator { get; protected set; }

        public bool IsOpen => ViewOperator != null && 
                              ViewOperator.State is ViewState.Opening or ViewState.Opened or ViewState.Closing;

        public virtual void Setup(TModel model)
        {
            Model = model;
        }

        public virtual void Initialize()
        {
            OnInitialize();
        }

        public virtual void Open(TModel model)
        {
            Setup(model);

            switch (ViewOperator)
            {
                case null:
                case { State: ViewState.Closed }:
                    break;
                case { State: ViewState.Closing }:
                    ViewOperator.Open(Viewer);
                    return;
                case { State: ViewState.Opening or ViewState.Opened }:
                    return;
            }

            ViewOperator = new ViewOperator();
            
            // Default implementation: spawn on viewer's rect transform
            View = ViewSource.Spawn(Viewer.Transform);
            
            OnOpen();
            
            ViewOperator.ResetViewAction += Reset;
            
            // Note: If we had a MenuPopupQueue, we would use it here.
            // For now, we open immediately.
            ViewOperator.Open(Viewer);
        }

        public virtual void Close(bool immediate = false)
        {
            if (ViewOperator == null) return;

            if (immediate)
                ViewOperator.CloseImmediately(Viewer);
            else
                ViewOperator.Close(Viewer);
            
            OnClose();
        }

        protected virtual void Reset()
        {
            OnReset();
            
            if (View != null)
            {
                ViewSource.Despawn(View);
                View = null;
            }
            
            ViewOperator = null;
        }

        public virtual void Dispose()
        {
            if (IsOpen) Close(true);
            OnDispose();
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnOpen() { }
        protected virtual void OnClose() { }
        protected virtual void OnReset() { }
        protected virtual void OnDispose() { }
    }
}
