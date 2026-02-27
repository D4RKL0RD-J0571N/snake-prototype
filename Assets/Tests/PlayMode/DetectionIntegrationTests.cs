using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using SnakePrototype.Core;
using SnakePrototype.Systems.Detection;
using SnakePrototype.Systems.Snake;
using SnakePrototype.Events;
using SnakePrototype.Systems.Grid;

namespace SnakePrototype.Tests.PlayMode
{
    public class DetectionIntegrationTests
    {
        private DetectionManager _detectionManager;
        private GameObject _sourceObj;
        private DetectionSource _source;
        private GameObject _snakeObj;
        private SnakeView _snakeView;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            ServiceLocator.ShutdownAll();

            // Setup Dependencies
            var gridConfig = ScriptableObject.CreateInstance<GridConfiguration>();
            var gridManager = new GridManager(gridConfig);
            ServiceLocator.Register(gridManager);

            var snakeConfig = ScriptableObject.CreateInstance<SnakeConfiguration>();
            var snakeManager = new SnakeManager(snakeConfig);
            snakeManager.Initialize(); // Added
            ServiceLocator.Register(snakeManager);

            _detectionManager = new DetectionManager();
            ServiceLocator.Register(_detectionManager);

            // Setup Source
            _sourceObj = new GameObject("TestSource");
            _source = _sourceObj.AddComponent<DetectionSource>();
            _source.ViewAngle = 90f;
            _source.ViewRange = 10f;
            _source.BuildRate = 100f;
            _source.ObstacleMask = 0;

            // Setup Snake Head
            _snakeObj = new GameObject("TestSnake");
            _snakeView = _snakeObj.AddComponent<SnakeView>();
            
            // Assign dummy prefab and container
            var dummyPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dummyPrefab.SetActive(false);
            _snakeView.GetType().GetField("_segmentPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(_snakeView, dummyPrefab);
            
            var container = new GameObject("Container").transform;
            container.SetParent(_snakeObj.transform);
            _snakeView.GetType().GetField("_segmentContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(_snakeView, container);
            
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Object.Destroy(_sourceObj);
            Object.Destroy(_snakeObj);
            ServiceLocator.ShutdownAll();
            yield return null;
        }

        [UnityTest]
        public IEnumerator DetectionSource_SnakeInFOV_IncreasesDetection()
        {
            // The Snake starts at grid (5,5) which is world (5, 0.2, 5)
            // Position source to see that area
            _sourceObj.transform.position = new Vector3(5, 0, 0);
            _sourceObj.transform.forward = Vector3.forward;
            _source.ViewAngle = 180f; // Wide FOV for test robustness
            _source.ViewRange = 20f;

            float initialDetection = 0f;
            GameEventManager.AddListener<DetectionLevelChangedEvent>(e => initialDetection = e.DetectionLevel);

            // Manual step to ensure detection logic runs
            _source.ForceCheck();

            Assert.Greater(initialDetection, 0, "Detection level should increase when snake is in FOV");
            yield return null;
        }

        [UnityTest]
        public IEnumerator DetectionSource_SnakeBehindSource_NoDetection()
        {
            // Position snake behind source
            _snakeObj.transform.position = new Vector3(0, 0, -5);
            _sourceObj.transform.position = Vector3.zero;
            _sourceObj.transform.forward = Vector3.forward;

            float currentDetection = 0f;
            GameEventManager.AddListener<DetectionLevelChangedEvent>(e => currentDetection = e.DetectionLevel);

            _source.ForceCheck();

            Assert.AreEqual(0, currentDetection, "Detection level should not increase when snake is behind source");
            yield return null;
        }
    }
}
