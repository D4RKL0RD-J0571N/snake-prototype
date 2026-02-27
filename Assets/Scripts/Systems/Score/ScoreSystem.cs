using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Events;
using SnakePrototype.Systems.Snake;
using System.IO;
using System;
using System.Linq;

namespace SnakePrototype.Systems.Score
{
    public class ScoreSystem : IGameService
    {
        private ScoreConfiguration _config;
        private HighscoreData _highscoreData;
        private int _currentScore;
        private int _currentLevel;
        private float _levelStartTime;
        private int _energyCoresCollected;
        private int _currentLength;

        // NES-style scoring multipliers
        private float _scoreMultiplier = 1.0f;
        private int _comboCount = 0;
        private float _lastCollectionTime;

        public int CurrentScore => _currentScore;
        public int CurrentLevel => _currentLevel;
        public HighscoreEntry[] Highscores => _highscoreData.Entries ?? new HighscoreEntry[0];
        public string CurrentRank => CalculateRank(_currentScore);

        public ScoreSystem(ScoreConfiguration config)
        {
            _config = config;
        }

        public void Initialize()
        {
            Debug.Log("ScoreSystem Initialized");
            LoadHighscores();

            GameEventManager.AddListener<EnergyCollectedEvent>(OnEnergyCollected);
            GameEventManager.AddListener<SnakeMovedEvent>(OnSnakeMoved);
            GameEventManager.AddListener<LevelStartedEvent>(OnLevelStarted);
            GameEventManager.AddListener<LevelCompleteEvent>(OnLevelComplete);
            GameEventManager.AddListener<SnakeDiedEvent>(OnSnakeDied);
        }

        public void Shutdown()
        {
            SaveHighscores();

            GameEventManager.RemoveListener<EnergyCollectedEvent>(OnEnergyCollected);
            GameEventManager.RemoveListener<SnakeMovedEvent>(OnSnakeMoved);
            GameEventManager.RemoveListener<LevelStartedEvent>(OnLevelStarted);
            GameEventManager.RemoveListener<LevelCompleteEvent>(OnLevelComplete);
            GameEventManager.RemoveListener<SnakeDiedEvent>(OnSnakeDied);
        }

        private void LoadHighscores()
        {
            _highscoreData = HighscoreData.Load();
            Debug.Log($"Loaded {(_highscoreData.Entries?.Length ?? 0)} highscores via PlayerPrefs");
        }

        public void SaveHighscores()
        {
            _highscoreData.LastPlayed = DateTime.Now;
            _highscoreData.TotalGamesPlayed++;
            HighscoreData.Save(_highscoreData);
            Debug.Log($"Saved {(_highscoreData.Entries?.Length ?? 0)} highscores via PlayerPrefs");
        }


        private void OnLevelStarted(LevelStartedEvent e)
        {
            _currentLevel = e.LevelIndex;
            _currentScore = 0;
            _levelStartTime = Time.time;
            _energyCoresCollected = 0;
            _currentLength = 3; // Starting snake length
            _scoreMultiplier = 1.0f;
            _comboCount = 0;
            _lastCollectionTime = Time.time;

            GameEventManager.Publish(new ScoreChangedEvent(0));
        }

        private void OnEnergyCollected(EnergyCollectedEvent e)
        {
            // Calculate combo bonus for quick collections
            float timeSinceLastCollection = Time.time - _lastCollectionTime;
            if (timeSinceLastCollection < 2.0f) // Within 2 seconds
            {
                _comboCount++;
                _scoreMultiplier = 1.0f + (_comboCount * 0.2f);
            }
            else
            {
                _comboCount = 0;
                _scoreMultiplier = 1.0f;
            }

            _lastCollectionTime = Time.time;
            _energyCoresCollected++;

            // NES-style scoring calculation
            int baseScore = _config.EnergyCoreValue;
            int lengthBonus = _currentLength * _config.LengthMultiplier;
            int speedBonus = CalculateSpeedBonus();
            // Removed detection penalty - doesn't fit snake game design

            int totalScore = Mathf.RoundToInt((baseScore + lengthBonus + speedBonus) * _scoreMultiplier);
            totalScore = Mathf.Max(0, totalScore); // Ensure no negative scores

            _currentScore += totalScore;

            Debug.Log($"Score: +{totalScore} (Base:{baseScore} Length:{lengthBonus} Speed:{speedBonus} Multiplier:{_scoreMultiplier:F1})");

            GameEventManager.Publish(new ScoreChangedEvent(_currentScore));
        }

