using System;
using UnityEngine;

namespace SnakePrototype.Systems.Score
{
    [Serializable]
    public class HighscoreEntry
    {
        public string PlayerName;
        public int Score;
        public int Level;
        public string Rank;
        public DateTime Date;
        public float PlayTime;
        
        public HighscoreEntry(string name, int score, int level, string rank, float playTime)
        {
            PlayerName = name;
            Score = score;
            Level = level;
            Rank = rank;
            Date = DateTime.Now;
            PlayTime = playTime;
        }
    }
    
    [Serializable]
    public class HighscoreData
    {
        public HighscoreEntry[] Entries;
        public int LastPlayedLevel;
        public int TotalGamesPlayed;
        public DateTime LastPlayed;
        
        public HighscoreData()
        {
            Entries = new HighscoreEntry[0];
            LastPlayedLevel = 1;
            TotalGamesPlayed = 0;
            LastPlayed = DateTime.Now;
        }
    }
}
