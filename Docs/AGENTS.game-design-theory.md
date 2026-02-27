# Game Design Theory Agent - Specialized Documentation

## Overview

The `game-design-theory` agent provides expertise in game design principles, player psychology, balance mechanics, and progression systems for the Snake Prototype project using the MDA framework and modern design theory.

## Core Responsibilities

### Game Design Framework Application
- MDA (Mechanics, Dynamics, Aesthetics) framework implementation
- Player psychology and motivation analysis
- Game balance and difficulty curve design
- Progression system optimization

### Player Experience Optimization
- Engagement and retention strategies
- Flow state and challenge balancing
- Player feedback and reward systems
- Accessibility and inclusivity considerations

### Design Decision Support
- Mechanic design validation
- Balance change impact analysis
- Player behavior prediction
- Design pattern recommendations

## MDA Framework Implementation

### Mechanics (M)

#### Core Mechanics Analysis
```csharp
// Game mechanics analysis framework
public class GameMechanicsAnalyzer
{
    public struct MechanicProfile
    {
        public string Name;
        public float Complexity;
        public float SkillCeiling;
        public float LearningCurve;
        public float PlayerAgency;
        public string Category; // Movement, Collection, Avoidance, etc.
    }
    
    public MechanicProfile AnalyzeMechanic(string mechanicName)
    {
        return mechanicName switch
        {
            "SnakeMovement" => new MechanicProfile
            {
                Name = "Snake Movement",
                Complexity = 2.0f, // Simple to learn
                SkillCeiling = 6.0f, // High mastery potential
                LearningCurve = 1.5f, // Gentle learning curve
                PlayerAgency = 8.0f, // High player control
                Category = "Movement"
            },
            
            "DetectionSystem" => new MechanicProfile
            {
                Name = "Detection System",
                Complexity = 4.0f, // Moderate complexity
                SkillCeiling = 7.0f, // High mastery potential
                LearningCurve = 3.0f, // Steeper learning curve
                PlayerAgency = 6.0f, // Moderate agency (avoidance)
                Category = "Avoidance"
            },
            
            "EnergyCollection" => new MechanicProfile
            {
                Name = "Energy Core Collection",
                Complexity = 1.0f, // Very simple
                SkillCeiling = 3.0f, // Low mastery ceiling
                LearningCurve = 1.0f, // Instant understanding
                PlayerAgency = 7.0f, // High agency (choice)
                Category = "Collection"
            },
            
            _ => throw new ArgumentException($"Unknown mechanic: {mechanicName}")
        };
    }
    
    public float CalculateMechanicBalance(MechanicProfile mechanic)
    {
        // Balance formula considering multiple factors
        float balanceScore = 0f;
        
        // Complexity should be appropriate for skill ceiling
        balanceScore += Mathf.Abs(mechanic.Complexity - (mechanic.SkillCeiling / 2f)) * -0.5f;
        
        // Learning curve should match complexity
        balanceScore += Mathf.Abs(mechanic.LearningCurve - mechanic.Complexity) * -0.3f;
        
        // Higher player agency is generally better
        balanceScore += mechanic.PlayerAgency * 0.2f;
        
        return Mathf.Clamp01(balanceScore + 0.5f); // Normalize to 0-1
    }
}
```

#### Mechanic Interaction Analysis
```csharp
// Mechanic interaction and synergy analysis
public class MechanicInteractionAnalyzer
{
    public struct InteractionProfile
    {
        public string MechanicA;
        public string MechanicB;
        public float SynergyScore; // How well they work together
        public float ConflictScore; // Potential conflicts
        public string Description;
    }
    
    public InteractionProfile[] AnalyzeSnakePrototypeInteractions()
    {
        return new InteractionProfile[]
        {
            new InteractionProfile
            {
                MechanicA = "SnakeMovement",
                MechanicB = "DetectionSystem",
                SynergyScore = 8.5f, // High synergy - movement creates risk/reward
                ConflictScore = 2.0f, // Low conflict
                Description = "Movement through detection zones creates tension and strategic choices"
            },
            
            new InteractionProfile
            {
                MechanicA = "SnakeMovement",
                MechanicB = "EnergyCollection",
                SynergyScore = 7.0f, // Good synergy - growth affects movement
                ConflictScore = 1.5f, // Very low conflict
                Description = "Collection drives movement and creates progression"
            },
            
            new InteractionProfile
            {
                MechanicA = "DetectionSystem",
                MechanicB = "EnergyCollection",
                SynergyScore = 9.0f, // Excellent synergy - risk/reward core loop
                ConflictScore = 1.0f, // Minimal conflict
                Description = "Core risk/reward loop: collect energy while avoiding detection"
            }
        };
    }
    
    public float CalculateOverallSynergy(InteractionProfile[] interactions)
    {
        float totalSynergy = 0f;
        float totalConflict = 0f;
        
        foreach (var interaction in interactions)
        {
            totalSynergy += interaction.SynergyScore;
            totalConflict += interaction.ConflictScore;
        }
        
        return (totalSynergy - totalConflict) / interactions.Length;
    }
}
```

