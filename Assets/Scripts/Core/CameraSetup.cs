using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Events;
using SnakePrototype.Systems.Grid;
using SnakePrototype.Systems.Snake;

namespace SnakePrototype.Core
{
    /// <summary>
    /// Handles camera movement and cinematic transitions.
    /// </summary>
    public class CameraSetup : MonoBehaviour
    {
        [Header("Settings")]
        public Transform Target;
        public Vector3 FollowOffset = new Vector3(0, 18, -2); // Lower for more immersion
        public Vector3 IntroOffset = new Vector3(0, 40, 0); // High and centered
        public float SmoothSpeed = 8f;
        public float ZoomSpeed = 2f;

        private Vector3 _currentOffset;
        private Vector3 _targetOffset;
        private bool _isIntro = true;

        private void Start()
        {
            _currentOffset = IntroOffset;
            _targetOffset = IntroOffset;
            
            GameEventManager.AddListener<LevelCompleteEvent>(OnLevelComplete);
            GameEventManager.AddListener<LevelStartedEvent>(OnLevelStarted);
            GameEventManager.AddListener<RespawnEvent>(OnRespawn);
        }

        private void OnDestroy()
        {
            GameEventManager.RemoveListener<LevelCompleteEvent>(OnLevelComplete);
            GameEventManager.RemoveListener<LevelStartedEvent>(OnLevelStarted);
            GameEventManager.RemoveListener<RespawnEvent>(OnRespawn);
        }

        private void OnLevelComplete(LevelCompleteEvent e)
        {
            _isIntro = true;
            _targetOffset = IntroOffset;
        }

        private void OnLevelStarted(LevelStartedEvent e)
        {
            _isIntro = false;
            _targetOffset = FollowOffset;
            FindNewTarget();
        }

        private void OnRespawn(RespawnEvent e)
        {
             _isIntro = true;
             _targetOffset = IntroOffset;
        }

        private void LateUpdate()
        {
            _currentOffset = Vector3.Lerp(_currentOffset, _targetOffset, Time.unscaledDeltaTime * ZoomSpeed);

            if (_isIntro)
            {
                // Center on grid if possible
                var grid = ServiceLocator.Get<GridManager>();
                // Center on grid (World (0,0,0) is center)
                if (grid != null)
                {
                    Vector3 gridCenter = Vector3.zero; 
                    transform.position = Vector3.Lerp(transform.position, gridCenter + _currentOffset, SmoothSpeed * Time.unscaledDeltaTime);
                }
            }
            else if (Target != null && Target.gameObject.activeInHierarchy)
            {
                // Follow Snake
                Vector3 desiredPos = Target.position + _currentOffset;
                transform.position = Vector3.Lerp(transform.position, desiredPos, SmoothSpeed * Time.deltaTime);
            }
            else
            {
                FindNewTarget();
            }

            // Maintain strict top-down orientation
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        private void FindNewTarget()
        {
            var snakeView = Object.FindAnyObjectByType<SnakeView>();
            if (snakeView != null && snakeView.HeadTransform != null)
            {
                Target = snakeView.HeadTransform;
            }
        }
    }
}
