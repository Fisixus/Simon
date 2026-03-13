using System;
using System.Collections.Generic;

namespace Simon.Core.Events
{
    public interface ISignal {}

    public class SignalBus
    {
        private readonly Dictionary<Type, List<Action<ISignal>>> _subscribers = new Dictionary<Type, List<Action<ISignal>>>();
        private readonly Dictionary<Delegate, Action<ISignal>> _callbackMapping = new Dictionary<Delegate, Action<ISignal>>();

        public void DeclareSignal<TSignal>() where TSignal : ISignal
        {
            if (!_subscribers.ContainsKey(typeof(TSignal)))
            {
                _subscribers[typeof(TSignal)] = new List<Action<ISignal>>();
            }
        }

        public void Subscribe<TSignal>(Action<TSignal> callback) where TSignal : ISignal
        {
            var type = typeof(TSignal);
            if (!_subscribers.ContainsKey(type))
            {
                DeclareSignal<TSignal>();
            }

            Action<ISignal> wrappedCallback = signal => callback((TSignal)signal);
            _callbackMapping[callback] = wrappedCallback;
            _subscribers[type].Add(wrappedCallback);
        }

        public void Unsubscribe<TSignal>(Action<TSignal> callback) where TSignal : ISignal
        {
            var type = typeof(TSignal);
            if (_subscribers.TryGetValue(type, out var subscribers) && _callbackMapping.TryGetValue(callback, out var wrappedCallback))
            {
                subscribers.Remove(wrappedCallback);
                _callbackMapping.Remove(callback);
            }
        }

        public void Invoke<TSignal>(TSignal signal) where TSignal : ISignal
        {
            var type = typeof(TSignal);
            if (_subscribers.TryGetValue(type, out var subscribers))
            {
                // Use a copy to avoid modification during iteration
                var subscribersCopy = new List<Action<ISignal>>(subscribers);
                foreach (var subscriber in subscribersCopy)
                {
                    subscriber?.Invoke(signal);
                }
            }
        }
    }
}
