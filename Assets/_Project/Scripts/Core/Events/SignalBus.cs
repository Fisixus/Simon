using System;
using System.Collections.Generic;

namespace Simon.Core.Events
{
    internal interface ISignal {}

    internal class SignalBus
    {
        private readonly Dictionary<Type, List<Action<ISignal>>> _subscribers = new Dictionary<Type, List<Action<ISignal>>>();

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
            _subscribers[type].Add(signal => callback((TSignal)signal));
        }

        public void Fire<TSignal>(TSignal signal) where TSignal : ISignal
        {
            var type = typeof(TSignal);
            if (_subscribers.TryGetValue(type, out var subscribers))
            {
                foreach (var subscriber in subscribers)
                {
                    subscriber?.Invoke(signal);
                }
            }
        }
    }
}
