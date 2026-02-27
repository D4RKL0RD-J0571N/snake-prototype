using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Systems.Snake;

namespace SnakePrototype.Systems.Detection
{
    /// <summary>
    /// Component that reports sightings of the snake to the DetectionManager.
    /// Can represent a security camera, guard, or sensor.
    /// </summary>
    [AddComponentMenu("SnakePrototype/Systems/Detection/Detection Source")]
    public class DetectionSource : MonoBehaviour
    {
        [Header("Detection Settings")]
        public float ViewAngle = 45f;
        public float ViewRange = 10f;
        public float BuildRate = 50f; // Detection gain per second
        public LayerMask ObstacleMask;

        private void Update()
        {
            CheckForSnake();
        }

        public void ForceCheck() => CheckForSnake(); // Explicit for tests

        private void CheckForSnake()
        {
            var snakeView = Object.FindAnyObjectByType<SnakeView>();
            if (snakeView == null || snakeView.HeadTransform == null) return;

            Transform head = snakeView.HeadTransform;
            Vector3 directionToHead = head.position - transform.position;
            float distance = directionToHead.magnitude;

            if (distance <= ViewRange)
            {
                float angle = Vector3.Angle(transform.forward, directionToHead);
                if (angle <= ViewAngle * 0.5f)
                {
                    // Check for obstacles (Grid cells, walls, etc.)
                    if (!Physics.Raycast(transform.position, directionToHead.normalized, distance, ObstacleMask))
                    {
                        // Sighting!
                        var detection = ServiceLocator.Get<DetectionManager>();
                        if (detection != null)
                        {
                            detection.ReportSighting(BuildRate * Time.deltaTime, this);
                        }
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, ViewRange);
            
            Vector3 leftRay = Quaternion.Euler(0, -ViewAngle * 0.5f, 0) * transform.forward * ViewRange;
            Vector3 rightRay = Quaternion.Euler(0, ViewAngle * 0.5f, 0) * transform.forward * ViewRange;
            
            Gizmos.DrawRay(transform.position, leftRay);
            Gizmos.DrawRay(transform.position, rightRay);
        }
    }
}
