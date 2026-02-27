using UnityEngine;

namespace SnakePrototype.Systems.Score
{
    [CreateAssetMenu(fileName = "ScoreConfiguration", menuName = "SnakePrototype/ScoreConfiguration")]
    public class ScoreConfiguration : ScriptableObject
    {
        [Header("Scoring Values")]
        public int EnergyCoreValue = 10;
        public int LengthMultiplier = 5;
        public int SpeedBonus = 2;
        public int DetectionPenalty = 20;
        
        [Header("Highscore Settings")]
        public int MaxHighscores = 10;
        public string PlayerNameDefault = "PLAYER";
        
        [Header("NES-style Features")]
        public bool UseRetroScoring = true;
        public int[] MilestoneScores = { 1000, 5000, 10000, 25000, 50000 };
        public string[] MilestoneNames = { "ROOKIE", "ACE", "EXPERT", "MASTER", "LEGEND" };
    }
}
