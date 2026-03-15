using System;
using System.Collections;
using UnityEngine;

namespace Simon.Core.MVC
{
    public class ViewOperator
    {
        public event Action OnStartedOpening;
        public event Action OnCompletedOpening;
        public event Action OnStartedClosing;
        public event Action OnCompletedClosing;
        public event Action<float> WhileOpening;
        public event Action<float> WhileClosing;

        public float OpeningDuration;
        public float ClosingDuration;

        public float Progress { get; private set; } = 0f;
        public ViewState State { get; private set; } = ViewState.Closed;
        
        private Coroutine _updateViewCoroutine;
        internal event Action ResetViewAction;

        internal void Open(MonoBehaviour host)
        {
            if (State is ViewState.Opened or ViewState.Opening) return;
            
            State = ViewState.Opening;
            OnStartedOpening?.Invoke();
            
            if (OpeningDuration <= 0)
            {
                StopAnimation(host);
                CompleteOpening();
                return;
            }
            
            _updateViewCoroutine ??= host.StartCoroutine(Animate(host));
        }
        
        internal void OpenImmediately(MonoBehaviour host)
        {
            if (State is ViewState.Opened) return;
            
            if (State != ViewState.Opening)
            {
                State = ViewState.Opening;
                OnStartedOpening?.Invoke();
            }

            StopAnimation(host);
            CompleteOpening();
        }
        
        private void CompleteOpening()
        {
            Progress = 1;
            WhileOpening?.Invoke(Progress);
            State = ViewState.Opened;
            OnCompletedOpening?.Invoke();
        }

        internal void Close(MonoBehaviour host)
        {
            if (State is ViewState.Closed or ViewState.Closing) return;
            
            State = ViewState.Closing;
            OnStartedClosing?.Invoke();
            
            if (ClosingDuration <= 0)
            {
                StopAnimation(host);
                CompleteClosing();
                return;
            }
            
            _updateViewCoroutine ??= host.StartCoroutine(Animate(host));
        }

        internal void CloseImmediately(MonoBehaviour host)
        {
            if (State is ViewState.Closed) return;
            
            if (State != ViewState.Closing)
            {
                State = ViewState.Closing;
                OnStartedClosing?.Invoke();
            }
            
            StopAnimation(host);
            CompleteClosing();
        }

        private void StopAnimation(MonoBehaviour host)
        {
            if (_updateViewCoroutine != null)
            {
                host.StopCoroutine(_updateViewCoroutine);
                _updateViewCoroutine = null;
            }
        }

        private void CompleteClosing()
        {
            Progress = 0;
            WhileClosing?.Invoke(Progress);
            State = ViewState.Closed;
            OnCompletedClosing?.Invoke();
            ResetViewAction?.Invoke();
            ResetViewAction = null;
        }

        private IEnumerator Animate(MonoBehaviour host)
        {
            while (true)
            {
                if (State == ViewState.Opening)
                {
                    if (Progress >= 1)
                    {
                        CompleteOpening();
                        _updateViewCoroutine = null;
                        yield break;
                    }

                    WhileOpening?.Invoke(Progress);
                    yield return null;
                    
                    if (OpeningDuration <= 0)
                    {
                        CompleteOpening();
                        _updateViewCoroutine = null;
                        yield break;
                    }
                    
                    Progress = Mathf.Clamp01(Progress + Time.deltaTime / OpeningDuration);
                }
                else if (State == ViewState.Closing)
                {
                    if (Progress <= 0)
                    {
                        CompleteClosing();
                        _updateViewCoroutine = null;
                        yield break;
                    }

                    WhileClosing?.Invoke(Progress);
                    yield return null;

                    if (ClosingDuration <= 0)
                    {
                        CompleteClosing();
                        _updateViewCoroutine = null;
                        yield break;
                    }
                    
                    Progress = Mathf.Clamp01(Progress - Time.deltaTime / ClosingDuration);
                }
                else
                {
                    _updateViewCoroutine = null;
                    yield break;
                }
            }
        }
    }
}
