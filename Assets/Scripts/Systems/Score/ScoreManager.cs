using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Events;


namespace SnakePrototype.Systems.Score
{
    /// <summary>
    /// Legacy ScoreManager - now delegates to ScoreSystem for comprehensive scoring
    /// </summary>
    public class ScoreManager : IGameService
    {
        private ScoreSystem _scoreSystem;
        private int _currentScore = 0;

        public int CurrentScore => _currentScore;

        public void Initialize()
        {
            Debug.Log("ScoreManager Initialized (Legacy Mode)");

            // Get the comprehensive score system
            _scoreSystem = ServiceLocator.Get<ScoreSystem>();

            if (_scoreSystem != null)
            {
                // Subscribe to score changes from the main scoring system
                GameEventManager.AddListener<ScoreChangedEvent>(OnScoreChanged);
                GameEventManager.AddListener<RespawnEvent>(OnRespawn);
            }
            else
            {
                // Fallback to basic scoring if ScoreSystem not available
                Debug.LogWarning("ScoreSystem not found, using basic scoring");
                GameEventManager.AddListener<EnergyCollectedEvent>(OnEnergyCollected);
                GameEventManager.AddListener<RespawnEvent>(OnRespawn);
            }
        }

        private void OnEnergyCollected(EnergyCollectedEvent e)
        {
            // Only used if ScoreSystem is not available
            if (_scoreSystem == null)
            {
                _currentScore += e.Value;
                GameEventManager.Publish(new ScoreChangedEvent(_currentScore));
            }
        }

        private void OnScoreChanged(ScoreChangedEvent e)
        {
            _currentScore = e.CurrentScore;
        }

        private void OnRespawn(RespawnEvent e)
        {
            Reset();
        }

        public void Reset()
        {
            if (_scoreSystem != null)
            {
                _scoreSystem.ResetCurrentGame();
            }
            else
            {
                _currentScore = 0;
                GameEventManager.Publish(new ScoreChangedEvent(_currentScore));
            }
        }

        public void Shutdown()
        {
            GameEventManager.RemoveListener<ScoreChangedEvent>(OnScoreChanged);
            GameEventManager.RemoveListener<RespawnEvent>(OnRespawn);

            if (_scoreSystem == null)
            {
                GameEventManager.RemoveListener<EnergyCollectedEvent>(OnEnergyCollected);
            }
        }
    }
}
