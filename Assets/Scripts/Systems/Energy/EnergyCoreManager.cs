using System.Collections.Generic;
using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Events;
using SnakePrototype.Systems.Grid;
using SnakePrototype.Systems.Snake;

namespace SnakePrototype.Systems.Energy
{
    public class EnergyCoreManager : IGameService
    {
        private List<Vector2Int> _activeCores = new();
        private int _targetCount = 3;

        public IReadOnlyList<Vector2Int> ActiveCores => _activeCores;

        public void Initialize()
        {
            Debug.Log("EnergyCoreManager Initialized");
            GameEventManager.AddListener<SnakeMovedEvent>(OnSnakeMoved);
            GameEventManager.AddListener<GameStateChangedEvent>(OnGameStateChanged);
            GameEventManager.AddListener<RespawnEvent>(OnRespawn);
            GameEventManager.AddListener<LevelGeneratedEvent>(OnLevelGenerated);
            
            // Do not spawn immediately; wait for first level generation
        }

        private void OnLevelGenerated(LevelGeneratedEvent e)
        {
            ResetCores();
        }

        private void OnRespawn(RespawnEvent e)
        {
            ResetCores();
        }

        private void ResetCores()
        {
            GameEventManager.Publish(new EnergyCoresResetEvent());
            _activeCores.Clear();
            SpawnInitialCores();
        }

        private void SpawnInitialCores()
        {
            for (int i = 0; i < _targetCount; i++)
            {
                SpawnCore();
            }
        }

        private void SpawnCore()
        {
            var grid = ServiceLocator.Get<GridManager>();
            var snake = ServiceLocator.Get<SnakeManager>();
            
            if (grid == null) return;

            Vector2Int pos = Vector2Int.zero;
            bool found = false;
            int attempts = 0;
            while (attempts < 200)
            {
                pos = grid.GetRandomPosition();
                bool nearStart = Vector2Int.Distance(pos, new Vector2Int(5, 5)) < 3;
                if (!IsOccupied(pos) && !nearStart && (snake == null || !IsInSnake(snake.BodyParts, pos)))
                {
                    found = true;
                    break;
                }
                attempts++;
            }

            if (found)
            {
                _activeCores.Add(pos);
                GameEventManager.Publish(new EnergySpawnedEvent(pos));
            }
            else
            {
                Debug.LogWarning("EnergyCoreManager: Could not find valid spawn position for core!");
            }
        }

        private bool IsOccupied(Vector2Int pos)
        {
            var grid = ServiceLocator.Get<GridManager>();
            if (grid != null && grid.IsWall(pos.x, pos.y)) return true;
            return _activeCores.Contains(pos);
        }

        private bool IsInSnake(IReadOnlyList<Vector2Int> body, Vector2Int pos)
        {
            foreach(var part in body) if (part == pos) return true;
            return false;
        }

        private void OnSnakeMoved(SnakeMovedEvent e)
        {
            if (_activeCores.Contains(e.HeadPosition))
            {
                CollectCore(e.HeadPosition);
            }
        }

        private void CollectCore(Vector2Int pos)
        {
            _activeCores.Remove(pos);
            GameEventManager.Publish(new EnergyCollectedEvent(10, pos));
            SpawnCore();
        }

        private void OnGameStateChanged(GameStateChangedEvent e)
        {
             if (e.NewState == GameState.Playing && _activeCores.Count == 0) 
             {
                 SpawnInitialCores();
             }
        }

        public void Shutdown()
        {
            GameEventManager.RemoveListener<SnakeMovedEvent>(OnSnakeMoved);
            GameEventManager.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
            GameEventManager.RemoveListener<RespawnEvent>(OnRespawn);
            GameEventManager.RemoveListener<LevelGeneratedEvent>(OnLevelGenerated);
        }
    }
}
