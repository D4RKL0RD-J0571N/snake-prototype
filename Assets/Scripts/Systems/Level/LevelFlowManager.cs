using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Events;
using SnakePrototype.Systems.Environment;

namespace SnakePrototype.Systems.Level
{
    /// <summary>
    /// Manages the game loop, level progression, and win conditions.
    /// </summary>
    public class LevelFlowManager : IGameService
    {
        #region Fields
        public enum FlowState { Idle, Intro, Running }
        private FlowState _state = FlowState.Idle;
        private int _currentLevel = 1;
        private int _collectedCores = 0;
        private int _currentPaletteSeed = -1;
        private LevelConfig _config;
        #endregion

        #region Initialization
        public LevelFlowManager(LevelConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            GameEventManager.AddListener<EnergyCollectedEvent>(OnEnergyCollected);
            GameEventManager.AddListener<RespawnEvent>(OnRespawn);
            GameEventManager.AddListener<LevelIntroConfirmedEvent>(OnLevelIntroConfirmed);
            GameEventManager.AddListener<ConfirmEvent>(OnConfirm);
            
            Debug.Log("LevelFlowManager Initialized.");
            
            // Start in MainMenu
            GameEventManager.Publish(new GameStateChangedEvent(GameState.MainMenu));
        }

        public void Shutdown()
        {
            GameEventManager.RemoveListener<EnergyCollectedEvent>(OnEnergyCollected);
            GameEventManager.RemoveListener<RespawnEvent>(OnRespawn);
            GameEventManager.RemoveListener<LevelIntroConfirmedEvent>(OnLevelIntroConfirmed);
            GameEventManager.RemoveListener<ConfirmEvent>(OnConfirm);
        }
        #endregion

        #region Public Methods
        public void StartLevel()
        {
            _collectedCores = 0;
            _state = FlowState.Running;
            Time.timeScale = 1f;

            // Generate seed based on level index and time
            int seed = (int)System.DateTime.UtcNow.Ticks ^ _currentLevel;
            _config.Seed = seed;

            // Notify UI and systems
            GameEventManager.Publish(new GameStateChangedEvent(GameState.Playing));
            GameEventManager.Publish(new LevelStartedEvent(_currentLevel, _config.TargetCores));

            // Rebuild level
            var generator = ServiceLocator.Get<LevelGenerator>();
            generator?.GenerateLevel(seed, _currentLevel);
        }
        #endregion

        #region Event Handlers
        private void OnEnergyCollected(EnergyCollectedEvent e)
        {
            _collectedCores++;
            if (_collectedCores >= _config.TargetCores)
            {
                CompleteLevel();
            }
        }

        private void OnRespawn(RespawnEvent e)
        {
            // When dying, we go back to intro for the CURRENT level
            PrepareNextLevel(false);
        }

        private void OnLevelIntroConfirmed(LevelIntroConfirmedEvent e)
        {
            StartLevel();
        }

        private void OnConfirm(ConfirmEvent e)
        {
            // Transition from MainMenu to Intro
            // This is a temporary debug transition until buttons are wired in UIManager
            if (_state == FlowState.Idle)
            {
                PrepareNextLevel(true);
            }
        }
        #endregion

        #region Level Logic
        private void PrepareNextLevel(bool isAdvance)
        {
            if (isAdvance && _state != FlowState.Idle) // Don't advance on first init if we want to start at 1
            {
                // _currentLevel++ is handled below
            }
            
            _state = FlowState.Intro;
            Time.timeScale = 0f;

            // Apply difficulty for the level we are about to enter
            ApplyDifficultyRamp();
            
            // Generate a deterministic seed for the palette based on level index.
            // This ensures that retries of the same level keep the same colors.
            _currentPaletteSeed = 42 + (_currentLevel * 777);

            // Generate palette for preview
            var paletteManager = ServiceLocator.Get<ColorPaletteManager>();
            paletteManager?.GeneratePalette(_config, _currentPaletteSeed);
            var palette = paletteManager?.CurrentPalette;

            // Publish completed event with NEXT info
            // For the very first level, "completed" is 0.
            GameEventManager.Publish(new LevelCompleteEvent(_currentLevel - 1, _currentLevel, _config.TargetCores, palette));
        }

        private void CompleteLevel()
        {
            Debug.Log($"<color=green>Level {_currentLevel} Complete!</color>");
            _currentLevel++;
            PrepareNextLevel(true);
        }

        private void ApplyDifficultyRamp()
        {
            // Scale parameters based on current level
            _config.TargetCores = 3 + (_currentLevel - 1) * 2;
            _config.GuardCount = 1 + (_currentLevel / 2);
            _config.WallProbability = Mathf.Clamp(0.04f + (_currentLevel * 0.01f), 0.04f, 0.20f);
            
            // Rotate themes
            _config.ColorTheme = (ColorThemeMode)((_currentLevel - 1) % 4);
        }
        #endregion
    }
}
