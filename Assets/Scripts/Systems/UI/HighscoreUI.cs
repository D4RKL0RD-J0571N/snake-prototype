using UnityEngine;
using UnityEngine.UIElements;
using SnakePrototype.Core;
using SnakePrototype.Events;
using SnakePrototype.Systems.Score;

namespace SnakePrototype.Systems.UI
{
    public class HighscoreUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        private VisualElement _root;
        private ScrollView _highscoreList;
        private Label _titleLabel;
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
        }
        
        private void OnDisable()
        {
            GameEventManager.RemoveListener<ScoreChangedEvent>(OnScoreChanged);
        }
        
        private void InitializeUI()
        {
            _scoreSystem = ServiceLocator.Get<ScoreSystem>();
            
            if (_uiDocument != null)
            {
                _root = _uiDocument.rootVisualElement;
                SetupHighscoreDisplay();
            }
        }
        
        private void SetupHighscoreDisplay()
        {
            // Create NES-style highscore display
            _titleLabel = new Label("HIGH SCORES");
            _titleLabel.AddToClassList("highscore-title");
            
            _highscoreList = new ScrollView();
            _highscoreList.AddToClassList("highscore-list");
            
            _backButton = new Button(OnBackClicked);
            _backButton.text = "BACK";
            _backButton.AddToClassList("nes-button");
            
            _clearButton = new Button(OnClearClicked);
            _clearButton.text = "CLEAR";
            _clearButton.AddToClassList("nes-button");
            
            // Layout
            _root.Add(_titleLabel);
            _root.Add(_highscoreList);
            
            var buttonContainer = new VisualElement();
            buttonContainer.AddToClassList("button-container");
            buttonContainer.Add(_backButton);
            buttonContainer.Add(_clearButton);
            _root.Add(buttonContainer);
            
            RefreshHighscores();
        }
        
        private void RefreshHighscores()
        {
            if (_scoreSystem == null || _highscoreList == null) return;
            
            _highscoreList.Clear();
            
            var highscores = _scoreSystem.Highscores;
            
            if (highscores.Length == 0)
            {
                var noScoresLabel = new Label("NO HIGH SCORES YET");
                noScoresLabel.AddToClassList("no-scores");
                _highscoreList.contentContainer.Add(noScoresLabel);
                return;
            }
            
            // Create NES-style highscore entries
            for (int i = 0; i < highscores.Length; i++)
            {
                var entry = highscores[i];
                var entryElement = CreateHighscoreEntry(i + 1, entry);
                _highscoreList.contentContainer.Add(entryElement);
            }
        }
        
        private VisualElement CreateHighscoreEntry(int rank, HighscoreEntry entry)
        {
            var container = new VisualElement();
            container.AddToClassList("highscore-entry");
            
            // Rank
            var rankLabel = new Label($"{rank:D2}");
            rankLabel.AddToClassList("rank");
            
            // Player name (8-character NES style)
            var nameLabel = new Label(entry.PlayerName.PadRight(8).Substring(0, 8).ToUpper());
            nameLabel.AddToClassList("player-name");
            
            // Score
            var scoreLabel = new Label($"{entry.Score:D7}");
            scoreLabel.AddToClassList("score");
            
            // Level
            var levelLabel = new Label($"L{entry.Level:D2}");
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
            // Return to main menu
            GameEventManager.Publish(new GameStateChangedEvent(GameState.MainMenu));
        }
        
        private void OnClearClicked()
        {
            // Clear highscores with confirmation
            if (_scoreSystem != null)
            {
                _scoreSystem.ClearHighscores();
                RefreshHighscores();
            }
        }
        
        private void OnScoreChanged(ScoreChangedEvent e)
        {
            // Update if this is a new highscore
            if (_scoreSystem != null && _scoreSystem.IsNewHighscore(e.CurrentScore))
            {
                RefreshHighscores();
            }
        }
    }
}