### Dynamics (D)

#### Player Behavior Modeling
```csharp
// Player behavior and dynamics analysis
public class PlayerDynamicsAnalyzer
{
    public struct PlayerBehaviorProfile
    {
        public string PlayStyle;
        public float RiskTolerance;
        public float ExplorationTendency;
        public float OptimizationFocus;
        public float CompletionRate;
        public float AverageScore;
    }
    
    public PlayerBehaviorProfile AnalyzePlayerBehavior(GameSessionData sessionData)
    {
        // Analyze player behavior patterns
        float riskTolerance = CalculateRiskTolerance(sessionData);
        float explorationTendency = CalculateExplorationTendency(sessionData);
        float optimizationFocus = CalculateOptimizationFocus(sessionData);
        
        return new PlayerBehaviorProfile
        {
            PlayStyle = DeterminePlayStyle(riskTolerance, explorationTendency, optimizationFocus),
            RiskTolerance = riskTolerance,
            ExplorationTendency = explorationTendency,
            OptimizationFocus = optimizationFocus,
            CompletionRate = sessionData.LevelsCompleted / (float)sessionData.LevelsAttempted,
            AverageScore = sessionData.TotalScore / sessionData.SessionsPlayed
        };
    }
    
    private float CalculateRiskTolerance(GameSessionData data)
    {
        // Calculate based on detection encounters and near-misses
        int totalDetectionEvents = data.DetectionEncounters + data.NearMisses;
        int totalMovementActions = data.TotalMoves;
        
        return totalDetectionEvents / (float)totalMovementActions;
    }
    
    private float CalculateExplorationTendency(GameSessionData data)
    {
        // Calculate based on unique grid cells visited
        int totalGridCells = data.GridWidth * data.GridHeight;
        float explorationRatio = data.UniqueCellsVisited / (float)totalGridCells;
        
        return explorationRatio;
    }
    
    private float CalculateOptimizationFocus(GameSessionData data)
    {
        // Calculate based on path efficiency and score optimization
        float pathEfficiency = data.OptimalPathLength / data.ActualPathLength;
        float scoreEfficiency = data.ActualScore / data.TheoreticalMaxScore;
        
        return (pathEfficiency + scoreEfficiency) / 2f;
    }
    
    private string DeterminePlayStyle(float risk, float exploration, float optimization)
    {
        if (risk > 0.7f && exploration > 0.6f)
            return "Aggressive Explorer";
        if (risk < 0.3f && optimization > 0.7f)
            return "Conservative Optimizer";
        if (exploration > 0.7f && risk > 0.5f)
            return "Risk-Taking Explorer";
        if (optimization > 0.6f && risk < 0.5f)
            return "Efficient Planner";
        if (risk > 0.6f)
            return "Risk-Taker";
        if (exploration > 0.6f)
            return "Explorer";
        if (optimization > 0.6f)
            return "Optimizer";
        
        return "Balanced Player";
    }
}
```

