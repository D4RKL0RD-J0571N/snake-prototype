using System.Collections.Generic;
using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Events;
using SnakePrototype.Utils;

namespace SnakePrototype.Systems.Environment
{
    /// <summary>
    /// Centralized object pooling manager for all game objects
    /// </summary>
    public class PoolingManager : IGameService
    {
        private Dictionary<string, MonoBehaviourPool<MonoBehaviour>> _pools = new Dictionary<string, MonoBehaviourPool<MonoBehaviour>>();
        private Transform _poolContainer;

        public void Initialize()
        {
            Debug.Log("PoolingManager Initialized");

            // Create container for pooled objects
            _poolContainer = new GameObject("PoolContainer").transform;

            // Initialize common pools
            InitializeVFXPools();
            InitializeUIPools();

            GameEventManager.AddListener<LevelStartedEvent>(OnLevelStarted);
            GameEventManager.AddListener<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void InitializeVFXPools()
        {
            // Create pools for common VFX prefabs
            // These would be configured via ScriptableObject or inspector in a full implementation
            Debug.Log("VFX pools initialized");
        }

        private void InitializeUIPools()
        {
            // Create pools for common UI elements
            Debug.Log("UI pools initialized");
        }

        public MonoBehaviourPool<T> GetPool<T>(GameObject prefab, int maxSize = 50) where T : MonoBehaviour
        {
            string poolKey = $"{prefab.name}_{typeof(T).Name}";

            if (!_pools.ContainsKey(poolKey))
            {
                var pool = new MonoBehaviourPool<T>(prefab, _poolContainer, maxSize);
                _pools[poolKey] = pool as MonoBehaviourPool<MonoBehaviour>;

                // Warm the pool with a few objects
                if (pool is MonoBehaviourPool<T> typedPool)
                {
                    typedPool.Warm(Mathf.Min(5, maxSize / 4));
                }
            }

            return _pools[poolKey] as MonoBehaviourPool<T>;
        }

        public T GetPooledObject<T>(GameObject prefab) where T : MonoBehaviour
        {
            var pool = GetPool<T>(prefab);
            return pool.Get();
        }

        public void ReleasePooledObject<T>(T item, GameObject prefab) where T : MonoBehaviour
        {
            if (item == null) return;

            string poolKey = $"{prefab.name}_{typeof(T).Name}";
            if (_pools.ContainsKey(poolKey))
            {
                var pool = _pools[poolKey] as MonoBehaviourPool<T>;
                pool?.Release(item);
            }
            else
            {
                // If no pool exists, just destroy the object
                Object.Destroy(item.gameObject);
            }
        }

        private void OnLevelStarted(LevelStartedEvent e)
        {
            // Clear pools when starting a new level to free memory
            ClearAllPools();
        }

        private void OnGameStateChanged(GameStateChangedEvent e)
        {
            if (e.NewState == GameState.MainMenu)
            {
                // Clear pools when returning to main menu
                ClearAllPools();
            }
        }

        public void ClearAllPools()
        {
            foreach (var pool in _pools.Values)
            {
                pool?.Clear();
            }
            _pools.Clear();

            Debug.Log("All object pools cleared");
        }

        public void Shutdown()
        {
            GameEventManager.RemoveListener<LevelStartedEvent>(OnLevelStarted);
            GameEventManager.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);

            ClearAllPools();

            if (_poolContainer != null)
            {
                Object.DestroyImmediate(_poolContainer.gameObject);
                _poolContainer = null;
            }
        }

        #region Statistics and Debugging
        public void LogPoolStatistics()
        {
            Debug.Log("=== Pool Statistics ===");
            foreach (var kvp in _pools)
            {
                var pool = kvp.Value;
                Debug.Log($"Pool {kvp.Key}: Active={pool.CountActive}, Inactive={pool.CountInactive}, Total={pool.CountAll}");
            }
        }

        public int GetTotalPooledObjects()
        {
            int total = 0;
            foreach (var pool in _pools.Values)
            {
                total += pool.CountAll;
            }
            return total;
        }

        public int GetTotalActiveObjects()
        {
            int total = 0;
            foreach (var pool in _pools.Values)
            {
                total += pool.CountActive;
            }
            return total;
        }
        #endregion
    }
}
