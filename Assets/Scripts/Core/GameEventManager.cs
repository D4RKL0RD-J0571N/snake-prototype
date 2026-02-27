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
        private static readonly Dictionary<Type, List<Action<GameEvent>>> _subscribers = new();

        /// <summary>
        /// Subscribes a listener to a specific event type.
        /// </summary>
        /// <typeparam name="T">The type of GameEvent to listen for.</typeparam>
        /// <param name="listener">The callback method.</param>
        public static void AddListener<T>(Action<T> listener) where T : GameEvent
        {
            var type = typeof(T);
            if (!_subscribers.ContainsKey(type))
            {
                _subscribers[type] = new List<Action<GameEvent>>();
            }

            // Wrapper to cast the generic event back to T
            _subscribers[type].Add(e => listener((T)e));
        }

        /// <summary>
        /// Removes a listener. (Note: Simple implementation, rigorous removal might need a wrapper reference)
        /// For this prototype, we rely on scene reload clearing static state or careful management.
        /// </summary>
        // In a full prod system, we'd return a distinct token or handle delegate equality better.
        public static void RemoveListener<T>(Action<T> listener) where T : GameEvent
        {
             // Simplified for prototype: clearing specific types is often safer 
             // or just clearing all on Shutdown.
        }

        /// <summary>
        /// Publishes an event to all subscribers.
        /// </summary>
        /// <param name="gameEvent">The event instance.</param>
        public static void Publish(GameEvent gameEvent)
        {
            var type = gameEvent.GetType();
            if (_subscribers.TryGetValue(type, out var listeners))
            {
                foreach (var listener in listeners)
                {
                    try
                    {
                        listener?.Invoke(gameEvent);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error processing event {type.Name}: {e}");
                    }
                }
            }
        }

        /// <summary>
        /// Clears all subscribers. Call this on game shutdown/reload.
        /// </summary>
        public static void Clear()
        {
            _subscribers.Clear();
        }
    }
}
