using NUnit.Framework;
using UnityEngine;
using SnakePrototype.Systems.Snake;
using SnakePrototype.Core;
using SnakePrototype.Systems.Grid;
using System.Linq;
using SnakePrototype.Events;

namespace SnakePrototype.Tests.EditMode
{
    [TestFixture]
    public class SnakeManagerTests
    {
        private SnakeManager _snakeManager;
        private SnakeConfiguration _config;
        private GridManager _gridManager;

        [SetUp]
        public void SetUp()
        {
            _config = ScriptableObject.CreateInstance<SnakeConfiguration>();
            _config.StartLength = 3;
            _config.MoveInterval = 0.1f;

            _gridManager = new GridManager(ScriptableObject.CreateInstance<GridConfiguration>());
            ServiceLocator.Register(_gridManager);

            _snakeManager = new SnakeManager(_config);
            _snakeManager.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            _snakeManager.Shutdown();
            ServiceLocator.ShutdownAll();
            Object.DestroyImmediate(_config);
        }

        [Test]
        public void SnakeManager_InitialState_IsCorrect()
        {
            Assert.AreEqual(3, _snakeManager.BodyParts.Count);
            // Default start position is (5,5) in SnakeManager
            Assert.AreEqual(new Vector2Int(5, 5), _snakeManager.HeadPosition);
        }

        [Test]
        public void SnakeManager_Move_AdvancesOneStep()
        {
            Vector2Int initialHead = _snakeManager.HeadPosition;
            
            // Advance by MoveInterval
            _snakeManager.Tick(_config.MoveInterval + 0.01f);
            
            // Default direction is Right (1,0)
            Assert.AreEqual(initialHead + Vector2Int.right, _snakeManager.HeadPosition);
        }

        [Test]
        public void SnakeManager_ResetSnake_RestoresDefaults()
        {
            // Move once
            _snakeManager.Tick(_config.MoveInterval + 0.15f);
            Assert.AreNotEqual(new Vector2Int(5, 5), _snakeManager.HeadPosition);

            // Trigger reset via initialize for test purposes
            _snakeManager.Initialize(); 
            
            Assert.AreEqual(new Vector2Int(5, 5), _snakeManager.HeadPosition);
            Assert.AreEqual(3, _snakeManager.BodyParts.Count);
        }

        [Test]
        public void SnakeManager_SelfCollision_TriggersGameOver()
        {
            // Initial: (5,5), (4,5), (3,5). Facing Right.
            // Move: Right -> (6,5)
            // Turn: Up -> (6,6)
            // Turn: Left -> (5,6)
            // Turn: Down -> Hit (5,5) - WAIT, body (6,6), (6,5), (5,5)
            // Let's do a tighter loop:
            // 4 segments: (5,5), (4,5), (3,5), (2,5)
            // 1. Right -> (6,5)
            // 2. Up -> (6,6)
            // 3. Left -> (5,6)
            // 4. Down -> (5,5) - Collision!
            
            _config.StartLength = 4;
            _snakeManager.Initialize(); // Re-init with 4

            bool gameOverPublished = false;
            GameEventManager.AddListener<GameStateChangedEvent>(e => {
                if (e.NewState == GameState.GameOver) gameOverPublished = true;
            });

            // 1. Tick
            _snakeManager.Tick(_config.MoveInterval + 0.01f); // (6,5)
            
            // 2. Turn Up
            _snakeManager.HandleInput(Vector2Int.up);
            _snakeManager.Tick(_config.MoveInterval + 0.01f); // (6,6)
            
            // 3. Turn Left
            _snakeManager.HandleInput(Vector2Int.left);
            _snakeManager.Tick(_config.MoveInterval + 0.01f); // (5,6)
            
            // 4. Turn Down
            _snakeManager.HandleInput(Vector2Int.down);
            _snakeManager.Tick(_config.MoveInterval + 0.01f); // (5,5) - BOOM
            // The logic above should have triggered Die() in Move().
            
            Assert.IsTrue(gameOverPublished, "GameStateChangedEvent(GameOver) should be published on self-collision");
        }
    }
}
