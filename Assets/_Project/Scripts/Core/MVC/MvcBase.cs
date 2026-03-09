using System;
using System.Collections.Generic;
using Simon.Core.DI;

namespace Simon.Core.MVC
{
    public interface IMvcView 
    {
        void Initialize();
    }

    public abstract class MvcView : UnityEngine.MonoBehaviour, IMvcView
    {
        public abstract void Initialize();
    }

    public class ReactiveProperty<T>
    {
        private T _value;
        public event Action<T> OnValueChanged;

        public T Value
        {
            get => _value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value)) return;
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }

        public ReactiveProperty(T defaultValue = default)
        {
            _value = defaultValue;
        }

        public void SetWithoutNotify(T value)
        {
            _value = value;
        }
    }

    public abstract class MvcController<TViewInterface, TModel> : IInitializable, IDisposable
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
