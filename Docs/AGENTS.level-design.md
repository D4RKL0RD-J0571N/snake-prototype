# Level Design Agent - Specialized Documentation

## Overview

The `level-design` agent provides expertise in procedural generation, difficulty progression, environmental storytelling, and spatial design principles for the Snake Prototype project's labyrinth environments.

## Core Responsibilities

### Procedural Level Generation
- Seed-based level generation algorithms
- Balanced procedural content distribution
- Dynamic difficulty adjustment in levels
- Procedural asset placement and theming

### Spatial Design Principles
- Player flow and navigation design
- Environmental storytelling through layout
- Spatial hierarchy and landmark creation
- Sight lines and visibility considerations

### Difficulty Progression
- Level difficulty curve design
- Skill-based challenge scaling
- Player learning curve integration
- Adaptive difficulty systems

## Procedural Generation System

### Seed-Based Level Generation

#### Core Generation Algorithm
```csharp
// Procedural level generation system for Snake Prototype
public class ProceduralLevelGenerator
{
    public struct LevelGenerationSettings
    {
        public int Width;
        public int Height;
        public int Seed;
        public float WallDensity;
        public float EnergyCoreDensity;
        public int GuardCount;
        public DifficultyLevel Difficulty;
        public LevelTheme Theme;
    }
    
    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard,
        Expert
    }
    
    public enum LevelTheme
    {
        Industrial,
        Laboratory,
        ServerRoom,
        Maintenance
    }
    
    public LevelData GenerateLevel(LevelGenerationSettings settings)
    {
        // Initialize random with seed for reproducible levels
        Random.InitState(settings.Seed);
        
        var levelData = new LevelData
        {
            Width = settings.Width,
            Height = settings.Height,
            Seed = settings.Seed,
            Theme = settings.Theme
        };
        
        // Generate level components in order
        GenerateBaseLayout(levelData, settings);
        GenerateWalls(levelData, settings);
        GenerateEnergyCores(levelData, settings);
        GenerateGuards(levelData, settings);
        GenerateEnvironmentalDetails(levelData, settings);
        
        // Validate and optimize level
        ValidateLevel(levelData);
        OptimizeLevel(levelData);
        
        return levelData;
    }
    
    private void GenerateBaseLayout(LevelData levelData, LevelGenerationSettings settings)
    {
        // Create base grid structure
        levelData.Grid = new bool[settings.Width, settings.Height];
        
        // Generate main pathways
        GenerateMainPathways(levelData, settings);
        
        // Generate secondary pathways
        GenerateSecondaryPathways(levelData, settings);
        
        // Generate dead ends and ambush points
        GenerateDeadEnds(levelData, settings);
    }
    
    private void GenerateMainPathways(LevelData levelData, LevelGenerationSettings settings)
    {
        // Generate primary corridors for snake movement
        var mainPaths = new List<Vector2Int>();
        
        // Create horizontal main corridor
        for (int x = 1; x < settings.Width - 1; x++)
        {
            mainPaths.Add(new Vector2Int(x, settings.Height / 2));
        }
        
        // Create vertical main corridor
        for (int y = 1; y < settings.Height - 1; y++)
        {
            mainPaths.Add(new Vector2Int(settings.Width / 2, y));
        }
        
        // Mark main paths as clear
        foreach (var path in mainPaths)
        {
            if (IsValidPosition(path, settings))
            {
                levelData.Grid[path.x, path.y] = false; // false = clear
            }
        }
    }
    
    private void GenerateWalls(LevelData levelData, LevelGenerationSettings settings)
    {
        // Generate walls based on density and difficulty
        int wallCount = Mathf.FloorToInt(settings.Width * settings.Height * settings.WallDensity);
        
        for (int i = 0; i < wallCount; i++)
        {
            var position = GetRandomValidPosition(levelData, settings);
            
            if (ShouldPlaceWall(position, levelData, settings))
            {
                levelData.Grid[position.x, position.y] = true; // true = wall
                
                // Create wall clusters for more interesting layouts
                CreateWallCluster(position, levelData, settings);
            }
        }
    }
    
    private void CreateWallCluster(Vector2Int center, LevelData levelData, LevelGenerationSettings settings)
    {
        // Create small clusters of walls for more organic feel
        int clusterSize = Random.Range(2, 4);
        
        for (int i = 0; i < clusterSize; i++)
        {
            var offset = Random.insideUnitCircle * 2f;
            var clusterPos = new Vector2Int(
                Mathf.RoundToInt(center.x + offset.x),
                Mathf.RoundToInt(center.y + offset.y)
            );
            
            if (IsValidPosition(clusterPos, settings) && !IsMainPath(clusterPos, levelData))
            {
                levelData.Grid[clusterPos.x, clusterPos.y] = true;
            }
        }
    }
    
    private void GenerateEnergyCores(LevelData levelData, LevelGenerationSettings settings)
    {
        // Calculate energy core count based on level size and difficulty
        int coreCount = Mathf.FloorToInt(settings.Width * settings.Height * settings.EnergyCoreDensity);
        
        // Adjust for difficulty
        coreCount = Mathf.RoundToInt(coreCount * GetDifficultyMultiplier(settings.Difficulty));
        
        levelData.EnergyCores = new List<Vector2Int>();
        
        for (int i = 0; i < coreCount; i++)
        {
            var position = GetOptimalEnergyPosition(levelData, settings);
            
            if (position != Vector2Int.zero)
            {
                levelData.EnergyCores.Add(position);
                
                // Mark position as occupied
                levelData.OccupiedPositions.Add(position);
            }
        }
    }
    
    private Vector2Int GetOptimalEnergyPosition(LevelData levelData, LevelGenerationSettings settings)
    {
        // Find optimal positions for energy cores
        var candidates = new List<Vector2Int>();
        
        // Generate candidate positions
        for (int x = 1; x < settings.Width - 1; x++)
        {
            for (int y = 1; y < settings.Height - 1; y++)
            {
                var pos = new Vector2Int(x, y);
                
                if (IsValidEnergyPosition(pos, levelData, settings))
                {
                    candidates.Add(pos);
                }
            }
        }
        
        // Score candidates based on strategic value
        var scoredCandidates = candidates.Select(pos => new
        {
            Position = pos,
            Score = ScoreEnergyPosition(pos, levelData, settings)
        }).OrderByDescending(c => c.Score).ToList();
        
        return scoredCandidates.Count > 0 ? scoredCandidates[0].Position : Vector2Int.zero;
    }
    
    private float ScoreEnergyPosition(Vector2Int position, LevelData levelData, LevelGenerationSettings settings)
    {
        float score = 0f;
        
        // Distance from walls (prefer open areas)
        float wallDistance = GetDistanceToNearestWall(position, levelData);
        score += wallDistance * 0.3f;
        
        // Distance from other energy cores (spread them out)
        float coreDistance = GetDistanceToNearestEnergyCore(position, levelData);
        score += coreDistance * 0.4f;
        
        // Strategic value (near intersections or decision points)
        float strategicValue = GetStrategicValue(position, levelData);
        score += strategicValue * 0.3f;
        
        return score;
    }
}
```

