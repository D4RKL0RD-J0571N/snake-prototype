using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Events;
using SnakePrototype.Systems.Environment;

namespace SnakePrototype.Systems.Detection
{
    /// <summary>
    /// Visualizes a detection cone using a LineRenderer.
    /// Can be used for static cameras or patrolling guards.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class DetectionConeView : MonoBehaviour
    {
        [Header("Cone Settings")]
        public float Angle = 45f;
        public float Range = 10f;
        public int Segments = 20;

        [Header("Visuals")]
        public Color SafeColor = Color.green;
        public Color DetectedColor = Color.red;

        private MaterialPropertyBlock _mpb;
        private Palette _currentPalette;
        private LineRenderer _lineRenderer;
        private float _lastDetectionLevel = -1f;
        private Color _lastColor = Color.clear;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.useWorldSpace = false;
            _lineRenderer.loop = true;
            _lineRenderer.positionCount = Segments + 2;
            
            // Fix rendering issues
            _lineRenderer.startWidth = 0.1f;
            _lineRenderer.endWidth = 0.1f;
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _lineRenderer.material.renderQueue = 3000; // Transparent queue
            _lineRenderer.sortingOrder = 1;
            _lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _lineRenderer.receiveShadows = false;

            _mpb = new MaterialPropertyBlock();
        }

        private void OnEnable()
        {
            GameEventManager.AddListener<PaletteChangedEvent>(OnPaletteChanged);
        }

        private void OnDisable()
        {
            GameEventManager.RemoveListener<PaletteChangedEvent>(OnPaletteChanged);
        }

        private void OnPaletteChanged(PaletteChangedEvent e)
        {
            _currentPalette = e.Palette;
            SafeColor = e.Palette.GuardAccent;
            SafeColor.a = 0.3f; // Semi-transparent
            DetectedColor = e.Palette.AlertAccent;
            DetectedColor.a = 0.6f;
            
            UpdateVisuals(0);
        }

        private void Update()
        {
            // Only update visuals if detection level actually changed
            var detection = ServiceLocator.Get<DetectionManager>();
            if (detection != null)
            {
                float currentDetection = detection.CurrentLevel / 100f;
                
                // Only update if detection level changed significantly
                if (Mathf.Abs(currentDetection - _lastDetectionLevel) > 0.01f)
                {
                    UpdateVisuals(currentDetection);
                    _lastDetectionLevel = currentDetection;
                }
            }
            
            // Draw cone geometry (this can stay in Update)
            DrawCone();
        }

        private void UpdateVisuals(float detectionNormalized)
        {
            Color targetColor = Color.Lerp(SafeColor, DetectedColor, detectionNormalized);
            
            if (_lineRenderer != null)
            {
                _lineRenderer.startColor = targetColor;
                _lineRenderer.endColor = targetColor;
                
                // Only update property block if color changed significantly
                if (Color.Equals(_lastColor, targetColor))
                {
                    return;
                }
                
                _lastColor = targetColor;
                
                // Also set color in material property if shader uses it
                _mpb.SetColor("_Color", targetColor);
                _mpb.SetColor("_BaseColor", targetColor);
                
                // Scan speed ramps up with detection
                float scanSpeed = 2.0f + (detectionNormalized * 8.0f);
                _mpb.SetFloat("_ScanSpeed", scanSpeed);
                
                _lineRenderer.SetPropertyBlock(_mpb);
            }
        }

        private void DrawCone()
        {
            float halfAngle = Angle * 0.5f;
            float yOffset = -0.1f; // Slightly below snake/cores (which are at 0.2f)
            _lineRenderer.SetPosition(0, new Vector3(0, yOffset, 0));

            for (int i = 0; i <= Segments; i++)
            {
                float currentAngle = -halfAngle + (Angle * i / Segments);
                float rad = currentAngle * Mathf.Deg2Rad;
                Vector3 pos = new Vector3(Mathf.Sin(rad) * Range, yOffset, Mathf.Cos(rad) * Range);
                _lineRenderer.SetPosition(i + 1, pos);
            }
        }
        
        // Optional: Gizmos for editor
        private void OnDrawGizmos()
        {
            Gizmos.color = SafeColor;
            Vector3 center = transform.position;
            float halfAngle = Angle * 0.5f;

            Vector3 leftRay = Quaternion.Euler(0, -halfAngle, 0) * transform.forward * Range;
            Vector3 rightRay = Quaternion.Euler(0, halfAngle, 0) * transform.forward * Range;

            Gizmos.DrawRay(center, leftRay);
            Gizmos.DrawRay(center, rightRay);
            
            // Draw arc
            Vector3 prev = center + leftRay;
            for (int i = 1; i <= Segments; i++)
            {
                float currentAngle = -halfAngle + (Angle * i / Segments);
                Vector3 next = center + Quaternion.Euler(0, currentAngle, 0) * transform.forward * Range;
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
            Gizmos.DrawLine(prev, center);
        }
    }
}
