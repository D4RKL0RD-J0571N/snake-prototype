using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Events;

namespace SnakePrototype.Systems.UI
{
    /// <summary>
    /// Adds a brief "glitch" pulse when the snake is first spotted.
    /// </summary>
    [AddComponentMenu("SnakePrototype/UI/Glitch Pulse")]
    public class GlitchPulse : MonoBehaviour
    {
        [SerializeField] private ScreenOverlayController _overlay;
        [SerializeField] private float _pulseDuration = 0.5f;
        [SerializeField] private AnimationCurve _pulseCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        private float _timer = 0f;
        private bool _isPulsing = false;

        private void Start()
        {
            GameEventManager.AddListener<DetectedEvent>(OnDetected);
        }

        private void OnDestroy()
        {
            GameEventManager.RemoveListener<DetectedEvent>(OnDetected);
        }

        private void OnDetected(DetectedEvent e)
        {
            _timer = _pulseDuration;
            _isPulsing = true;
        }

        private void Update()
        {
            if (!_isPulsing || _overlay == null) return;

            _timer -= Time.unscaledDeltaTime;
            float percent = Mathf.Clamp01(_timer / _pulseDuration);
            float intensity = _pulseCurve.Evaluate(1.0f - percent);

            // We could add a "GlitchOverride" property to the shader or just additive boost
            // For now, let's assume we can trigger a flash or jitter
            
            if (_timer <= 0) _isPulsing = false;
        }
    }
}