#### Flow State Analysis
```csharp
// Flow state and engagement analysis
public class FlowStateAnalyzer
{
    public struct FlowProfile
    {
        public float ChallengeLevel;
        public float SkillLevel;
        public float FlowStateScore;
        public float EngagementLevel;
        public string FlowChannel;
    }
    
    public FlowProfile AnalyzeFlowState(PlayerPerformanceData performance)
    {
        float challengeLevel = CalculateChallengeLevel(performance);
        float skillLevel = CalculateSkillLevel(performance);
        float flowScore = CalculateFlowState(challengeLevel, skillLevel);
        float engagement = CalculateEngagement(performance);
        
        return new FlowProfile
        {
            ChallengeLevel = challengeLevel,
            SkillLevel = skillLevel,
            FlowStateScore = flowScore,
            EngagementLevel = engagement,
            FlowChannel = DetermineFlowChannel(challengeLevel, skillLevel)
        };
    }
    
    private float CalculateChallengeLevel(PlayerPerformanceData performance)
    {
        // Challenge based on detection frequency, level complexity, time pressure
        float detectionChallenge = performance.DetectionFrequency * 0.4f;
        float complexityChallenge = performance.LevelComplexity * 0.3f;
        float timeChallenge = (1f - performance.TimeRemainingRatio) * 0.3f;
        
        return Mathf.Clamp01(detectionChallenge + complexityChallenge + timeChallenge);
    }
    
    private float CalculateSkillLevel(PlayerPerformanceData performance)
    {
        // Skill based on success rate, efficiency, and consistency
        float successRate = performance.SuccessRate * 0.4f;
        float efficiency = performance.PathEfficiency * 0.3f;
        float consistency = (1f - performance.Variance) * 0.3f;
        
        return Mathf.Clamp01(successRate + efficiency + consistency);
    }
    
    private float CalculateFlowState(float challenge, float skill)
    {
        // Flow occurs when challenge and skill are balanced and both are high
        float balance = 1f - Mathf.Abs(challenge - skill);
        float level = (challenge + skill) / 2f;
        
        return balance * level;
    }
    
    private string DetermineFlowChannel(float challenge, float skill)
    {
        if (challenge > skill + 0.3f)
            return "Anxiety"; // Too challenging
        if (skill > challenge + 0.3f)
            return "Boredom"; // Not challenging enough
        if (challenge < 0.3f || skill < 0.3f)
            return "Apathy"; // Both too low
        if (challenge > 0.7f && skill > 0.7f)
            return "Flow"; // Optimal flow state
        if (challenge > 0.5f && skill > 0.5f)
            return "Control"; // Good balance
        
        return "Transition"; // Moving between states
    }
}
```

### Aesthetics (A)

#### Player Experience Mapping
```csharp
// Player experience and aesthetic analysis
public class ExperienceAnalyzer
{
    public enum ExperienceType
    {
        Challenge,      // Overcoming obstacles
        Discovery,      // Exploring and finding new things
        Fantasy,        // Immersion in game world
        Narrative,      // Story and emotional engagement
        Fellowship,     // Social interaction
        Sensation,      // Audiovisual enjoyment
        Expression,      // Self-expression and creativity
        Submission      // Mindless fun and relaxation
    }
    
    public struct ExperienceProfile
    {
        public ExperienceType PrimaryExperience;
        public ExperienceType SecondaryExperience;
        public float[] ExperienceScores; // Scores for each experience type
        public float OverallSatisfaction;
    }
    
    public ExperienceProfile AnalyzePlayerExperience(PlayerFeedbackData feedback)
    {
        float[] scores = CalculateExperienceScores(feedback);
        var sortedScores = scores
            .Select((score, index) => new { Score = score, Type = (ExperienceType)index })
            .OrderByDescending(x => x.Score)
            .ToArray();
        
        return new ExperienceProfile
        {
            PrimaryExperience = sortedScores[0].Type,
            SecondaryExperience = sortedScores[1].Type,
            ExperienceScores = scores,
            OverallSatisfaction = feedback.OverallSatisfaction
        };
    }
    
    private float[] CalculateExperienceScores(PlayerFeedbackData feedback)
    {
        return new float[]
        {
            CalculateChallengeScore(feedback),    // Challenge
            CalculateDiscoveryScore(feedback),    // Discovery
            CalculateFantasyScore(feedback),      // Fantasy
            CalculateNarrativeScore(feedback),    // Narrative
            CalculateFellowshipScore(feedback),  // Fellowship
            CalculateSensationScore(feedback),    // Sensation
            CalculateExpressionScore(feedback),  // Expression
            CalculateSubmissionScore(feedback)    // Submission
        };
    }
    
    private float CalculateChallengeScore(PlayerFeedbackData feedback)
    {
        return (feedback.DifficultyRating + feedback.SenseOfAccomplishment) / 2f;
    }
    
    private float CalculateDiscoveryScore(PlayerFeedbackData feedback)
    {
        return (feedback.ExplorationEnjoyment + feedback.NewContentRating) / 2f;
    }
    
    private float CalculateFantasyScore(PlayerFeedbackData feedback)
    {
        return (feedback.ImmersionRating + feedback.WorldBuildingRating) / 2f;
    }
    
    // ... other calculation methods
}
```

