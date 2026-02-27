using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Events;

namespace SnakePrototype.Systems.UI
{
    /// <summary>
    /// Drives the ScreenOverlay shader properties based on game state and detection level.
    /// </summary>
    [AddComponentMenu("SnakePrototype/UI/Screen Overlay Controller")]
    public class ScreenOverlayController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Renderer _overlayRenderer;
        
        [Header("Settings")]
        [SerializeField] private float _baseAbberation = 0.001f;
        [SerializeField] private float _alertAbberation = 0.015f;
        [SerializeField] private float _baseScanline = 0.3f;
        [SerializeField] private float _alertScanline = 0.8f;

        private MaterialPropertyBlock _mpb;
        private float _currentDetection;
        private float _pulseTimer;
        private float _pulseIntensity;

        private void Start()
        {
            _mpb = new MaterialPropertyBlock();
            GameEventManager.AddListener<DetectionLevelChangedEvent>(OnDetectionChanged);
            GameEventManager.AddListener<DetectedEvent>(OnDetected);
        }

        private void OnDestroy()
        {
            GameEventManager.RemoveListener<DetectionLevelChangedEvent>(OnDetectionChanged);
            GameEventManager.RemoveListener<DetectedEvent>(OnDetected);
        }

        private void OnDetected(DetectedEvent e)
        {
            // Trigger a temporary glitch shock
            _pulseTimer = 0.4f;
            _pulseIntensity = 0.1f;
        }

        private void OnDetectionChanged(DetectionLevelChangedEvent e)
        {
            _currentDetection = e.DetectionLevel;
        }

        private void Update()
        {
            if (_overlayRenderer == null) return;

            float intensity = _currentDetection / 100f;
            float abberation = Mathf.Lerp(_baseAbberation, _alertAbberation, intensity) + (Mathf.Sin(Time.time * 50f) * _pulseIntensity);
            float scanline = Mathf.Lerp(_baseScanline, _alertScanline, intensity);
            
            _overlayRenderer.GetPropertyBlock(_mpb);
            _mpb.SetFloat("_AbberationStrength", abberation);
            _mpb.SetFloat("_ScanlineIntensity", scanline);
            
            // Add extra jitter if detection is high or pulsing
            if (intensity > 0.8f || _pulseTimer > 0)
            {
                float jitter = intensity > 0.8f ? 0.05f : 0.1f;
                _mpb.SetFloat("_FlickerIntensity", jitter + (Mathf.PingPong(Time.time * 20f, 0.1f)));
            }
            else
            {
                _mpb.SetFloat("_FlickerIntensity", 0.02f);
            }

            if (_pulseTimer > 0)
            {
                _pulseTimer -= Time.unscaledDeltaTime;
                _pulseIntensity = Mathf.Lerp(_pulseIntensity, 0f, Time.unscaledDeltaTime * 10f);
            }

            _overlayRenderer.SetPropertyBlock(_mpb);
        }
    }
}
