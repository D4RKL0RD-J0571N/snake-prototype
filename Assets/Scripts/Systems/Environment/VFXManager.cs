using System.Collections.Generic;
using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Events;
using SnakePrototype.Systems.Grid;
using SnakePrototype.Utils;

namespace SnakePrototype.Systems.Environment
{
    public class VFXManager : IGameService
    {
        private VFXConfiguration _config;
        private Transform _vfxContainer;
        private PoolingManager _poolingManager;

        // Object pools for different VFX types
        private MonoBehaviourPool<MonoBehaviour> _energyPickupPool;
        private MonoBehaviourPool<MonoBehaviour> _energyRipplePool;
        private MonoBehaviourPool<MonoBehaviour> _snakeDeathPool;
        private MonoBehaviourPool<MonoBehaviour> _levelCompletePool;

        public VFXManager(VFXConfiguration config)
        {
            _config = config;
        }

        public void Initialize()
        {
            Debug.Log("VFXManager Initialized");
            _vfxContainer = new GameObject("VFXContainer").transform;

            // Get or create pooling manager
            _poolingManager = ServiceLocator.Get<PoolingManager>();
            if (_poolingManager == null)
            {
                _poolingManager = new PoolingManager();
                _poolingManager.Initialize();
                ServiceLocator.Register<PoolingManager>(_poolingManager);
            }

            // Initialize pools if we have prefabs
            if (_config != null)
            {
                InitializePools();
            }

            GameEventManager.AddListener<EnergyCollectedEvent>(OnEnergyCollected);
            GameEventManager.AddListener<SnakeDiedEvent>(OnSnakeDied);
            GameEventManager.AddListener<LevelCompleteEvent>(OnLevelComplete);
        }

        private void InitializePools()
        {
            if (_config.EnergyPickupVFX != null)
            {
                _energyPickupPool = _poolingManager.GetPool<MonoBehaviour>(_config.EnergyPickupVFX, 20);
            }

            if (_config.EnergyCollectionRipple != null)
            {
                _energyRipplePool = _poolingManager.GetPool<MonoBehaviour>(_config.EnergyCollectionRipple, 10);
            }

            if (_config.SnakeDeathVFX != null)
            {
                _snakeDeathPool = _poolingManager.GetPool<MonoBehaviour>(_config.SnakeDeathVFX, 5);
            }

            if (_config.LevelCompleteVFX != null)
            {
                _levelCompletePool = _poolingManager.GetPool<MonoBehaviour>(_config.LevelCompleteVFX, 3);
            }
        }

        public void Shutdown()
        {
            GameEventManager.RemoveListener<EnergyCollectedEvent>(OnEnergyCollected);
            GameEventManager.RemoveListener<SnakeDiedEvent>(OnSnakeDied);
            GameEventManager.RemoveListener<LevelCompleteEvent>(OnLevelComplete);

            // Clean up all active VFX objects
            CleanupAllVFX();

            if (_vfxContainer != null)
            {
                GameObject.Destroy(_vfxContainer.gameObject);
                _vfxContainer = null;
            }
        }

        private void CleanupAllVFX()
        {
            // PoolingManager handles cleanup now
            if (_poolingManager != null)
            {
                _poolingManager.ClearAllPools();
            }
        }

        private void OnEnergyCollected(EnergyCollectedEvent e)
        {
            if (_config == null) return;
            SpawnVFX(_energyPickupPool, _config.EnergyPickupVFX, e.Position);
            SpawnVFX(_energyRipplePool, _config.EnergyCollectionRipple, e.Position);
        }

        private void OnSnakeDied(SnakeDiedEvent e)
        {
            if (_config == null) return;
            SpawnVFX(_snakeDeathPool, _config.SnakeDeathVFX, e.Position);
        }

        private void OnLevelComplete(LevelCompleteEvent e)
        {
            if (_config == null) return;

            // Possibly spawn at snake head or center of board
            var grid = ServiceLocator.Get<GridManager>();
            if (grid != null)
            {
                Vector2Int center = new Vector2Int(grid.Width / 2, grid.Height / 2);
                SpawnVFX(_levelCompletePool, _config.LevelCompleteVFX, center);
            }
        }

        private void SpawnVFX(MonoBehaviourPool<MonoBehaviour> pool, GameObject prefab, Vector2Int gridPos)
        {
            if (_config == null)
            {
                Debug.LogWarning("VFXManager: _config is null during SpawnVFX!");
                return;
            }

            if (prefab == null) return;

            var grid = ServiceLocator.Get<GridManager>();
            if (grid == null)
            {
                Debug.LogWarning("VFXManager: GridManager not found!");
                return;
            }

            Vector3 worldPos = grid.GetWorldPosition(gridPos);

            // Try to get from pool first
            MonoBehaviour vfxComponent = null;
            if (pool != null)
            {
                vfxComponent = pool.Get();
            }

            // If pool is empty or not available, instantiate normally
            if (vfxComponent == null)
            {
                if (_vfxContainer == null)
                {
                    _vfxContainer = new GameObject("VFXContainer_Fallback").transform;
                }

                GameObject vfx = GameObject.Instantiate(prefab, worldPos, Quaternion.identity, _vfxContainer);
                vfxComponent = vfx.GetComponent<MonoBehaviour>();
                if (vfxComponent == null)
                {
                    vfxComponent = vfx.AddComponent<MonoBehaviour>();
                }
            }
            else
            {
                // Reuse pooled object
                vfxComponent.transform.position = worldPos;
                vfxComponent.transform.rotation = Quaternion.identity;
                vfxComponent.gameObject.SetActive(true);
            }

            // Auto destroy after duration with proper cleanup
            var ps = vfxComponent.GetComponent<ParticleSystem>();
            float duration = ps != null ? ps.main.duration + ps.main.startLifetime.constantMax : _config.DefaultDuration;

            // Use coroutine for proper delayed cleanup
            MonoBehaviour coroutineRunner = _vfxContainer.GetComponent<MonoBehaviour>();
            if (coroutineRunner == null)
            {
                coroutineRunner = _vfxContainer.gameObject.AddComponent<VFXCoroutineRunner>();
            }
            coroutineRunner.StartCoroutine(DestroyVFXAfterDelay(vfxComponent, pool, prefab, duration));
        }

        private System.Collections.IEnumerator DestroyVFXAfterDelay(MonoBehaviour vfxComponent, MonoBehaviourPool<MonoBehaviour> pool, GameObject prefab, float delay)
        {
            yield return new WaitForSeconds(delay);

            // Return to pool or destroy
            if (pool != null && vfxComponent != null)
            {
                pool.Release(vfxComponent);
            }
            else if (vfxComponent != null && vfxComponent.gameObject != null)
            {
                GameObject.Destroy(vfxComponent.gameObject);
            }
        }
    }

    // Helper component for running coroutines
    public class VFXCoroutineRunner : MonoBehaviour { }
}
