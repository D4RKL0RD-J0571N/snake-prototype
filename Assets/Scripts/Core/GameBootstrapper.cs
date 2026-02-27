using UnityEngine;
using SnakePrototype.Systems.Input;
using SnakePrototype.Systems.Grid;
using SnakePrototype.Systems.Snake;
using SnakePrototype.Systems.Detection;
using SnakePrototype.Systems.Score;
using SnakePrototype.Systems.UI;
using SnakePrototype.Systems.Environment;
using SnakePrototype.Systems.Game;

namespace SnakePrototype.Core
{
    /// <summary>
    /// The global entry point. Attached to a persistent GameObject in the bootstrap scene.
    /// Responsible for initializing the ServiceLocator and all GameServices.
    /// </summary>
    [DefaultExecutionOrder(-100)] // Ensure this runs before everything else
    public class GameBootstrapper : MonoBehaviour
    {
        #region Fields
        [Header("Configuration")]
        [SerializeField] private SnakePrototype.Systems.Grid.GridConfiguration _gridConfig;
        [SerializeField] private SnakePrototype.Systems.Snake.SnakeConfiguration _snakeConfig;
        [SerializeField] private SnakePrototype.Systems.Level.LevelConfig _levelConfig;
        [SerializeField] private SnakePrototype.Systems.Environment.VFXConfiguration _vfxConfig;

        [Header("UI Reference")]
        [SerializeField] private UnityEngine.UIElements.UIDocument _mainUIDocument;

        [Header("Audio References")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _musicTensionSource;
        [SerializeField] private AudioSource _musicAlertSource;
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _ambientSource;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            InitializeServices();
        }

        private void Update()
        {
            // Optional: Centralized tick system if we want to avoid MonoBehaviours in managers
            // For now, managers might hook into Unity's update via a centralized ticker or 
            // handle their own internal loops if they were MonoBehaviours (but they aren't).

            // Current approach: We might need a generic ITickable interface for services that need updates.
            // For this prototype, let's keep it simple: InputManager pushes events. 
            // SnakeManager might need a distinct Tick() called here.

            var snake = ServiceLocator.Get<SnakeManager>();
            snake?.Tick(Time.deltaTime);

            var detection = ServiceLocator.Get<DetectionManager>();
            detection?.Tick(Time.deltaTime);

            // Note: InputManager in this prototype is polling in Tick(), but we need to call it.
            var input = ServiceLocator.Get<InputManager>();
            input?.Tick();

            var lighting = ServiceLocator.Get<LightingManager>();
            lighting?.Tick(Time.unscaledDeltaTime);

            var audio = ServiceLocator.Get<AudioManager>();
            audio?.Tick(Time.unscaledDeltaTime);
        }

        private void OnDestroy()
        {
            ServiceLocator.ShutdownAll();
            GameEventManager.Clear();
        }
        #endregion

        #region Initialization
        private void InitializeServices()
        {
            Debug.Log("Bootstrapping Game Services...");

            // 1. Core Event System
            // (Static, doesn't need registration, but we might want to clear it)
            GameEventManager.Clear();

            // 2. Input (Core Dependency)
            var inputManager = new InputManager();
            ServiceLocator.Register<InputManager>(inputManager);

            // 3. Grid (World State)
            var gridManager = new GridManager(_gridConfig);
            ServiceLocator.Register<GridManager>(gridManager);

            // 4. Score System (Comprehensive scoring)
            var scoreConfiguration = CreateDefaultScoreConfiguration();
            var scoreSystem = new ScoreSystem(scoreConfiguration);
            ServiceLocator.Register<ScoreSystem>(scoreSystem);

            // 5. Score Manager (Legacy compatibility)
            var scoreManager = new ScoreManager();
            ServiceLocator.Register<ScoreManager>(scoreManager);

            // 6. Snake (Player)
            var snakeManager = new SnakeManager(_snakeConfig);
            ServiceLocator.Register<SnakeManager>(snakeManager);

            // 7. Detection (AI/Rules) - Depends on Snake
            var detectionManager = new DetectionManager();
            ServiceLocator.Register<DetectionManager>(detectionManager);

            // 7.5 Pause Service (Event-driven)
            var pauseService = new PauseService();
            ServiceLocator.Register<PauseService>(pauseService);

            // 8. UI (Visuals) - Depends on everything
            var uiManager = new UIManager(_mainUIDocument);
            ServiceLocator.Register<UIManager>(uiManager);

            // 9. Environment
            var colorPaletteManager = new ColorPaletteManager();
            ServiceLocator.Register<ColorPaletteManager>(colorPaletteManager);

            var lightingManager = new LightingManager();
            ServiceLocator.Register<LightingManager>(lightingManager);

            var audioManager = new AudioManager(_musicSource, _musicTensionSource, _musicAlertSource, _sfxSource, _ambientSource);
            ServiceLocator.Register<AudioManager>(audioManager);

            // 10. Grid Visuals (System & Service)
            var gridGO = new GameObject("GridSystem");
            var gridView = gridGO.AddComponent<GridView>();
            ServiceLocator.Register<GridView>(gridView);

            // 11. Level (Generation) - Must be after visual systems (including GridView)
            var levelGenerator = new SnakePrototype.Systems.Level.LevelGenerator(_levelConfig);
            ServiceLocator.Register<SnakePrototype.Systems.Level.LevelGenerator>(levelGenerator);

            // 12. Level Flow (Progression)
            var levelFlowManager = new SnakePrototype.Systems.Level.LevelFlowManager(_levelConfig);
            ServiceLocator.Register<SnakePrototype.Systems.Level.LevelFlowManager>(levelFlowManager);

            // 13. Energy (Collection) - Must be after Level (for walls)
            var energyManager = new SnakePrototype.Systems.Energy.EnergyCoreManager();
            ServiceLocator.Register<SnakePrototype.Systems.Energy.EnergyCoreManager>(energyManager);

            // 14. VFX (Polish)
            var vfxManager = new VFXManager(_vfxConfig);
            ServiceLocator.Register<VFXManager>(vfxManager);

            Debug.Log("All Services Initialized.");
        }
        #endregion

        #region Helper Methods
        private ScoreConfiguration CreateDefaultScoreConfiguration()
        {
            var config = ScriptableObject.CreateInstance<ScoreConfiguration>();
            // Use default values from the ScriptableObject
            return config;
        }
        #endregion
    }
}