## Balance Principles

### Difficulty Curve Design

#### Dynamic Difficulty Adjustment
```csharp
// Dynamic difficulty adjustment system
public class DynamicDifficultyManager
{
    public struct DifficultySettings
    {
        public float SnakeSpeed;
        public float DetectionSensitivity;
        public float EnergyCoreSpawnRate;
        public float GuardPatrolSpeed;
        public float DifficultyLevel; // 0.0 to 1.0
    }
    
    private DifficultySettings _baseSettings;
    private DifficultySettings _currentSettings;
    private PlayerPerformanceTracker _performanceTracker;
    
    public DynamicDifficultyManager(DifficultySettings baseSettings)
    {
        _baseSettings = baseSettings;
        _currentSettings = baseSettings;
        _performanceTracker = new PlayerPerformanceTracker();
    }
    
    public void UpdateDifficulty(float deltaTime)
    {
        _performanceTracker.Update(deltaTime);
        
        // Calculate optimal difficulty based on player performance
        float optimalDifficulty = CalculateOptimalDifficulty();
        
        // Smoothly adjust difficulty
        _currentSettings.DifficultyLevel = Mathf.Lerp(
            _currentSettings.DifficultyLevel,
            optimalDifficulty,
            deltaTime * 0.1f // Smooth adjustment over 10 seconds
        );
        
        // Apply difficulty to game settings
        ApplyDifficultySettings();
    }
    
    private float CalculateOptimalDifficulty()
    {
        float successRate = _performanceTracker.GetRecentSuccessRate();
        float engagementLevel = _performanceTracker.GetEngagementLevel();
        float frustrationLevel = _performanceTracker.GetFrustrationLevel();
        
        // Target 70% success rate for optimal flow
        float targetSuccessRate = 0.7f;
        float successRateDiff = targetSuccessRate - successRate;
        
        // Adjust based on engagement and frustration
        float difficultyAdjustment = successRateDiff * 2f; // Amplify adjustment
        difficultyAdjustment += (engagementLevel - frustrationLevel) * 0.5f;
        
        return Mathf.Clamp01(_baseSettings.DifficultyLevel + difficultyAdjustment);
    }
    
    private void ApplyDifficultySettings()
    {
        float difficulty = _currentSettings.DifficultyLevel;
        
        // Apply difficulty multipliers
        _currentSettings.SnakeSpeed = _baseSettings.SnakeSpeed * (1f + difficulty * 0.5f);
        _currentSettings.DetectionSensitivity = _baseSettings.DetectionSensitivity * (1f + difficulty * 0.3f);
        _currentSettings.EnergyCoreSpawnRate = _baseSettings.EnergyCoreSpawnRate * (1f - difficulty * 0.2f);
        _currentSettings.GuardPatrolSpeed = _baseSettings.GuardPatrolSpeed * (1f + difficulty * 0.4f);
    }
}
```