#### Advanced Generation Patterns
```csharp
// Advanced procedural generation patterns
public class AdvancedGenerationPatterns
{
    public struct PatternLibrary
    {
        public Pattern[] RoomPatterns;
        public Pattern[] CorridorPatterns;
        public Pattern[] JunctionPatterns;
        public Pattern[] DeadEndPatterns;
    }
    
    public struct Pattern
    {
        public string Name;
        public bool[,] Layout;
        public Vector2Int Entrance;
        public Vector2Int Exit;
        public float Weight;
        public DifficultyLevel MinDifficulty;
    }
    
    public PatternLibrary CreatePatternLibrary()
    {
        return new PatternLibrary
        {
            RoomPatterns = CreateRoomPatterns(),
            CorridorPatterns = CreateCorridorPatterns(),
            JunctionPatterns = CreateJunctionPatterns(),
            DeadEndPatterns = CreateDeadEndPatterns()
        };
    }
    
    private Pattern[] CreateRoomPatterns()
    {
        return new Pattern[]
        {
            new Pattern
            {
                Name = "SmallSquare",
                Layout = new bool[,]
                {
                    { true, true, true, true, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, false, false, false, true },
                    { true, true, true, true, true }
                },
                Entrance = new Vector2Int(0, 2),
                Exit = new Vector2Int(4, 2),
                Weight = 1.0f,
                MinDifficulty = DifficultyLevel.Easy
            },
            
            new Pattern
            {
                Name = "LargeRectangle",
                Layout = new bool[,]
                {
                    { true, true, true, true, true, true, true },
                    { true, false, false, false, false, false, true },
                    { true, false, false, false, false, false, true },
                    { true, false, false, false, false, false, true },
                    { true, true, true, true, true, true, true }
                },
                Entrance = new Vector2Int(0, 2),
                Exit = new Vector2Int(6, 2),
                Weight = 0.8f,
                MinDifficulty = DifficultyLevel.Medium
            }
        };
    }
    
    public void ApplyPattern(LevelData levelData, Pattern pattern, Vector2Int position)
    {
        // Apply pattern to level grid
        for (int x = 0; x < pattern.Layout.GetLength(0); x++)
        {
            for (int y = 0; y < pattern.Layout.GetLength(1); y++)
            {
                var worldPos = position + new Vector2Int(x, y);
                
                if (IsValidPosition(worldPos, levelData))
                {
                    levelData.Grid[worldPos.x, worldPos.y] = pattern.Layout[x, y];
                }
            }
        }
        
        // Add pattern metadata
        levelData.AppliedPatterns.Add(new AppliedPattern
        {
            Pattern = pattern,
            Position = position,
            Rotation = Quaternion.identity
        });
    }
}
```

