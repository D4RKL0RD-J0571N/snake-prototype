using NUnit.Framework;
using UnityEngine;
using SnakePrototype.Systems.Grid;
using SnakePrototype.Core;

namespace SnakePrototype.Tests.EditMode
{
    [TestFixture]
    public class GridManagerTests
    {
        private GridManager _gridManager;
        private GridConfiguration _config;

        [SetUp]
        public void SetUp()
        {
            _config = ScriptableObject.CreateInstance<GridConfiguration>();
            _config.Width = 10;
            _config.Height = 10;
            _config.CellSize = 1f;

            _gridManager = new GridManager(_config);
            _gridManager.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            _gridManager.Shutdown();
            Object.DestroyImmediate(_config);
        }

        [Test]
        public void GridManager_IsBounds_ReturnsTrue_ForInside()
        {
            Assert.IsTrue(_gridManager.IsBounds(new Vector2Int(0, 0)));
            Assert.IsTrue(_gridManager.IsBounds(new Vector2Int(5, 5)));
            Assert.IsTrue(_gridManager.IsBounds(new Vector2Int(9, 9)));
        }

        [Test]
        public void GridManager_IsBounds_ReturnsFalse_ForOutside()
        {
            Assert.IsFalse(_gridManager.IsBounds(new Vector2Int(-1, 0)));
            Assert.IsFalse(_gridManager.IsBounds(new Vector2Int(0, -1)));
            Assert.IsFalse(_gridManager.IsBounds(new Vector2Int(10, 0)));
            Assert.IsFalse(_gridManager.IsBounds(new Vector2Int(0, 10)));
        }

        [Test]
        public void GridManager_GetWorldPosition_IsCorrect()
        {
            // Grid (1,1) with CellSize 1 should be (1, 0, 1)
            Vector2Int pos = new Vector2Int(1, 1);
            Vector3 worldPos = _gridManager.GetWorldPosition(pos);
            
            Assert.AreEqual(1f, worldPos.x);
            Assert.AreEqual(0f, worldPos.y);
            Assert.AreEqual(1f, worldPos.z);
        }
    }
}