#### Balance Metrics and Analysis
```csharp
// Game balance analysis tools
public class BalanceAnalyzer
{
    public struct BalanceReport
    {
        public float OverallBalanceScore;
        public float[] MechanicBalanceScores;
        public string[] BalanceIssues;
        public string[] Recommendations;
    }
    
    public BalanceReport AnalyzeGameBalance(GameData gameData)
    {
        var report = new BalanceReport
        {
            MechanicBalanceScores = CalculateMechanicBalanceScores(gameData),
            BalanceIssues = IdentifyBalanceIssues(gameData),
            Recommendations = GenerateBalanceRecommendations(gameData)
        };
        
        report.OverallBalanceScore = report.MechanicBalanceScores.Average();
        
        return report;
    }
    
    private float[] CalculateMechanicBalanceScores(GameData gameData)
    {
        return new float[]
        {
            CalculateMovementBalance(gameData),
            CalculateDetectionBalance(gameData),
            CalculateCollectionBalance(gameData),
            CalculateProgressionBalance(gameData)
        };
    }
    
    private float CalculateMovementBalance(GameData gameData)
    {
        // Movement should be responsive but not too fast
        float responsivenessScore = Mathf.Clamp01(gameData.AverageInputLatency / 0.1f); // Target 100ms
        float speedBalanceScore = Mathf.Clamp01(1f - Mathf.Abs(gameData.AverageSnakeSpeed - 5f) / 5f); // Target 5 units/sec
        
        return (responsivenessScore + speedBalanceScore) / 2f;
    }
    
    private float CalculateDetectionBalance(GameData gameData)
    {
        // Detection should be challenging but fair
        float detectionRateScore = Mathf.Clamp01(1f - Mathf.Abs(gameData.DetectionRate - 0.3f) / 0.3f); // Target 30% detection rate
        float avoidanceScore = gameData.SuccessfulAvoidanceRate; // Higher is better
        
        return (detectionRateScore + avoidanceScore) / 2f;
    }
    
    private float CalculateCollectionBalance(GameData gameData)
    {
        // Collection should be rewarding and achievable
        float collectionRateScore = Mathf.Clamp01(gameData.EnergyCollectionRate / 0.8f); // Target 80% collection rate
        float rewardScore = Mathf.Clamp01(gameData.ScorePerCollection / 100f); // Target 100 points per collection
        
        return (collectionRateScore + rewardScore) / 2f;
    }
    
    private float CalculateProgressionBalance(GameData gameData)
    {
        // Progression should be smooth and motivating
        float levelCompletionScore = gameData.LevelCompletionRate;
        float difficultyProgressionScore = Mathf.Clamp01(1f - Mathf.Abs(gameData.DifficultyIncreaseRate - 0.1f) / 0.1f); // Target 10% increase per level
        
        return (levelCompletionScore + difficultyProgressionScore) / 2f;
    }
}
```

## Progression Systems

### Player Progression Design

#### Skill Tree and Unlock System
```csharp
// Player progression and skill system
public class ProgressionSystem
{
    public struct SkillNode
    {
        public string Id;
        public string Name;
        public string Description;
        public int Cost;
        public string[] Prerequisites;
        public float[] Effects; // Multipliers for various game parameters
        public bool IsUnlocked;
        public bool IsPurchased;
    }
    
    private Dictionary<string, SkillNode> _skillTree;
    private PlayerProgress _playerProgress;
    
    public ProgressionSystem()
    {
        InitializeSkillTree();
        _playerProgress = new PlayerProgress();
    }
    
    private void InitializeSkillTree()
    {
        _skillTree = new Dictionary<string, SkillNode>
        {
            ["speed_boost_1"] = new SkillNode
            {
                Id = "speed_boost_1",
                Name = "Speed Boost I",
                Description = "Increase snake movement speed by 10%",
                Cost = 100,
                Prerequisites = new string[0],
                Effects = new float[] { 1.1f, 0f, 0f, 0f }, // Speed, Detection, Collection, Health
                IsUnlocked = true,
                IsPurchased = false
            },
            
            ["detection_reduction_1"] = new SkillNode
            {
                Id = "detection_reduction_1",
                Name = "Stealth I",
                Description = "Reduce detection sensitivity by 15%",
                Cost = 150,
                Prerequisites = new string[] { "speed_boost_1" },
                Effects = new float[] { 0f, 0.85f, 0f, 0f },
                IsUnlocked = false,
                IsPurchased = false
            },
            
            ["collection_bonus_1"] = new SkillNode
            {
                Id = "collection_bonus_1",
                Name = "Efficiency I",
                Description = "Increase energy core value by 20%",
                Cost = 200,
                Prerequisites = new string[] { "speed_boost_1" },
                Effects = new float[] { 0f, 0f, 1.2f, 0f },
                IsUnlocked = false,
                IsPurchased = false
            }
        };
    }
    
    public bool CanPurchaseSkill(string skillId)
    {
        if (!_skillTree.ContainsKey(skillId))
            return false;
            
        var skill = _skillTree[skillId];
        
        // Check if player has enough points
        if (_playerProgress.SkillPoints < skill.Cost)
            return false;
            
        // Check prerequisites
        foreach (var prereq in skill.Prerequisites)
        {
            if (!_skillTree[prereq].IsPurchased)
                return false;
        }
        
        return true;
    }
    
    public void PurchaseSkill(string skillId)
    {
        if (!CanPurchaseSkill(skillId))
            return;
            
        var skill = _skillTree[skillId];
        skill.IsPurchased = true;
        _playerProgress.SkillPoints -= skill.Cost;
        
        // Apply skill effects
        ApplySkillEffects(skill);
        
        // Unlock dependent skills
        UnlockDependentSkills(skillId);
    }
    
    private void ApplySkillEffects(SkillNode skill)
    {
        // Apply effects to game parameters
        _playerProgress.SpeedMultiplier *= skill.Effects[0];
        _playerProgress.DetectionMultiplier *= skill.Effects[1];
        _playerProgress.CollectionMultiplier *= skill.Effects[2];
        _playerProgress.HealthMultiplier *= skill.Effects[3];
    }
    
    private void UnlockDependentSkills(string purchasedSkillId)
    {
        foreach (var skill in _skillTree.Values)
        {
            if (skill.Prerequisites.Contains(purchasedSkillId) && !skill.IsUnlocked)
            {
                skill.IsUnlocked = true;
            }
        }
    }
}
```