## Difficulty Progression System

### Adaptive Difficulty Design

#### Dynamic Difficulty Adjustment
```csharp
// Dynamic difficulty progression system
public class DifficultyProgressionManager
{
    public struct DifficultyProfile
    {
        public float WallDensity;
        public float GuardSensitivity;
        public int GuardCount;
        public float EnergyCoreScarcity;
        public float PathComplexity;
        public float TimePressure;
    }
    
    public struct PlayerPerformanceData
    {
        public float CompletionRate;
        public float AverageTime;
        public float DeathCount;
        public float ScorePerMinute;
        public float DetectionAvoidanceRate;
    }
    
    private DifficultyProfile _baseDifficulty;
    private PlayerPerformanceData _playerPerformance;
    private List<DifficultyProfile> _difficultyHistory;
    
    public DifficultyProgressionManager(DifficultyProfile baseDifficulty)
    {
        _baseDifficulty = baseDifficulty;
        _difficultyHistory = new List<DifficultyProfile>();
    }
    
    public DifficultyProfile CalculateNextLevelDifficulty(int currentLevel, PlayerPerformanceData performance)
    {
        _playerPerformance = performance;
        
        // Calculate base progression
        var progressionDifficulty = CalculateBaseProgression(currentLevel);
        
        // Adjust based on player performance
        var adjustedDifficulty = AdjustForPlayerPerformance(progressionDifficulty, performance);
        
        // Smooth difficulty changes
        var smoothedDifficulty = SmoothDifficultyChanges(adjustedDifficulty);
        
        // Validate difficulty bounds
        var validatedDifficulty = ValidateDifficultyBounds(smoothedDifficulty);
        
        // Store in history
        _difficultyHistory.Add(validatedDifficulty);
        
        return validatedDifficulty;
    }
    
    private DifficultyProfile CalculateBaseProgression(int level)
    {
        // Base difficulty increases with level
        float levelMultiplier = 1f + (level * 0.1f);
        
        return new DifficultyProfile
        {
            WallDensity = Mathf.Clamp(_baseDifficulty.WallDensity * levelMultiplier, 0.1f, 0.4f),
            GuardSensitivity = Mathf.Clamp(_baseDifficulty.GuardSensitivity * levelMultiplier, 0.5f, 1.5f),
            GuardCount = Mathf.RoundToInt(_baseDifficulty.GuardCount * (1f + level * 0.2f)),
            EnergyCoreScarcity = Mathf.Clamp(_baseDifficulty.EnergyCoreScarcity * levelMultiplier, 0.5f, 0.9f),
            PathComplexity = Mathf.Clamp(_baseDifficulty.PathComplexity * levelMultiplier, 1f, 3f),
            TimePressure = Mathf.Clamp(_baseDifficulty.TimePressure * levelMultiplier, 0f, 1f)
        };
    }
    
    private DifficultyProfile AdjustForPlayerPerformance(DifficultyProfile difficulty, PlayerPerformanceData performance)
    {
        var adjustedDifficulty = difficulty;
        
        // Adjust based on completion rate
        if (performance.CompletionRate > 0.8f)
        {
            // Player is doing well, increase difficulty
            adjustedDifficulty.WallDensity *= 1.1f;
            adjustedDifficulty.GuardSensitivity *= 1.05f;
            adjustedDifficulty.GuardCount = Mathf.RoundToInt(adjustedDifficulty.GuardCount * 1.1f);
        }
        else if (performance.CompletionRate < 0.4f)
        {
            // Player is struggling, decrease difficulty
            adjustedDifficulty.WallDensity *= 0.9f;
            adjustedDifficulty.GuardSensitivity *= 0.95f;
            adjustedDifficulty.GuardCount = Mathf.RoundToInt(adjustedDifficulty.GuardCount * 0.9f);
        }
        
        // Adjust based on death count
        if (performance.DeathCount > 3f)
        {
            // Too many deaths, make it easier
            adjustedDifficulty.EnergyCoreScarcity *= 0.8f;
            adjustedDifficulty.TimePressure *= 0.9f;
        }
        else if (performance.DeathCount < 1f)
        {
            // Too few deaths, make it harder
            adjustedDifficulty.EnergyCoreScarcity *= 1.1f;
            adjustedDifficulty.TimePressure *= 1.1f;
        }
        
        // Adjust based on score efficiency
        if (performance.ScorePerMinute < 100f)
        {
            // Low scoring, provide more opportunities
            adjustedDifficulty.EnergyCoreScarcity *= 0.9f;
        }
        
        return adjustedDifficulty;
    }
    
    private DifficultyProfile SmoothDifficultyChanges(DifficultyProfile newDifficulty)
    {
        if (_difficultyHistory.Count == 0)
            return newDifficulty;
            
        var lastDifficulty = _difficultyHistory[_difficultyHistory.Count - 1];
        var smoothingFactor = 0.3f; // 30% smoothing
        
        return new DifficultyProfile
        {
            WallDensity = Mathf.Lerp(lastDifficulty.WallDensity, newDifficulty.WallDensity, smoothingFactor),
            GuardSensitivity = Mathf.Lerp(lastDifficulty.GuardSensitivity, newDifficulty.GuardSensitivity, smoothingFactor),
            GuardCount = Mathf.RoundToInt(Mathf.Lerp(lastDifficulty.GuardCount, newDifficulty.GuardCount, smoothingFactor)),
            EnergyCoreScarcity = Mathf.Lerp(lastDifficulty.EnergyCoreScarcity, newDifficulty.EnergyCoreScarcity, smoothingFactor),
            PathComplexity = Mathf.Lerp(lastDifficulty.PathComplexity, newDifficulty.PathComplexity, smoothingFactor),
            TimePressure = Mathf.Lerp(lastDifficulty.TimePressure, newDifficulty.TimePressure, smoothingFactor)
        };
    }
}
```

