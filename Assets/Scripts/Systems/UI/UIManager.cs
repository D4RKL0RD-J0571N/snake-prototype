using UnityEngine;
using UnityEngine.UIElements;
using SnakePrototype.Core;
using SnakePrototype.Events;
using SnakePrototype.Systems.Environment;

namespace SnakePrototype.Systems.UI
{
    public class UIManager : IGameService
    {
        private UIDocument _document;
        private VisualElement _root;
        private Label _scoreLabel;
        private ProgressBar _energyBar;
        private VisualElement _alertOverlay;
        private Label _alertHeader;
        private Label _alertSubHeader;
        private Button _respawnButton;
        private Button _quitButton;

        // Phase 1: Main Menu
        private VisualElement _mainMenu;
        private Button _menuPlayButton;
        private Button _menuHighscoresButton;
        private Button _menuQuitButton;

        // Phase 3 Alert Meter
        private VisualElement _alertMeterRoot;
        private VisualElement _meterFill;
        private Label _detectionValueLabel;
        private Label _statusTipLabel;

        private Label _progressLabel;
        private int _currentTargetCores;
        private int _currentCollectedCores;

        // Phase 4: Level Intro
        private VisualElement _levelIntro;
        private Label _introTitle;
        private Label _introGoal;
        private Label _introDifficulty;
        private VisualElement _paletteRow;
        private Button _beginButton;

        private GameState _currentState = GameState.Playing;

        // UI Scaling
        private float _scaleFactor = 1.0f;
        private Vector2 _referenceResolution = new Vector2(1920, 1080);

        public UIManager(UIDocument document)
        {
            _document = document;
        }

        public void Initialize()
        {
            Debug.Log("UIManager Initialized");
            if (_document != null)
            {
                _root = _document.rootVisualElement;
                InitHUD();

                // Calculate scale factor for responsive UI
                CalculateScaleFactor();
                ApplyUIScaling();
            }

            // Unhide UI if re-initializing
            if (_root != null) _root.style.display = DisplayStyle.Flex;

            GameEventManager.AddListener<ScoreChangedEvent>(OnScoreChanged);
            GameEventManager.AddListener<DetectedEvent>(OnDetected);
            GameEventManager.AddListener<DetectionLevelChangedEvent>(OnDetectionChanged);
            GameEventManager.AddListener<GameStateChangedEvent>(OnGameStateChanged);
            GameEventManager.AddListener<PaletteChangedEvent>(OnPaletteChanged);
            GameEventManager.AddListener<LevelCompleteEvent>(OnLevelComplete);
            GameEventManager.AddListener<LevelStartedEvent>(OnLevelStarted);
            GameEventManager.AddListener<EnergyCollectedEvent>(OnEnergyCollected);
        }

        private void CalculateScaleFactor()
        {
            // Calculate scale factor based on screen resolution
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // Use the smaller dimension to ensure UI fits
            float scaleX = screenWidth / _referenceResolution.x;
            float scaleY = screenHeight / _referenceResolution.y;
            _scaleFactor = Mathf.Min(scaleX, scaleY);

            // Clamp scale factor to reasonable range
            _scaleFactor = Mathf.Clamp(_scaleFactor, 0.5f, 2.0f);

            Debug.Log($"UI Scale Factor: {_scaleFactor:F2} (Screen: {screenWidth}x{screenHeight})");
        }

        private void ApplyUIScaling()
        {
            if (_root == null) return;

            // Apply scaling to root container
            var hudContainer = _root.Q<VisualElement>("HUDContainer");
            if (hudContainer != null)
            {
                hudContainer.style.scale = new Vector2(_scaleFactor, _scaleFactor);

                // Adjust font sizes for better readability at different scales
                float fontSize = 16 * _scaleFactor;
                hudContainer.style.fontSize = fontSize;
            }

            // Scale alert overlay
            if (_alertOverlay != null)
            {
                _alertOverlay.style.scale = new Vector2(_scaleFactor, _scaleFactor);
            }

            // Scale level intro
            if (_levelIntro != null)
            {
                _levelIntro.style.scale = new Vector2(_scaleFactor, _scaleFactor);
            }
        }

