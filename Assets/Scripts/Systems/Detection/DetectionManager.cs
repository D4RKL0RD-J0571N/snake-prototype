using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Events;
using SnakePrototype.Systems.Snake;

namespace SnakePrototype.Systems.Detection
{
    public class DetectionManager : IGameService
    {
        private float _currentDetectionLevel = 0f;
        private const float DECAY_RATE = 15.0f; // Increased decay for better feel
        private readonly System.Collections.Generic.Dictionary<object, float> _activeSightings = new();
        private float _stressTimer = 0f;
        private const float STRESS_THRESHOLD = 0.5f; 
        private bool _isGameOver = false;

        public float CurrentLevel => _currentDetectionLevel;

        public void Initialize()
        {
            Debug.Log("DetectionManager Initialized");
            GameEventManager.AddListener<RespawnEvent>(OnRespawn);
        }

        private void OnRespawn(RespawnEvent e)
        {
            _currentDetectionLevel = 0;
            _stressTimer = 0;
            _isGameOver = false;
        }

        public void Tick(float deltaTime)
        {
            if (_isGameOver) return;

            // Aggregate sightings (take the strongest one)
            float maxSighting = 0f;
            foreach (var sighting in _activeSightings.Values)
            {
                if (sighting > maxSighting) maxSighting = sighting;
            }

            if (maxSighting > 0)
            {
                _currentDetectionLevel += maxSighting * deltaTime;
            }
            else if (_currentDetectionLevel > 0)
            {
                _currentDetectionLevel -= DECAY_RATE * deltaTime;
            }

            _currentDetectionLevel = Mathf.Clamp(_currentDetectionLevel, 0, 100);
            GameEventManager.Publish(new DetectionLevelChangedEvent(_currentDetectionLevel));

            // Stress Threshold Logic
            if (_currentDetectionLevel >= 100)
            {
                _stressTimer += deltaTime;
                if (_stressTimer >= STRESS_THRESHOLD)
                {
                    TriggerGameOver();
                }
            }
            else
            {
                _stressTimer = 0f;
            }

            _activeSightings.Clear();
        }

        public void ReportSighting(float amount, object source = null)
        {
            if (_isGameOver) return;

            object key = source ?? this;
            if (!_activeSightings.ContainsKey(key) || amount > _activeSightings[key])
            {
                _activeSightings[key] = amount;
            }
        }

        private void TriggerGameOver()
        {
            if (_isGameOver) return;
            _isGameOver = true;
            Debug.Log("SNAKE DETECTED! SYSTEM LOCKDOWN.");
            GameEventManager.Publish(new DetectedEvent());
            GameEventManager.Publish(new GameStateChangedEvent(GameState.GameOver));
        }

        public void Shutdown() { }
    }
}