### Skill-Based Challenge Scaling

#### Player Skill Assessment
```csharp
// Player skill assessment and challenge scaling
public class PlayerSkillAssessment
{
    public struct SkillMetrics
    {
        public float MovementSkill;        // Precision and efficiency of movement
        public float SpatialAwareness;      // Understanding of level layout
        public float RiskAssessment;        // Ability to evaluate risks
        public float PatternRecognition;    // Recognition of patterns and strategies
        public float AdaptationSpeed;       // Speed of adapting to new challenges
        public float OverallSkill;          // Combined skill score
    }
    
    public SkillMetrics AssessPlayerSkill(List<GameSessionData> recentSessions)
    {
        if (recentSessions.Count == 0)
            return new SkillMetrics { OverallSkill = 0.5f }; // Default to medium skill
            
        var metrics = new SkillMetrics();
        
        // Analyze movement skill
        metrics.MovementSkill = AssessMovementSkill(recentSessions);
        
        // Analyze spatial awareness
        metrics.SpatialAwareness = AssessSpatialAwareness(recentSessions);
        
        // Analyze risk assessment
        metrics.RiskAssessment = AssessRiskAssessment(recentSessions);
        
        // Analyze pattern recognition
        metrics.PatternRecognition = AssessPatternRecognition(recentSessions);
        
        // Analyze adaptation speed
        metrics.AdaptationSpeed = AssessAdaptationSpeed(recentSessions);
        
        // Calculate overall skill
        metrics.OverallSkill = CalculateOverallSkill(metrics);
        
        return metrics;
    }
    
    private float AssessMovementSkill(List<GameSessionData> sessions)
    {
        float movementEfficiency = 0f;
        float movementPrecision = 0f;
        
        foreach (var session in sessions)
        {
            // Efficiency: optimal path vs actual path
            float efficiency = session.OptimalPathLength / session.ActualPathLength;
            movementEfficiency += efficiency;
            
            // Precision: unnecessary movements and corrections
            float precision = 1f - (session.CorrectionCount / session.TotalMoves);
            movementPrecision += precision;
        }
        
        movementEfficiency /= sessions.Count;
        movementPrecision /= sessions.Count;
        
        return (movementEfficiency + movementPrecision) / 2f;
    }
    
    private float AssessSpatialAwareness(List<GameSessionData> sessions)
    {
        float awarenessScore = 0f;
        
        foreach (var session in sessions)
        {
            // How well player navigates the level
            float navigationScore = session.UniqueAreasVisited / (float)session.TotalAreas;
            
            // How often player gets stuck or lost
            float stuckScore = 1f - (session.StuckEvents / session.PlayTime);
            
            awarenessScore += (navigationScore + stuckScore) / 2f;
        }
        
        return awarenessScore / sessions.Count;
    }
    
    private float AssessRiskAssessment(List<GameSessionData> sessions)
    {
        float riskScore = 0f;
        
        foreach (var session in sessions)
        {
            // Risk vs reward decisions
            float riskRewardRatio = session.SuccessfulRisks / (session.SuccessfulRisks + session.FailedRisks);
            
            // Detection avoidance skill
            float avoidanceSkill = session.DetectionAvoidanceRate;
            
            riskScore += (riskRewardRatio + avoidanceSkill) / 2f;
        }
        
        return riskScore / sessions.Count;
    }
    
    private float AssessPatternRecognition(List<GameSessionData> sessions)
    {
        float patternScore = 0f;
        
        foreach (var session in sessions)
        {
            // How quickly player learns level patterns
            float learningSpeed = session.FirstCompletionTime / session.BestCompletionTime;
            
            // Recognition of guard patterns
            float guardPatternRecognition = session.GuardPatternSuccessRate;
            
            patternScore += (learningSpeed + guardPatternRecognition) / 2f;
        }
        
        return patternScore / sessions.Count;
    }
    
    private float AssessAdaptationSpeed(List<GameSessionData> sessions)
    {
        if (sessions.Count < 2)
            return 0.5f; // Default for insufficient data
            
        float adaptationScore = 0f;
        
        for (int i = 1; i < sessions.Count; i++)
        {
            var currentSession = sessions[i];
            var previousSession = sessions[i - 1];
            
            // Improvement from previous session
            float improvement = previousSession.BestScore > 0 ? 
                (currentSession.BestScore - previousSession.BestScore) / previousSession.BestScore : 0f;
            
            adaptationScore += Mathf.Clamp01(improvement + 0.5f); // Normalize to 0-1
        }
        
        return adaptationScore / (sessions.Count - 1);
    }
    
    private float CalculateOverallSkill(SkillMetrics metrics)
    {
        // Weight different skills based on importance
        float weightedScore = 
            metrics.MovementSkill * 0.25f +
            metrics.SpatialAwareness * 0.20f +
            metrics.RiskAssessment * 0.20f +
            metrics.PatternRecognition * 0.20f +
            metrics.AdaptationSpeed * 0.15f;
            
        return Mathf.Clamp01(weightedScore);
    }
}
```

