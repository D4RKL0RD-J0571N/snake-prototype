using UnityEngine;

namespace SnakePrototype.Events
{
    /// <summary>
    /// Base class for all typed game events.
    /// </summary>
    public abstract class GameEvent { }

    #region Input Events
    public class MoveInputEvent : GameEvent
    {
        public Vector2 Direction { get; }
        public MoveInputEvent(Vector2 direction) => Direction = direction;
    }

    public class PauseInputEvent : GameEvent { }
    #endregion

    #region Gameplay Events
    public class SnakeMovedEvent : GameEvent
    {
        public Vector2Int HeadPosition { get; }
        public SnakeMovedEvent(Vector2Int headPos) => HeadPosition = headPos;
    }

    public class EnergyCollectedEvent : GameEvent 
    {
        public int Value { get; }
        public Vector2Int Position { get; }
        public EnergyCollectedEvent(int value, Vector2Int pos) 
        { 
            Value = value; 
            Position = pos; 
        }
    }

    public class EnergySpawnedEvent : GameEvent
    {
        public Vector2Int Position { get; }
        public EnergySpawnedEvent(Vector2Int pos) => Position = pos;
    }

    public class EnergyCoresResetEvent : GameEvent { }

    public class ScoreChangedEvent : GameEvent
    {
        public int CurrentScore { get; }
        public ScoreChangedEvent(int score) => CurrentScore = score;
    }

    public class DetectionLevelChangedEvent : GameEvent
    {
        public float DetectionLevel { get; }
        public DetectionLevelChangedEvent(float level) => DetectionLevel = level;
    }
    
    public class DetectedEvent : GameEvent { }

    public class SnakeDiedEvent : GameEvent
    {
        public string Cause { get; }
        public Vector2Int Position { get; }
        public SnakeDiedEvent(string cause, Vector2Int position)
        {
            Cause = cause;
            Position = position;
        }
    }
    #endregion

    #region Game State Events
    public class GameStateChangedEvent : GameEvent
    {
        public GameState NewState { get; }
        public GameStateChangedEvent(GameState newState) => NewState = newState;
    }

    public class RespawnEvent : GameEvent { }

    public class LevelGeneratedEvent : GameEvent
    {
        public int Seed { get; }
        public int LevelIndex { get; }
        public LevelGeneratedEvent(int seed, int levelIndex)
        {
            Seed = seed;
            LevelIndex = levelIndex;
        }
    }

    public class LevelStartedEvent : GameEvent
    {
        public int LevelIndex { get; }
        public int TargetCores { get; }
        public LevelStartedEvent(int levelIndex, int targetCores)
        {
            LevelIndex = levelIndex;
            TargetCores = targetCores;
        }
    }

    public class LevelCompleteEvent : GameEvent
    {
        public int CompletedLevel { get; }
        public int NextLevel { get; }
        public int NextTargetCores { get; }
        public SnakePrototype.Systems.Environment.Palette NextPalette { get; }
        
        public LevelCompleteEvent(int completedLevel, int nextLevel, int targetCores, SnakePrototype.Systems.Environment.Palette palette)
        {
            CompletedLevel = completedLevel;
            NextLevel = nextLevel;
            NextTargetCores = targetCores;
            NextPalette = palette;
        }
    }

    public class LevelIntroConfirmedEvent : GameEvent { }

    public class ConfirmEvent : GameEvent { }

    public class PaletteChangedEvent : GameEvent
    {
        public SnakePrototype.Systems.Environment.Palette Palette { get; }
        public PaletteChangedEvent(SnakePrototype.Systems.Environment.Palette palette) => Palette = palette;
    }
    #endregion

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }
}
