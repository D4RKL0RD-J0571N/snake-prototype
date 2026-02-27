using System.Collections.Generic;
using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Events;
using SnakePrototype.Systems.Grid;
using SnakePrototype.Systems.Environment;

namespace SnakePrototype.Systems.Snake
{
    public class SnakeManager : IGameService
    {
        #region Fields
        private SnakeConfiguration _config;
        private List<Vector2Int> _bodyParts = new();
        private Vector2Int _currentDirection = Vector2Int.right;
        private Vector2Int _nextDirection = Vector2Int.right;
        private float _moveTimer;
        private bool _isGameOver = false;
        private bool _growNextMove = false;

        public Vector2Int HeadPosition => _bodyParts.Count > 0 ? _bodyParts[0] : Vector2Int.zero;
        public IReadOnlyList<Vector2Int> BodyParts => _bodyParts;
        #endregion

        #region Initialization
        public SnakeManager(SnakeConfiguration config)
        {
            // Ensure we have a config, even if default (though SOs should be assigned)
            _config = config;
            if (_config == null)
            {
                Debug.LogError("SnakeManager: Missing Configuration!");
            }
        }

        public void Initialize()
        {
            Debug.Log("SnakeManager Initialized");
            GameEventManager.AddListener<MoveInputEvent>(OnMoveInput);
            GameEventManager.AddListener<RespawnEvent>(OnRespawn);
            GameEventManager.AddListener<EnergyCollectedEvent>(OnEnergyCollected);
            GameEventManager.AddListener<LevelStartedEvent>(OnLevelStarted);
            ResetSnake();
        }

        public void Shutdown()
        {
            GameEventManager.RemoveListener<MoveInputEvent>(OnMoveInput);
            GameEventManager.RemoveListener<RespawnEvent>(OnRespawn);
            GameEventManager.RemoveListener<EnergyCollectedEvent>(OnEnergyCollected);
            GameEventManager.RemoveListener<LevelStartedEvent>(OnLevelStarted);
        }
        #endregion

        #region Core Logic
        private void ResetSnake()
        {
            _bodyParts.Clear();
            var startPos = new Vector2Int(5, 5);
            int length = _config != null ? _config.StartLength : 3;

            for (int i = 0; i < length; i++)
            {
                _bodyParts.Add(startPos - new Vector2Int(i, 0));
            }
            _currentDirection = Vector2Int.right;
            _nextDirection = Vector2Int.right;
            _moveTimer = 0;
            _isGameOver = false;
            _growNextMove = false;
        }

        public void Tick(float deltaTime)
        {
            if (_config == null || _isGameOver) return;

            _moveTimer += deltaTime;
            if (_moveTimer >= _config.MoveInterval)
            {
                _moveTimer = 0;
                Move();
            }
        }

        private void Move()
        {
            _currentDirection = _nextDirection;
            Vector2Int newHead = _bodyParts[0] + _currentDirection;

            var grid = ServiceLocator.Get<GridManager>();
            if (grid != null)
            {
                // Check Bounds
                if (!grid.IsBounds(newHead))
                {
                    Debug.Log($"Death by Bounds! Head: {newHead}");
                    Die("Bounds", newHead);
                    return;
                }

                // Check Walls
                if (grid.IsWall(newHead.x, newHead.y))
                {
                    Debug.Log($"Death by Wall! Head: {newHead}");
                    Die("Wall", newHead);
                    return;
                }
            }

            if (_bodyParts.Contains(newHead))
            {
                Debug.Log($"Death by Self-Collision! Head: {newHead}");
                Die("Self-Collision", newHead);
                return;
            }

            _bodyParts.Insert(0, newHead);

            if (_growNextMove)
            {
                _growNextMove = false;
                // Do not remove tail -> Snake grows
            }
            else
            {
                _bodyParts.RemoveAt(_bodyParts.Count - 1);
            }

            GameEventManager.Publish(new SnakeMovedEvent(newHead));
        }

        private void Die(string cause, Vector2Int position)
        {
            Debug.Log($"Snake Died! Cause: {cause}");
            _isGameOver = true;

            // Ensure VFXManager is available before publishing death event
            var vfxManager = ServiceLocator.Get<VFXManager>();
            if (vfxManager == null)
            {
                Debug.LogWarning("SnakeManager: VFXManager not available during death!");
            }

            GameEventManager.Publish(new SnakeDiedEvent(cause, position));
            GameEventManager.Publish(new GameStateChangedEvent(GameState.GameOver));
        }
        #endregion

        #region Event Handlers
        private void OnMoveInput(MoveInputEvent e)
        {
            Vector2 rawInput = e.Direction;
            Vector2Int inputDir = Vector2Int.zero;
            Debug.Log($"[SnakeManager] Raw Input: {rawInput}");

            if (Mathf.Abs(rawInput.x) > Mathf.Abs(rawInput.y))
            {
                inputDir = new Vector2Int(rawInput.x > 0 ? 1 : -1, 0);
            }
            else if (Mathf.Abs(rawInput.y) > 0)
            {
                inputDir = new Vector2Int(0, rawInput.y > 0 ? 1 : -1);
            }
            else
            {
                return;
            }

            HandleInput(inputDir);
        }

        public void HandleInput(Vector2Int inputDir)
        {
            if (_isGameOver) return;

            // Smart 180 Turn Logic
            if (inputDir == -_currentDirection)
            {
                Vector2Int smartTurn = Vector2Int.zero;
                if (_currentDirection == Vector2Int.left) smartTurn = Vector2Int.up;
                else if (_currentDirection == Vector2Int.up) smartTurn = Vector2Int.right;
                else if (_currentDirection == Vector2Int.right) smartTurn = Vector2Int.down;
                else if (_currentDirection == Vector2Int.down) smartTurn = Vector2Int.left;

                _nextDirection = smartTurn;
                return;
            }

            // Prevent 180 against Pending Next
            if (inputDir == -_nextDirection && _nextDirection != _currentDirection)
            {
                return;
            }

            _nextDirection = inputDir;
        }

        private void OnRespawn(RespawnEvent e)
        {
            ResetSnake();
            GameEventManager.Publish(new GameStateChangedEvent(GameState.Playing));
            GameEventManager.Publish(new SnakeMovedEvent(HeadPosition)); // Force visual update
        }

        private void OnEnergyCollected(EnergyCollectedEvent e)
        {
            _growNextMove = true;
        }

        private void OnLevelStarted(LevelStartedEvent e)
        {
            ResetSnake();
            GameEventManager.Publish(new SnakeMovedEvent(HeadPosition));
        }
        #endregion
    }
}