## Environmental Storytelling

### Narrative Through Level Design

#### Environmental Narrative System
```csharp
// Environmental storytelling through level design
public class EnvironmentalStoryteller
{
    public struct NarrativeElement
    {
        public string Name;
        public string Description;
        public Vector2Int Position;
        public EnvironmentalType Type;
        public StoryBeat AssociatedStoryBeat;
        public float EmotionalImpact;
    }
    
    public enum EnvironmentalType
    {
        DamagedEquipment,
        AbandonedWorkstation,
        WarningSigns,
        EmergencyEquipment,
        PersonalEffects,
        StructuralDamage,
        PowerFailure,
        MaintenanceArea
    }
    
    public struct StoryBeat
    {
        public string Title;
        public string Description;
        public EmotionalTone Tone;
        public float Importance;
    }
    
    public enum EmotionalTone
    {
        Tension,
        Mystery,
        Danger,
        Abandonment,
        Urgency,
        Despair,
        Hope
    }
    
    private List<StoryBeat> _storyBeats;
    private Dictionary<EnvironmentalType, NarrativeElement[]> _elementTemplates;
    
    public EnvironmentalStoryteller()
    {
        InitializeStoryBeats();
        InitializeElementTemplates();
    }
    
    private void InitializeStoryBeats()
    {
        _storyBeats = new List<StoryBeat>
        {
            new StoryBeat
            {
                Title = "The Incident",
                Description = "Something went wrong in this facility. The signs of panic are everywhere.",
                Tone = EmotionalTone.Tension,
                Importance = 1.0f
            },
            new StoryBeat
            {
                Title = "Evacuation",
                Description = "People left in a hurry. Personal items were left behind.",
                Tone = EmotionalTone.Urgency,
                Importance = 0.8f
            },
            new StoryBeat
            {
                Title = "System Failure",
                Description = "The automated systems are malfunctioning. Danger lurks in the darkness.",
                Tone = EmotionalTone.Danger,
                Importance = 0.9f
            },
            new StoryBeat
            {
                Title = "Lost Hope",
                Description = "The facility was abandoned. No one came back.",
                Tone = EmotionalTone.Despair,
                Importance = 0.7f
            }
        };
    }
    
    public void GenerateEnvironmentalNarrative(LevelData levelData, int storyProgress)
    {
        // Select appropriate story beats for current progress
        var activeBeats = GetActiveStoryBeats(storyProgress);
        
        // Place narrative elements throughout the level
        foreach (var beat in activeBeats)
        {
            PlaceNarrativeElements(levelData, beat);
        }
        
        // Create environmental connections between elements
        CreateEnvironmentalConnections(levelData);
        
        // Ensure narrative flow through level
        OptimizeNarrativeFlow(levelData);
    }
    
    private List<StoryBeat> GetActiveStoryBeats(int progress)
    {
        var activeBeats = new List<StoryBeat>();
        
        // Select beats based on progress
        foreach (var beat in _storyBeats)
        {
            if (ShouldIncludeBeat(beat, progress))
            {
                activeBeats.Add(beat);
            }
        }
        
        return activeBeats;
    }
    
    private bool ShouldIncludeBeat(StoryBeat beat, int progress)
    {
        // Include beats based on progress and importance
        float progressThreshold = beat.Importance * 10f; // Convert to level count
        
        return progress >= progressThreshold;
    }
    
    private void PlaceNarrativeElements(LevelData levelData, StoryBeat beat)
    {
        // Get element templates for this story beat
        var elementTypes = GetElementTypesForBeat(beat);
        
        foreach (var elementType in elementTypes)
        {
            var positions = FindOptimalPositions(levelData, elementType, beat);
            
            foreach (var position in positions)
            {
                var element = CreateNarrativeElement(elementType, position, beat);
                levelData.NarrativeElements.Add(element);
            }
        }
    }
    
    private EnvironmentalType[] GetElementTypesForBeat(StoryBeat beat)
    {
        return beat.Tone switch
        {
            EmotionalTone.Tension => new[] { EnvironmentalType.WarningSigns, EnvironmentalType.StructuralDamage },
            EmotionalTone.Urgency => new[] { EnvironmentalType.AbandonedWorkstation, EnvironmentalType.PersonalEffects },
            EmotionalTone.Danger => new[] { EnvironmentalType.DamagedEquipment, EnvironmentalType.PowerFailure },
            EmotionalTone.Despair => new[] { EnvironmentalType.EmergencyEquipment, EnvironmentalType.MaintenanceArea },
            _ => new EnvironmentalType[0]
        };
    }
    
    private List<Vector2Int> FindOptimalPositions(LevelData levelData, EnvironmentalType elementType, StoryBeat beat)
    {
        var positions = new List<Vector2Int>();
        int elementCount = GetElementCount(elementType, beat.Importance);
        
        for (int i = 0; i < elementCount; i++)
        {
            var position = FindOptimalPosition(levelData, elementType, beat);
            if (position != Vector2Int.zero)
            {
                positions.Add(position);
            }
        }
        
        return positions;
    }
    
    private Vector2Int FindOptimalPosition(LevelData levelData, EnvironmentalType elementType, StoryBeat beat)
    {
        var candidates = new List<Vector2Int>();
        
        // Generate candidate positions
        for (int x = 1; x < levelData.Width - 1; x++)
        {
            for (int y = 1; y < levelData.Height - 1; y++)
            {
                var pos = new Vector2Int(x, y);
                
                if (IsValidNarrativePosition(pos, levelData, elementType))
                {
                    candidates.Add(pos);
                }
            }
        }
        
        // Score candidates based on narrative impact
        var scoredCandidates = candidates.Select(pos => new
        {
            Position = pos,
            Score = ScoreNarrativePosition(pos, levelData, elementType, beat)
        }).OrderByDescending(c => c.Score).ToList();
        
        return scoredCandidates.Count > 0 ? scoredCandidates[0].Position : Vector2Int.zero;
    }
    
    private float ScoreNarrativePosition(Vector2Int position, LevelData levelData, EnvironmentalType elementType, StoryBeat beat)
    {
        float score = 0f;
        
        // Visibility score (how likely player is to see this)
        float visibility = CalculateVisibility(position, levelData);
        score += visibility * 0.3f;
        
        // Contextual relevance (how well it fits the environment)
        float contextRelevance = CalculateContextualRelevance(position, levelData, elementType);
        score += contextRelevance * 0.4f;
        
        // Emotional impact (how much it contributes to the story beat)
        float emotionalImpact = CalculateEmotionalImpact(position, levelData, beat);
        score += emotionalImpact * 0.3f;
        
        return score;
    }
    
    private float CalculateVisibility(Vector2Int position, LevelData levelData)
    {
        // Calculate how visible the position is from common paths
        float visibility = 0f;
        
        // Check visibility from main paths
        var mainPaths = GetMainPaths(levelData);
        foreach (var path in mainPaths)
        {
            float distance = Vector2Int.Distance(position, path);
            if (distance <= 3f) // Within 3 tiles
            {
                visibility += 1f - (distance / 3f);
            }
        }
        
        return Mathf.Clamp01(visibility / mainPaths.Count);
    }
    
    private float CalculateContextualRelevance(Vector2Int position, LevelData levelData, EnvironmentalType elementType)
    {
        // Score based on how well the element fits its location
        float relevance = 0f;
        
        switch (elementType)
        {
            case EnvironmentalType.DamagedEquipment:
                // Place near walls and corners
                relevance = IsNearWall(position, levelData) ? 1f : 0.3f;
                break;
                
            case EnvironmentalType.AbandonedWorkstation:
                // Place in open areas
                relevance = IsInOpenArea(position, levelData) ? 1f : 0.3f;
                break;
                
            case EnvironmentalType.WarningSigns:
                // Place near intersections and decision points
                relevance = IsNearIntersection(position, levelData) ? 1f : 0.3f;
                break;
        }
        
        return relevance;
    }
}
```