        private void InitHUD()
        {
            _scoreLabel = _root?.Q<Label>("ScoreLabel");
            _energyBar = _root?.Q<ProgressBar>("EnergyBar");
            _alertOverlay = _root?.Q<VisualElement>("AlertOverlay");
            _alertHeader = _alertOverlay?.Q<Label>("AlertHeader");
            _alertSubHeader = _alertOverlay?.Q<Label>("AlertSubHeader");

            _respawnButton = _alertOverlay?.Q<Button>("RespawnButton");
            _quitButton = _alertOverlay?.Q<Button>("QuitButton");

            // Phase 3 Alert Meter
            _alertMeterRoot = _root?.Q<VisualElement>("AlertMeterContainer");
            _meterFill = _alertMeterRoot?.Q<VisualElement>("MeterFill");
            _detectionValueLabel = _alertMeterRoot?.Q<Label>("DetectionValue");
            _statusTipLabel = _alertMeterRoot?.Q<Label>("StatusText");

            _progressLabel = _root?.Q<Label>("ProgressLabel");

            // Phase 4 Intro screen
            _levelIntro = _root?.Q<VisualElement>("LevelIntro");
            _introTitle = _levelIntro?.Q<Label>("IntroTitle");
            _introGoal = _levelIntro?.Q<Label>("IntroGoal");
            _introDifficulty = _levelIntro?.Q<Label>("IntroDifficulty");
            _paletteRow = _levelIntro?.Q<VisualElement>("PalettePreview");
            _beginButton = _levelIntro?.Q<Button>("BeginButton");

            // Main Menu
            _mainMenu = _root?.Q<VisualElement>("MainMenu");
            _menuPlayButton = _mainMenu?.Q<Button>("PlayButton");
            _menuHighscoresButton = _mainMenu?.Q<Button>("HighscoresButton");
            _menuQuitButton = _mainMenu?.Q<Button>("QuitButton");

            if (_respawnButton != null) _respawnButton.clicked += OnRespawnClicked;
            if (_quitButton != null) _quitButton.clicked += OnQuitClicked;
            if (_beginButton != null) _beginButton.clicked += OnBeginClicked;

            if (_menuPlayButton != null) _menuPlayButton.clicked += OnMenuPlayClicked;
            if (_menuHighscoresButton != null) _menuHighscoresButton.clicked += OnMenuHighscoresClicked;
            if (_menuQuitButton != null) _menuQuitButton.clicked += OnQuitClicked;
        }

        private void OnBeginClicked()
        {
            if (_levelIntro != null) _levelIntro.style.display = DisplayStyle.None;
            GameEventManager.Publish(new LevelIntroConfirmedEvent());
        }

        private void OnRespawnClicked()
        {
            Debug.Log("Respawn/Resume Clicked");
            if (_currentState == GameState.Paused)
            {
                GameEventManager.Publish(new PauseInputEvent());
            }
            else
            {
                GameEventManager.Publish(new RespawnEvent());
            }
            
            if (_alertOverlay != null) _alertOverlay.style.display = DisplayStyle.None;
        }

