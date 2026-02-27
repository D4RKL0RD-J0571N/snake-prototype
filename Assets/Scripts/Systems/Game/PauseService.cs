using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Events;

namespace SnakePrototype.Systems.Game
{
    /// <summary>
    /// Service responsible for managing the game's pause state and time scale.
    /// </summary>
    public class PauseService : IGameService
    {
        private bool _isPaused = false;
        private GameState _currentGameState = GameState.Playing;

        public void Initialize()
        {
            GameEventManager.AddListener<PauseInputEvent>(OnPauseInput);
            GameEventManager.AddListener<GameStateChangedEvent>(OnGameStateChanged);
        }

        public void Shutdown()
        {
            GameEventManager.RemoveListener<PauseInputEvent>(OnPauseInput);
            GameEventManager.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnPauseInput(PauseInputEvent e)
        {
            // Only toggle pause if in Playing or Paused state
            if (_currentGameState == GameState.Playing || _currentGameState == GameState.Paused)
            {
                TogglePause();
            }
        }

        private void OnGameStateChanged(GameStateChangedEvent e)
        {
            _currentGameState = e.NewState;
            
            // If state changes externally and isn't Paused, ensure internal flag and timescale are reset
            if (e.NewState != GameState.Paused && e.NewState != GameState.Playing)
            {
                if (_isPaused)
                {
                    _isPaused = false;
                    Time.timeScale = 1f;
                }
            }
        }

        public void TogglePause()
        {
            _isPaused = !_isPaused;
            Time.timeScale = _isPaused ? 0f : 1f;
            
            var newState = _isPaused ? GameState.Paused : GameState.Playing;
            GameEventManager.Publish(new GameStateChangedEvent(newState));
            
            Debug.Log($"Game {(_isPaused ? "Paused" : "Resumed")}");
        }

        public bool IsPaused => _isPaused;
    }
}