### Reward System Design

#### Intrinsic and Extrinsic Rewards
```csharp
// Reward system design
public class RewardSystem
{
    public enum RewardType
    {
        Intrinsic,      // Mastery, achievement, discovery
        Extrinsic,      // Points, unlocks, cosmetics
        Social          // Leaderboards, sharing, recognition
    }
    
    public struct Reward
    {
        public RewardType Type;
        public string Name;
        public string Description;
        public float Value;
        public float Probability;
        public bool IsGuaranteed;
    }
    
    public Reward[] GenerateRewardPackage(PlayerAction action, PlayerPerformance performance)
    {
        var rewards = new List<Reward>();
        
        // Base rewards for action
        rewards.AddRange(GetBaseRewards(action));
        
        // Performance bonuses
        rewards.AddRange(GetPerformanceBonuses(performance));
        
        // Random rewards
        rewards.AddRange(GetRandomRewards(action, performance));
        
        return rewards.ToArray();
    }
    
    private Reward[] GetBaseRewards(PlayerAction action)
    {
        return action switch
        {
            PlayerAction.CollectEnergy => new Reward[]
            {
                new Reward { Type = RewardType.Extrinsic, Name = "Points", Value = 100, IsGuaranteed = true },
                new Reward { Type = RewardType.Intrinsic, Name = "Collection", Value = 10, IsGuaranteed = true }
            },
            
            PlayerAction.AvoidDetection => new Reward[]
            {
                new Reward { Type = RewardType.Intrinsic, Name = "Mastery", Value = 25, IsGuaranteed = true },
                new Reward { Type = RewardType.Extrinsic, Name = "Bonus Points", Value = 50, IsGuaranteed = true }
            },
            
            PlayerAction.CompleteLevel => new Reward[]
            {
                new Reward { Type = RewardType.Extrinsic, Name = "Level Complete", Value = 500, IsGuaranteed = true },
                new Reward { Type = RewardType.Intrinsic, Name = "Achievement", Value = 100, IsGuaranteed = true },
                new Reward { Type = RewardType.Extrinsic, Name = "Skill Point", Value = 1, IsGuaranteed = true }
            },
            
            _ => new Reward[0]
        };
    }
    
    private Reward[] GetPerformanceBonuses(PlayerPerformance performance)
    {
        var bonuses = new List<Reward>();
        
        // Speed bonus
        if (performance.CompletionTime < performance.TargetTime)
        {
            bonuses.Add(new Reward
            {
                Type = RewardType.Extrinsic,
                Name = "Speed Bonus",
                Value = (int)((performance.TargetTime - performance.CompletionTime) * 100),
                IsGuaranteed = true
            });
        }
        
        // Perfect run bonus
        if (performance.DetectionsAvoided == performance.TotalDetections)
        {
            bonuses.Add(new Reward
            {
                Type = RewardType.Intrinsic,
                Name = "Perfect Run",
                Value = 200,
                IsGuaranteed = true
            });
        }
        
        // Collection bonus
        if (performance.EnergyCollected == performance.TotalEnergy)
        {
            bonuses.Add(new Reward
            {
                Type = RewardType.Extrinsic,
                Name = "Collection Master",
                Value = 300,
                IsGuaranteed = true
            });
        }
        
        return bonuses.ToArray();
    }
}
```

