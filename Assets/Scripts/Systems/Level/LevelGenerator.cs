using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Systems.Grid;
using SnakePrototype.Systems.UI;
using SnakePrototype.Systems.Energy;
using SnakePrototype.Events;
using SnakePrototype.Systems.Detection;
using SnakePrototype.Systems.Environment;

namespace SnakePrototype.Systems.Level
{
    public class LevelGenerator : IGameService
    {
        private LevelConfig _config;
        private Transform _levelContainer;

        public LevelGenerator(LevelConfig config)
        {
            _config = config;
            _levelContainer = new GameObject("LevelContent").transform;
        }

        public void Initialize()
        {
            if (_config == null) return;
        }

        private int _currentLevelIndex = 1;

        public void SpawnGuards(System.Random rng = null)  
        {
            var grid = ServiceLocator.Get<GridManager>();
            if (grid == null || _config.GuardPrefab == null) return;

            // Use provided RNG or fallback to Unity.Random
            int count = _config.GuardCount;
            var spawnPoints = grid.GetRandomSpawnPoints(count); 
            // Note: GridManager.GetRandomSpawnPoints currently uses Unity Random. 
            // For true determinism we should pass RNG to it.

            foreach (var p in spawnPoints)
            {
                var worldPos = grid.GetWorldPosition(p);
                var guard = GameObject.Instantiate(_config.GuardPrefab, worldPos, Quaternion.identity, _levelContainer);
                
                if (guard.GetComponent<SnakePrototype.Utils.Billboard>() == null)
                {
                    guard.AddComponent<SnakePrototype.Utils.Billboard>();
                }
                
                var paletteManager = ServiceLocator.Get<SnakePrototype.Systems.Environment.ColorPaletteManager>();
                var palette = paletteManager?.CurrentPalette;

                // Initialize patrol if it has a component
                var patrol = guard.GetComponent<PatrolAgent>();
                if (patrol != null)
                {
                    var waypoints = new System.Collections.Generic.List<Vector2Int> { p };
                    patrol.Waypoints = waypoints;
                }

                // Apply palette to guard primitives
                if (palette != null)
                {
                    var renderers = guard.GetComponentsInChildren<Renderer>();
                    var mpb = new MaterialPropertyBlock();
                    mpb.SetColor("_BaseColor", palette.GuardAccent);
                    mpb.SetColor("_Color", palette.GuardAccent);
                    mpb.SetColor("_EmissionColor", palette.GuardAccent * 1.5f);
                    
                    foreach (var r in renderers)
                    {
                        r.SetPropertyBlock(mpb);
                    }
                }
            }
        }

        public void ClearLevelContent()
        {
            if (_levelContainer == null) return;
            foreach (Transform child in _levelContainer)
            {
                if (child != null) GameObject.Destroy(child.gameObject);
            }
        }

        public void GenerateLevel(int seed, int levelIndex)
        {
            _currentLevelIndex = levelIndex;
            var grid = ServiceLocator.Get<GridManager>();
            if (grid == null) return;

            Debug.Log($"Generating Level {levelIndex} with Seed: {seed}");
            _config.Seed = seed;

            ClearLevelContent();
            grid.Resize(_config.Width, _config.Height);
            
            // 1. Data Generation (Grid State)
            var rng = new System.Random(seed);
            
            for (int x = 0; x < _config.Width; x++)
            {
                for (int y = 0; y < _config.Height; y++)
                {
                    // Keep start area clear (5,5) - extended range
                    // We need to keep the "path" to the first move clear too
                    if (Mathf.Abs(x - 5) < 3 && Mathf.Abs(y - 5) < 3)
                    {
                        grid.SetWall(x, y, false);
                        continue;
                    }
                    
                    // Force the immediate right of the spawn to be empty
                    if (x >= 5 && x <= 8 && y == 5)
                    {
                        grid.SetWall(x, y, false);
                        continue;
                    }

                    bool isWall = rng.NextDouble() < _config.WallProbability;
                    grid.SetWall(x, y, isWall);
                }
            }

            // 2. Physical Placement
            if (_config.WallPrefab == null)
            {
                Debug.LogError("LevelGenerator: Wall Prefab is NULL! Walls will be invisible but collidable!");
            }

            for (int x = 0; x < _config.Width; x++)
            {
                for (int y = 0; y < _config.Height; y++)
                {
                    if (grid.IsWall(x, y))
                    {
                        if (_config.WallPrefab != null)
                        {
                            var worldPos = grid.GetWorldPosition(new Vector2Int(x, y));
                            var wall = GameObject.Instantiate(_config.WallPrefab, worldPos, Quaternion.identity, _levelContainer);
                            
                            if (wall.GetComponent<SnakePrototype.Utils.Billboard>() == null)
                            {
                                wall.AddComponent<SnakePrototype.Utils.Billboard>();
                            }
                        }
                    }
                }
            }

            // 3. Palette and Polish
            ApplyPalette();
            SpawnGuards();
            SpawnBorderWalls();

            // 4. Notify Listeners
            GameEventManager.Publish(new LevelGeneratedEvent(seed, levelIndex)); 
        }

        private void SpawnBorderWalls()
        {
            if (_config.WallPrefab == null) return;

            // Top and Bottom Borders
            for (int x = -1; x <= _config.Width; x++)
            {
                 SpawnWall(x, -1);
                 SpawnWall(x, _config.Height);
            }

            // Left and Right Borders (avoid corners double spawn by starting 0, end Height-1)
            for (int y = 0; y < _config.Height; y++)
            {
                SpawnWall(-1, y);
                SpawnWall(_config.Width, y);
            }
        }

        private void SpawnWall(int x, int y)
        {
            var grid = ServiceLocator.Get<GridManager>();
            var worldPos = grid.GetWorldPosition(new Vector2Int(x, y));
            var wall = GameObject.Instantiate(_config.WallPrefab, worldPos, Quaternion.identity, _levelContainer);
            
            if (wall.GetComponent<SnakePrototype.Utils.Billboard>() == null)
            {
                wall.AddComponent<SnakePrototype.Utils.Billboard>();
            }
        }

        public void ApplyPalette(bool forceGenerate = false)
        {
            var paletteManager = ServiceLocator.Get<SnakePrototype.Systems.Environment.ColorPaletteManager>();
            if (forceGenerate || paletteManager?.CurrentPalette == null)
            {
                paletteManager?.GeneratePalette(_config);
            }
            else
            {
                paletteManager?.BroadcastPalette();
            }

            var audio = ServiceLocator.Get<SnakePrototype.Systems.Environment.AudioManager>();
            audio?.SetPalette(_config);
        }

        public void Shutdown() { }
    }
}
