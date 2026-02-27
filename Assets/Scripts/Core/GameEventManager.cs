using System;
using System.Collections.Generic;
using SnakePrototype.Events;
using UnityEngine;

namespace SnakePrototype.Core
{
    /// <summary>
    /// Static event bus for global game communication.
    /// Subscribes are typed to specific GameEvent subclasses.
    /// </summary>
    public static class GameEventManager
    {
        private static readonly Dictionary<Type, List<EventListener>> _subscribers = new();

        private class EventListener
        {
            public Delegate OriginalDelegate;
            public Action<GameEvent> Wrapper;
        }

        /// <summary>
        /// Subscribes a listener to a specific event type.
        /// </typeparam>
        public static void AddListener<T>(Action<T> listener) where T : GameEvent
        {
            var type = typeof(T);
            if (!_subscribers.ContainsKey(type))
            {
                _subscribers[type] = new List<EventListener>();
            }

            // Check if already subscribed to prevent duplicates
            if (_subscribers[type].Exists(l => l.OriginalDelegate == (Delegate)listener))
                return;

            _subscribers[type].Add(new EventListener
            {
                OriginalDelegate = listener,
                Wrapper = e => listener((T)e)
            });
        }

        /// <summary>
        /// Removes a listener using the original delegate reference.
        /// </summary>
        public static void RemoveListener<T>(Action<T> listener) where T : GameEvent
        {
            var type = typeof(T);
            if (!_subscribers.TryGetValue(type, out var listeners)) return;

            var index = listeners.FindIndex(l => l.OriginalDelegate == (Delegate)listener);
            if (index != -1)
            {
                listeners.RemoveAt(index);
            }
        }

        /// <summary>
        /// Publishes an event to all subscribers.
        /// </summary>
        public static void Publish(GameEvent gameEvent)
        {
            var type = gameEvent.GetType();
            if (_subscribers.TryGetValue(type, out var listeners))
            {
                // Use a copy to avoid modification during iteration
                var listenersCopy = new List<EventListener>(listeners);
                foreach (var listener in listenersCopy)
                {
                    try
                    {
                        listener.Wrapper?.Invoke(gameEvent);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error processing event {type.Name}: {e}");
                    }
                }
            }
        }

        /// <summary>
        /// Clears all subscribers.
        /// </summary>
        public static void Clear()
        {
            _subscribers.Clear();
        }
    }
}