## Player Psychology

### Motivation and Engagement

#### Player Motivation Analysis
```csharp
// Player motivation and engagement analysis
public class PlayerMotivationAnalyzer
{
    public enum MotivationType
    {
        Achievement,     // Goal-oriented, mastery
        Exploration,     // Discovery, curiosity
        Social,          // Competition, cooperation
        Immersion,       // Story, world-building
        Creativity,      // Self-expression
        Competition      // Beating others, high scores
    }
    
    public struct MotivationProfile
    {
        public MotivationType PrimaryMotivation;
        public MotivationType SecondaryMotivation;
        public float[] MotivationScores;
        public EngagementPattern Pattern;
    }
    
    public MotivationProfile AnalyzePlayerMotivation(PlayerBehaviorData behaviorData)
    {
        float[] scores = CalculateMotivationScores(behaviorData);
        var sortedScores = scores
            .Select((score, index) => new { Score = score, Type = (MotivationType)index })
            .OrderByDescending(x => x.Score)
            .ToArray();
        
        return new MotivationProfile
        {
            PrimaryMotivation = sortedScores[0].Type,
            SecondaryMotivation = sortedScores[1].Type,
            MotivationScores = scores,
            Pattern = DetermineEngagementPattern(behaviorData)
        };
    }
    
    private float[] CalculateMotivationScores(PlayerBehaviorData data)
    {
        return new float[]
        {
            CalculateAchievementMotivation(data),
            CalculateExplorationMotivation(data),
            CalculateSocialMotivation(data),
            CalculateImmersionMotivation(data),
            CalculateCreativityMotivation(data),
            CalculateCompetitionMotivation(data)
        };
    }
    
    private float CalculateAchievementMotivation(PlayerBehaviorData data)
    {
        // Achievement motivation: goal completion, mastery seeking
        float goalCompletion = data.GoalsCompleted / (float)data.GoalsAttempted;
        float masterySeeking = data.DifficultyAttempts / (float)data.TotalSessions;
        float persistence = data.AverageAttemptsPerGoal;
        
        return (goalCompletion + masterySeeking + (1f / persistence)) / 3f;
    }
    
    private float CalculateExplorationMotivation(PlayerBehaviorData data)
    {
        // Exploration motivation: discovery, curiosity
        float explorationRate = data.UniqueAreasExplored / (float)data.TotalAreas;
        float curiosityScore = data.HiddenItemsFound / (float)data.TotalHiddenItems;
        float varietySeeking = data.DifferentStrategiesUsed / 10f; // Normalized to 10 strategies
        
        return (explorationRate + curiosityScore + varietySeeking) / 3f;
    }
    
    private float CalculateCompetitionMotivation(PlayerBehaviorData data)
    {
        // Competition motivation: beating others, high scores
        float leaderboardActivity = data.LeaderboardChecks / (float)data.TotalSessions;
        float scoreOptimization = data.ScoreImprovementRate;
        float challengeSeeking = data.HighDifficultyAttempts / (float)data.TotalSessions;
        
        return (leaderboardActivity + scoreOptimization + challengeSeeking) / 3f;
    }
    
    private EngagementPattern DetermineEngagementPattern(PlayerBehaviorData data)
    {
        if (data.SessionLength > 30 && data.ReturnRate > 0.8f)
            return EngagementPattern.DeepEngagement;
        if (data.SessionLength < 10 && data.Frequency > 5)
            return EngagementPattern.CasualEngagement;
        if (data.SessionLength > 20 && data.Frequency < 2)
            return EngagementPattern.BingeEngagement;
        if (data.SessionLength < 5 && data.ReturnRate < 0.3f)
            return EngagementPattern.LowEngagement;
        
        return EngagementPattern.ModerateEngagement;
    }
}
```

