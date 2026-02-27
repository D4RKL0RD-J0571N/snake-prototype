using NUnit.Framework;
using SnakePrototype.Systems.Score;
using SnakePrototype.Events;
using SnakePrototype.Core;

namespace SnakePrototype.Tests.EditMode
{
    [TestFixture]
    public class ScoreManagerTests
    {
        private ScoreManager _scoreManager;

        [SetUp]
        public void SetUp()
        {
            _scoreManager = new ScoreManager();
            _scoreManager.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            _scoreManager.Shutdown();
        }

        [Test]
        public void ScoreManager_EnergyCollected_IncrementsScore()
        {
            int publishedScore = -1;
            GameEventManager.AddListener<ScoreChangedEvent>(e => publishedScore = e.CurrentScore);

            // EnergyCollectedEvent(value, pos)
            GameEventManager.Publish(new EnergyCollectedEvent(10, UnityEngine.Vector2Int.zero));

            Assert.AreEqual(10, publishedScore);
            
            GameEventManager.Publish(new EnergyCollectedEvent(5, UnityEngine.Vector2Int.zero));
            Assert.AreEqual(15, publishedScore);
        }
    }
}
