using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Events;
using SnakePrototype.Systems.Level;

namespace SnakePrototype.Systems.Environment
{
    [AddComponentMenu("Snake Prototype/Systems/Environment/Lighting Manager")]
    public class LightingManager : IGameService
    {
        #region Fields
        private Color _normalColor = new Color(0.1f, 0.1f, 0.2f);
        private Color _alertColor = Color.red;
        private Color _targetNormalColor;
        private Color _targetAlertColor;
        private float _currentDetection;
        private float _pulseTimer;
        
        private Light _globalLight;
        private float _baseIntensity = 0.5f;
        #endregion

        #region Initialization
        public void Initialize()
        {
            Debug.Log("LightingManager Initialized");
            GameEventManager.AddListener<DetectionLevelChangedEvent>(OnDetectionChanged);
            GameEventManager.AddListener<RespawnEvent>(OnRespawn);
            GameEventManager.AddListener<PaletteChangedEvent>(OnPaletteChanged);
            GameEventManager.AddListener<DetectedEvent>(OnDetected);
            
            _targetNormalColor = _normalColor;
            _targetAlertColor = _alertColor;

            // Try to find a global light if not assigned
            if (_globalLight == null)
            {
                var lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
                if (lights.Length > 0) _globalLight = lights[0];
            }
        }

        public void Shutdown()
        {
            GameEventManager.RemoveListener<DetectionLevelChangedEvent>(OnDetectionChanged);
            GameEventManager.RemoveListener<RespawnEvent>(OnRespawn);
            GameEventManager.RemoveListener<PaletteChangedEvent>(OnPaletteChanged);
            GameEventManager.RemoveListener<DetectedEvent>(OnDetected);
        }
        #endregion

        #region Core Logic
        public void Tick(float deltaTime)
        {
            _pulseTimer += deltaTime;

            // Smooth cross-fade to new palette colors
            _normalColor = Color.Lerp(_normalColor, _targetNormalColor, deltaTime * 2f);
            _alertColor = Color.Lerp(_alertColor, _targetAlertColor, deltaTime * 2f);
            
            // Calculate target lighting colors and intensities based on detection
            float stress = _currentDetection / 100f;
            
            // Ambient shift
            RenderSettings.ambientLight = Color.Lerp(_normalColor, _alertColor, stress * 0.5f);
            
            if (_globalLight != null)
            {
                // Pulse effect when stressed
                float pulse = 1f;
                if (stress > 0.4f)
                {
                    float speed = 2f + (stress * 8f); // Faster pulse as stress increases
                    pulse = 1f + Mathf.Sin(_pulseTimer * speed) * 0.2f * stress;
                }
                
                _globalLight.color = Color.Lerp(_normalColor, _alertColor, stress);
                _globalLight.intensity = _baseIntensity * pulse;
            }
        }

        private void OnPaletteChanged(PaletteChangedEvent e)
        {
            _targetNormalColor = e.Palette.Background * 3f; 
            _targetAlertColor = e.Palette.AlertAccent;
        }
        #endregion

        #region Event Handlers
        private void OnDetectionChanged(DetectionLevelChangedEvent e)
        {
            _currentDetection = e.DetectionLevel;
        }

        private void OnRespawn(RespawnEvent e)
        {
            _currentDetection = 0;
            _pulseTimer = 0;
        }

        private void OnDetected(DetectedEvent e)
        {
            if (_globalLight != null)
            {
                // Instant red flash
                _globalLight.color = _alertColor * 2f;
                _globalLight.intensity = _baseIntensity * 3f;
            }
        }
        #endregion
    }
}