        private void OnSnakeMoved(SnakeMovedEvent e)
        {
            // Update current length (this would come from SnakeManager)
            var snakeManager = ServiceLocator.Get<SnakeManager>();
            if (snakeManager != null)
            {
                _currentLength = snakeManager.BodyParts.Count;
            }
        }

        private void OnLevelComplete(LevelCompleteEvent e)
        {
            float levelTime = Time.time - _levelStartTime;

            // Check if score qualifies for highscore
            CheckAndAddHighscore(_currentScore, _currentLevel, levelTime);

            Debug.Log($"Level {_currentLevel} Complete! Score: {_currentScore}, Time: {levelTime:F1}s");
        }

        private void OnSnakeDied(SnakeDiedEvent e)
        {
            float levelTime = Time.time - _levelStartTime;

            // Check if score qualifies for highscore even on death
            CheckAndAddHighscore(_currentScore, _currentLevel, levelTime);

            Debug.Log($"Game Over! Final Score: {_currentScore}, Time: {levelTime:F1}s");
        }

        private int CalculateSpeedBonus()
        {
            // Speed bonus based on collection rate
            float collectionRate = _energyCoresCollected / (Time.time - _levelStartTime + 0.001f);

            if (collectionRate > 2.0f) return _config.SpeedBonus * 2; // Very fast
            if (collectionRate > 1.5f) return _config.SpeedBonus; // Fast
            if (collectionRate > 1.0f) return _config.SpeedBonus / 2; // Normal
            return 0; // Slow
        }

        private void CheckAndAddHighscore(int score, int level, float playTime)
        {
            string rank = CalculateRank(score);
            var newEntry = new HighscoreEntry(_config.PlayerNameDefault, score, level, rank, playTime);

            if (_highscoreData.Entries == null)
            {
                _highscoreData.Entries = new HighscoreEntry[] { newEntry };
            }
            else
            {
                var entries = _highscoreData.Entries.ToList();
                entries.Add(newEntry);

                // Sort by score (descending)
                _highscoreData.Entries = entries.OrderByDescending(e => e.Score).Take(_config.MaxHighscores).ToArray();
            }
        }

        private string CalculateRank(int score)
        {
            if (!_config.UseRetroScoring) return "";

            for (int i = _config.MilestoneScores.Length - 1; i >= 0; i--)
            {
                if (score >= _config.MilestoneScores[i])
                {
                    return _config.MilestoneNames[i];
                }
            }
            return "BEGINNER";
        }

        #region Public API
        public void ResetCurrentGame()
        {
            _currentScore = 0;
            _currentLevel = 1;
            _comboCount = 0;
            _scoreMultiplier = 1.0f;

            GameEventManager.Publish(new ScoreChangedEvent(0));
        }

        public void SetPlayerName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                _config.PlayerNameDefault = name.ToUpper().PadRight(8, ' ').Substring(0, 8);
            }
        }

        public HighscoreEntry GetHighscore(int index)
        {
            if (_highscoreData.Entries != null && index < _highscoreData.Entries.Length)
            {
                return _highscoreData.Entries[index];
            }
            return null;
        }

        public bool IsNewHighscore(int score)
        {
            if (_highscoreData.Entries == null || _highscoreData.Entries.Length == 0)
                return true;

            return score > _highscoreData.Entries.Min(e => e.Score);
        }

        public void ClearHighscores()
        {
            _highscoreData = new HighscoreData();
            SaveHighscores();
            Debug.Log("Highscores cleared");
        }
        #endregion
    }
}
