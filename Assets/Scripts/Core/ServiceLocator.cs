using System;
using System.Collections.Generic;
using UnityEngine;

namespace SnakePrototype.Core
{
    /// <summary>
    /// Simple Service Locator for decoupling systems.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, IGameService> _services = new();

        /// <summary>
        /// Registers a service instance.
        /// </summary>
        public static void Register<T>(T service) where T : IGameService
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"Service {type.Name} is already registered. Overwriting.");
                _services[type].Shutdown();
            }

            _services[type] = service;
            service.Initialize();
            Debug.Log($"Service Registered: {type.Name}");
        }

        /// <summary>
        /// Retrieves a registered service.
        /// </summary>
        public static T Get<T>() where T : class, IGameService
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var service))
            {
                return service as T;
            }

            Debug.LogError($"Service {type.Name} not found!");
            return null;
        }

        /// <summary>
        /// Clears and shuts down all services.
        /// </summary>
        public static void ShutdownAll()
        {
            foreach (var service in _services.Values)
            {
                service.Shutdown();
            }
            _services.Clear();
        }
    }
}
