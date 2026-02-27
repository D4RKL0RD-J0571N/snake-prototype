using UnityEngine;

namespace SnakePrototype.Systems.Level
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "SnakePrototype/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        [Header("Grid Dimensions")]
        public int Width = 30;
        public int Height = 30;
        public float CellSize = 1f;

        [Header("Generation Settings")]
        public int Seed = 0;
        public float WallProbability = 0.08f;

        [Header("Palette")]
        public Color BackgroundColor = Color.black;
        public Color PrimaryColor = Color.cyan;
        public Color AlertColor = Color.red;
        public Color GridColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);
        
        [Header("Audio")]
        public AudioClip AmbientLoop;
        public AudioClip MusicBase;
        public AudioClip MusicTension;
        public AudioClip MusicAlert;
        public AudioClip CollectionSFX;
        public AudioClip AlertSFX;
        public AudioClip DeathSFX;
        public AudioClip LevelStartSFX;

        [Header("Gameplay")]
        public GameObject GuardPrefab;
        public GameObject WallPrefab;
        public int InitialEnergyCores = 3;
        public int GuardCount = 2;
        public int TargetCores = 5;
        public float DifficultyMultiplier = 1.0f;
        public ColorThemeMode ColorTheme;
    }

    public enum ColorThemeMode
    {
        Preset,
        RandomHue,
        Complementary,
        HighContrast
    }
}
