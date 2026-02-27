using UnityEngine;
using UnityEngine.UIElements;
using SnakePrototype.Core;
using SnakePrototype.Events;
using SnakePrototype.Systems.Score;

namespace SnakePrototype.Systems.UI
{
    [AddComponentMenu("SnakePrototype/UI/HighscoreUI")]
    public class HighscoreUI : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private VisualElement _root;
        private ScrollView _highscoreList;
        private Button _backButton;
        private Button _clearButton;
        
        private ScoreSystem _scoreSystem;
        
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
        }
        
        private void OnEnable()
        {
            InitializeUI();
            GameEventManager.AddListener<ScoreChangedEvent>(OnScoreChanged);
            GameEventManager.AddListener<GameStateChangedEvent>(OnGameStateChanged);
        }
        
        private void OnDisable()
        {
            GameEventManager.RemoveListener<ScoreChangedEvent>(OnScoreChanged);
            GameEventManager.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
            
            if (_backButton != null) _backButton.clicked -= OnBackClicked;
            if (_clearButton != null) _clearButton.clicked -= OnClearClicked;
        }
        
        private void InitializeUI()
        {
            _scoreSystem = ServiceLocator.Get<ScoreSystem>();
            
            if (_uiDocument != null)
            {
                _root = _uiDocument.rootVisualElement;
                
                _highscoreList = _root.Q<ScrollView>("HighscoreList");
                _backButton = _root.Q<Button>("BackButton");
                _clearButton = _root.Q<Button>("ClearButton");
                
                if (_backButton != null) _backButton.clicked += OnBackClicked;
                if (_clearButton != null) _clearButton.clicked += OnClearClicked;
                
                // Hide by default until GameState.Highscores is reached
                _root.style.display = DisplayStyle.None;
                
                RefreshHighscores();
            }
        }
        
        public void RefreshHighscores()
        {
            if (_scoreSystem == null || _highscoreList == null) return;
            
            _highscoreList.Clear();
            
            var highscores = _scoreSystem.Highscores;
            
            if (highscores == null || highscores.Length == 0)
            {
                var noScoresLabel = new Label("DATABASE_EMPTY: NO_RECORDS_FOUND");
                noScoresLabel.AddToClassList("no-scores");
                _highscoreList.Add(noScoresLabel);
                return;
            }
            
            // Create terminal-style highscore entries
            for (int i = 0; i < highscores.Length; i++)
            {
                var entry = highscores[i];
                var entryElement = CreateHighscoreEntry(i + 1, entry);
                _highscoreList.Add(entryElement);
            }
        }
        
        private VisualElement CreateHighscoreEntry(int rank, HighscoreEntry entry)
        {
            var container = new VisualElement();
            container.AddToClassList("highscore-entry");
            
            // Rank
            var rankLabel = new Label($"{rank:D2}");
            rankLabel.AddToClassList("rank");
            
            // Player name
            var nameLabel = new Label(entry.PlayerName.ToUpper());
            nameLabel.AddToClassList("player-name");
            
            // Score
            var scoreLabel = new Label($"{entry.Score:D7}");
            scoreLabel.AddToClassList("score");
            
            // Level
            var levelLabel = new Label($"SEC_{entry.Level:D2}");
            levelLabel.AddToClassList("level");
            
            // Rank title
            var rankTitleLabel = new Label(entry.Rank);
            rankTitleLabel.AddToClassList("rank-title");
            
            container.Add(rankLabel);
            container.Add(nameLabel);
            container.Add(scoreLabel);
            container.Add(levelLabel);
            container.Add(rankTitleLabel);
            
            return container;
        }
        
        private void OnBackClicked()
        {
            Debug.Log("Highscore Back Clicked");
            GameEventManager.Publish(new GameStateChangedEvent(GameState.MainMenu));
        }
        
        private void OnClearClicked()
        {
            Debug.Log("Wipe Database Clicked");
            if (_scoreSystem != null)
            {
                _scoreSystem.ClearHighscores();
                RefreshHighscores();
            }
        }
        
        private void OnScoreChanged(ScoreChangedEvent e)
        {
            // Refresh if highscores were updated
            RefreshHighscores();
        }
        private void OnGameStateChanged(GameStateChangedEvent e)
        {
            if (_uiDocument != null)
            {
                _uiDocument.rootVisualElement.style.display = (e.NewState == GameState.Highscores) ? DisplayStyle.Flex : DisplayStyle.None;
                
                if (e.NewState == GameState.Highscores)
                {
                    RefreshHighscores();
                }
            }
        }
    }
}