### Cognitive Load Management

#### Cognitive Load Analysis
```csharp
// Cognitive load and complexity management
public class CognitiveLoadAnalyzer
{
    public struct CognitiveLoadProfile
    {
        public float IntrinsicLoad;    // Difficulty of the core concept
        public float ExtraneousLoad;  // Poor design elements
        public float GermaneLoad;     // Schema construction and learning
        public float TotalLoad;
        public float OptimalLoad;
        public string[] LoadSources;
    }
    
    public CognitiveLoadProfile AnalyzeCognitiveLoad(GameState gameState, PlayerSkillLevel skillLevel)
    {
        float intrinsicLoad = CalculateIntrinsicLoad(gameState);
        float extraneousLoad = CalculateExtraneousLoad(gameState);
        float germaneLoad = CalculateGermaneLoad(gameState, skillLevel);
        
        float totalLoad = intrinsicLoad + extraneousLoad + germaneLoad;
        float optimalLoad = CalculateOptimalLoad(skillLevel);
        
        return new CognitiveLoadProfile
        {
            IntrinsicLoad = intrinsicLoad,
            ExtraneousLoad = extraneousLoad,
            GermaneLoad = germaneLoad,
            TotalLoad = totalLoad,
            OptimalLoad = optimalLoad,
            LoadSources = IdentifyLoadSources(gameState)
        };
    }
    
    private float CalculateIntrinsicLoad(GameState gameState)
    {
        // Core game difficulty
        float movementComplexity = gameState.SnakeLength * 0.1f;
        float detectionComplexity = gameState.ActiveDetectionSources * 0.2f;
        float spatialComplexity = gameState.GridComplexity * 0.3f;
        float temporalComplexity = gameState.TimePressure * 0.4f;
        
        return movementComplexity + detectionComplexity + spatialComplexity + temporalComplexity;
    }
    
    private float CalculateExtraneousLoad(GameState gameState)
    {
        // Poor design elements that add unnecessary complexity
        float uiComplexity = gameState.ActiveUIElements * 0.1f;
        float visualNoise = gameState.VisualEffectsIntensity * 0.2f;
        float audioNoise = gameState.AudioComplexity * 0.1f;
        float controlComplexity = gameState.ControlSchemeComplexity * 0.3f;
        
        return uiComplexity + visualNoise + audioNoise + controlComplexity;
    }
    
    private float CalculateGermaneLoad(GameState gameState, PlayerSkillLevel skillLevel)
    {
        // Learning and schema construction
        float learningOpportunities = gameState.NewMechanicsIntroduced * 0.3f;
        float patternRecognition = gameState.PatternComplexity * 0.2f;
        float skillApplication = gameState.RequiredSkillLevel * 0.5f;
        
        // Adjust based on player skill
        float skillMultiplier = skillLevel switch
        {
            PlayerSkillLevel.Beginner => 1.5f,    // Beginners need more germane load
            PlayerSkillLevel.Intermediate => 1.0f,
            PlayerSkillLevel.Expert => 0.7f,       // Experts need less germane load
            _ => 1.0f
        };
        
        return (learningOpportunities + patternRecognition + skillApplication) * skillMultiplier;
    }
    
    private float CalculateOptimalLoad(PlayerSkillLevel skillLevel)
    {
        return skillLevel switch
        {
            PlayerSkillLevel.Beginner => 0.4f,      // Lower optimal load for beginners
            PlayerSkillLevel.Intermediate => 0.6f,
            PlayerSkillLevel.Expert => 0.8f,          // Higher optimal load for experts
            _ => 0.6f
        };
    }
}
```

---

**Last Updated**: 2026-02-20  
**Skill Version**: 1.0  
**Framework**: MDA (Mechanics, Dynamics, Aesthetics)  
**Maintainer**: Game Design Team