## Spatial Design Principles

### Player Flow and Navigation

#### Flow Analysis System
```csharp
// Player flow analysis and optimization
public class PlayerFlowAnalyzer
{
    public struct FlowMetrics
    {
        public float PathEfficiency;      // How efficient player paths are
        public float ExplorationRate;     // How much of the level is explored
        public float BacktrackingAmount;   // How much backtracking occurs
        public float DecisionPoints;       // Number of meaningful decisions
        public float FlowScore;           // Overall flow quality
    }
    
    public struct FlowNode
    {
        public Vector2Int Position;
        public List<FlowNode> Connections;
        public float TrafficVolume;
        public float DecisionWeight;
        public NodeType Type;
    }
    
    public enum NodeType
    {
        Entrance,
        Exit,
        Junction,
        DeadEnd,
        Corridor,
        Room
    }
    
    public FlowMetrics AnalyzePlayerFlow(LevelData levelData, List<PlayerPathData> playerPaths)
    {
        // Create flow graph from level data
        var flowGraph = CreateFlowGraph(levelData);
        
        // Analyze player paths
        var pathAnalysis = AnalyzePlayerPaths(playerPaths, flowGraph);
        
        // Calculate flow metrics
        var metrics = new FlowMetrics
        {
            PathEfficiency = CalculatePathEfficiency(playerPaths, levelData),
            ExplorationRate = CalculateExplorationRate(playerPaths, levelData),
            BacktrackingAmount = CalculateBacktrackingAmount(playerPaths),
            DecisionPoints = CalculateDecisionPoints(flowGraph),
            FlowScore = 0f
        };
        
        // Calculate overall flow score
        metrics.FlowScore = CalculateOverallFlowScore(metrics);
        
        return metrics;
    }
    
    private FlowNode[] CreateFlowGraph(LevelData levelData)
    {
        var nodes = new List<FlowNode>();
        var nodeDictionary = new Dictionary<Vector2Int, FlowNode>();
        
        // Identify key nodes
        IdentifyKeyNodes(levelData, nodes, nodeDictionary);
        
        // Create connections between nodes
        CreateNodeConnections(levelData, nodes, nodeDictionary);
        
        return nodes.ToArray();
    }
    
    private void IdentifyKeyNodes(LevelData levelData, List<FlowNode> nodes, Dictionary<Vector2Int, FlowNode> nodeDictionary)
    {
        // Find entrance and exit
        var entrance = FindEntrance(levelData);
        var exit = FindExit(levelData);
        
        if (entrance != Vector2Int.zero)
        {
            var entranceNode = new FlowNode { Position = entrance, Type = NodeType.Entrance };
            nodes.Add(entranceNode);
            nodeDictionary[entrance] = entranceNode;
        }
        
        if (exit != Vector2Int.zero)
        {
            var exitNode = new FlowNode { Position = exit, Type = NodeType.Exit };
            nodes.Add(exitNode);
            nodeDictionary[exit] = exitNode;
        }
        
        // Find junctions and decision points
        FindJunctions(levelData, nodes, nodeDictionary);
        
        // Find rooms and dead ends
        FindRoomsAndDeadEnds(levelData, nodes, nodeDictionary);
    }
    
    private void FindJunctions(LevelData levelData, List<FlowNode> nodes, Dictionary<Vector2Int, FlowNode> nodeDictionary)
    {
        for (int x = 1; x < levelData.Width - 1; x++)
        {
            for (int y = 1; y < levelData.Height - 1; y++)
            {
                var pos = new Vector2Int(x, y);
                
                if (IsJunction(pos, levelData))
                {
                    var junctionNode = new FlowNode 
                    { 
                        Position = pos, 
                        Type = NodeType.Junction,
                        DecisionWeight = CalculateDecisionWeight(pos, levelData)
                    };
                    nodes.Add(junctionNode);
                    nodeDictionary[pos] = junctionNode;
                }
            }
        }
    }
    
    private bool IsJunction(Vector2Int position, LevelData levelData)
    {
        // Check if position has multiple valid paths
        int pathCount = 0;
        var directions = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        
        foreach (var dir in directions)
        {
            var nextPos = position + dir;
            if (IsValidPosition(nextPos, levelData) && !levelData.Grid[nextPos.x, nextPos.y])
            {
                pathCount++;
            }
        }
        
        return pathCount >= 3; // Junction has 3 or more paths
    }
    
    private float CalculateDecisionWeight(Vector2Int position, LevelData levelData)
    {
        // Calculate how important this decision point is
        float weight = 0f;
        
        // More paths = higher decision weight
        int pathCount = CountValidPaths(position, levelData);
        weight += pathCount * 0.2f;
        
        // Proximity to objectives
        float objectiveProximity = CalculateObjectiveProximity(position, levelData);
        weight += (1f - objectiveProximity) * 0.3f;
        
        // Strategic value
        float strategicValue = CalculateStrategicValue(position, levelData);
        weight += strategicValue * 0.5f;
        
        return Mathf.Clamp01(weight);
    }
    
    private float CalculatePathEfficiency(List<PlayerPathData> playerPaths, LevelData levelData)
    {
        if (playerPaths.Count == 0)
            return 0f;
            
        float totalEfficiency = 0f;
        
        foreach (var path in playerPaths)
        {
            // Calculate optimal path length
            float optimalLength = CalculateOptimalPathLength(path.StartPosition, path.EndPosition, levelData);
            
            // Calculate efficiency
            float efficiency = optimalLength / path.ActualLength;
            totalEfficiency += Mathf.Clamp01(efficiency);
        }
        
        return totalEfficiency / playerPaths.Count;
    }
    
    private float CalculateExplorationRate(List<PlayerPathData> playerPaths, LevelData levelData)
    {
        var visitedPositions = new HashSet<Vector2Int>();
        
        foreach (var path in playerPaths)
        {
            foreach (var pos in path.VisitedPositions)
            {
                visitedPositions.Add(pos);
            }
        }
        
        int totalAccessiblePositions = CountAccessiblePositions(levelData);
        return (float)visitedPositions.Count / totalAccessiblePositions;
    }
    
    private float CalculateOverallFlowScore(FlowMetrics metrics)
    {
        // Weight different metrics for overall flow score
        float weightedScore = 
            metrics.PathEfficiency * 0.3f +
            (1f - metrics.BacktrackingAmount) * 0.25f + // Less backtracking is better
            metrics.ExplorationRate * 0.2f +
            (metrics.DecisionPoints / 10f) * 0.15f + // Normalize decision points
            metrics.DecisionPoints * 0.1f; // More decision points is good
        
        return Mathf.Clamp01(weightedScore);
    }
}
```

---

**Last Updated**: 2026-02-20  
**Skill Version**: 1.0  
**Generation System**: Seed-Based Procedural  
**Design Philosophy**: Environmental Storytelling + Player Flow  
**Maintainer**: Level Design Team