        private void OnQuitClicked()
        {
            Debug.Log("Quit Clicked");
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private void OnMenuPlayClicked()
        {
            Debug.Log("Menu Play Clicked");
            if (_mainMenu != null) _mainMenu.style.display = DisplayStyle.None;
            // Use ConfirmEvent to trigger transition in LevelFlowManager
            GameEventManager.Publish(new ConfirmEvent());
        }

        private void OnMenuHighscoresClicked()
        {
            Debug.Log("Menu Highscores Clicked - Feature Pending");
        }

        private void OnScoreChanged(ScoreChangedEvent e)
        {
            if (_scoreLabel != null)
                _scoreLabel.text = $"SCORE: {e.CurrentScore}";
        }

        private void OnDetectionChanged(DetectionLevelChangedEvent e)
        {
            float level = e.DetectionLevel;

            // Hide alert meter if zero
            if (_alertMeterRoot != null)
            {
                _alertMeterRoot.style.display = level > 0 ? DisplayStyle.Flex : DisplayStyle.None;
            }

            // Update basic bar (fallback)
            if (_energyBar != null)
            {
                _energyBar.value = level;
                _energyBar.title = $"DETECTION: {(int)level}%";
                _energyBar.style.display = level > 0 ? DisplayStyle.Flex : DisplayStyle.None;
            }

            // Update Phase 3 Premium Meter
            if (_alertMeterRoot != null && level > 0)
            {
                if (_meterFill != null) _meterFill.style.width = Length.Percent(level);
                if (_detectionValueLabel != null) _detectionValueLabel.text = $"{(int)level}%";

                // Update Status Text
                if (_statusTipLabel != null)
                {
                    if (level >= 80) _statusTipLabel.text = "CRITICAL BREACH!";
                    else if (level >= 40) _statusTipLabel.text = "SNAKE SPOTTED";
                    else _statusTipLabel.text = "SCANNING...";
                }

                // Update CSS classes for color transitions
                _alertMeterRoot.RemoveFromClassList("alert-warning");
                _alertMeterRoot.RemoveFromClassList("alert-danger");

                if (level >= 80) _alertMeterRoot.AddToClassList("alert-danger");
                else if (level >= 40) _alertMeterRoot.AddToClassList("alert-warning");
            }
        }

        private void OnDetected(DetectedEvent e)
        {
            if (_alertOverlay != null)
            {
                _alertOverlay.style.display = DisplayStyle.Flex;
            }
        }

        private void OnGameStateChanged(GameStateChangedEvent e)
        {
            _currentState = e.NewState;

            if (e.NewState == GameState.MainMenu)
            {
                if (_mainMenu != null) _mainMenu.style.display = DisplayStyle.Flex;
                if (_alertOverlay != null) _alertOverlay.style.display = DisplayStyle.None;
                if (_levelIntro != null) _levelIntro.style.display = DisplayStyle.None;
            }

            if (e.NewState == GameState.Paused)
            {
                if (_alertOverlay != null)
                {
                    _alertOverlay.style.display = DisplayStyle.Flex;
                    if (_alertHeader != null) _alertHeader.text = "PAUSED";
                    if (_alertSubHeader != null) _alertSubHeader.text = "MISSION ON HOLD";
                    if (_respawnButton != null) _respawnButton.text = "RESUME";
                }
            }
            if (e.NewState == GameState.GameOver)
            {
                if (_alertOverlay != null)
                {
                    _alertOverlay.style.display = DisplayStyle.Flex;
                    if (_alertHeader != null) _alertHeader.text = "SYSTEM FAILURE";
                    if (_alertSubHeader != null) _alertSubHeader.text = "GAME OVER";
                    if (_respawnButton != null) _respawnButton.text = "RETRY";
                }
            }
            if (e.NewState == GameState.Playing)
            {
                if (_alertOverlay != null) _alertOverlay.style.display = DisplayStyle.None;
            }
        }

        private void OnPaletteChanged(PaletteChangedEvent e)
        {
            if (_root == null) return;
            var p = e.Palette;

            if (_scoreLabel != null) _scoreLabel.style.color = p.UIAccent;
            if (_progressLabel != null) _progressLabel.style.color = p.UIAccent;

            if (_alertMeterRoot != null)
            {
                _alertMeterRoot.style.borderBottomColor = p.AlertAccent;
                _alertMeterRoot.style.borderTopColor = p.AlertAccent;
                _alertMeterRoot.style.borderLeftColor = p.AlertAccent;
                _alertMeterRoot.style.borderRightColor = p.AlertAccent;
            }

            if (_statusTipLabel != null) _statusTipLabel.style.color = p.AlertAccent;

            var hudContainer = _root.Q<VisualElement>("HUDContainer");
            if (hudContainer != null)
            {
                Color bg = p.Background;
                bg.a = 0.8f;
                hudContainer.style.backgroundColor = bg;
            }

            // Phase 5: Direct Tinting for buttons and labels
            var buttons = _root.Query<Button>().ToList();
            foreach (var btn in buttons) btn.style.backgroundColor = p.UIAccent;

            var alertLabels = _root.Query<Label>(className: "alert-text").ToList();
            foreach (var l in alertLabels) l.style.color = p.AlertAccent;
        }

        private void OnLevelComplete(LevelCompleteEvent e)
        {
            // Show Intro Screen with details
            if (_levelIntro != null)
            {
                _levelIntro.style.display = DisplayStyle.Flex;

                if (_introTitle != null) _introTitle.text = $"SECTOR {e.NextLevel:D2}";
                if (_introGoal != null) _introGoal.text = $"GOAL: COLLECT {e.NextTargetCores} CORES";

                // Set Difficulty Label
                if (_introDifficulty != null)
                {
                    string diff = "EASY";
                    if (e.NextLevel > 10) diff = "CHAOTIC";
                    else if (e.NextLevel > 5) diff = "HARD";
                    else if (e.NextLevel > 2) diff = "MEDIUM";
                    _introDifficulty.text = $"DIFFICULTY: {diff}";
                }

                ApplyPalettePreview(_levelIntro, e.NextPalette);

                // Phase 5: Cinematic Reset/Trigger
                _levelIntro.RemoveFromClassList("active");
                _introTitle?.RemoveFromClassList("active");
                _levelIntro.AddToClassList("ui-fade-in");
                _introTitle?.AddToClassList("level-title-anim");

                _levelIntro.schedule.Execute(() =>
                {
                    _levelIntro.AddToClassList("active");
                    _introTitle?.AddToClassList("active");
                }).StartingIn(50);
            }

            // Hide normal HUD while in intro?
            var hudContainer = _root.Q<VisualElement>("HUDContainer");
            if (hudContainer != null) hudContainer.style.opacity = 0.2f;
        }

        private void ApplyPalettePreview(VisualElement root, Palette p)
        {
            if (p == null || _paletteRow == null) return;

            _paletteRow.Q<VisualElement>("Chip1").style.backgroundColor = p.GridAccent;
            _paletteRow.Q<VisualElement>("Chip2").style.backgroundColor = p.CoreAccent;
            _paletteRow.Q<VisualElement>("Chip3").style.backgroundColor = p.GuardAccent;
            _paletteRow.Q<VisualElement>("Chip4").style.backgroundColor = p.Background;
        }

        private void OnLevelStarted(LevelStartedEvent e)
        {
            _currentTargetCores = e.TargetCores;
            _currentCollectedCores = 0;

            // Restore HUD opacity
            var hudContainer = _root.Q<VisualElement>("HUDContainer");
            if (hudContainer != null) hudContainer.style.opacity = 1.0f;

            UpdateProgressUI();
        }

        private void OnEnergyCollected(EnergyCollectedEvent e)
        {
            _currentCollectedCores++;
            UpdateProgressUI();
        }

        private void UpdateProgressUI()
        {
            if (_progressLabel != null)
            {
                _progressLabel.text = $"CORES: {_currentCollectedCores} / {_currentTargetCores}";
            }
        }

        public void Shutdown()
        {
            GameEventManager.RemoveListener<ScoreChangedEvent>(OnScoreChanged);
            GameEventManager.RemoveListener<DetectedEvent>(OnDetected);
            GameEventManager.RemoveListener<DetectionLevelChangedEvent>(OnDetectionChanged);
            GameEventManager.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
            GameEventManager.RemoveListener<PaletteChangedEvent>(OnPaletteChanged);
            GameEventManager.RemoveListener<LevelCompleteEvent>(OnLevelComplete);
            GameEventManager.RemoveListener<LevelStartedEvent>(OnLevelStarted);
            GameEventManager.RemoveListener<EnergyCollectedEvent>(OnEnergyCollected);
        }
    }
}
