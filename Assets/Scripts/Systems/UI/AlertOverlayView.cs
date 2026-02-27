using UnityEngine;
using UnityEngine.UIElements;
using SnakePrototype.Core;
using SnakePrototype.Events;

namespace SnakePrototype.Systems.UI
{
    /// <summary>
    /// Logic for a full-screen alert overlay that pulses/tints based on detection level.
    /// </summary>
    public class AlertOverlayView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private string _overlayElementName = "AlertOverlay";

        [Header("Settings")]
        [SerializeField] private Color _tintColor = new Color(1, 0, 0, 0.3f);
        [SerializeField] private float _pulseFrequency = 2f;

        private VisualElement _overlay;

        private void Start()
        {
            if (_uiDocument != null)
            {
                _overlay = _uiDocument.rootVisualElement.Q<VisualElement>(_overlayElementName);
            }
            GameEventManager.AddListener<DetectionLevelChangedEvent>(OnDetectionChanged);
        }

        private void OnDestroy()
        {
            GameEventManager.RemoveListener<DetectionLevelChangedEvent>(OnDetectionChanged);
        }

        private void OnDetectionChanged(DetectionLevelChangedEvent e)
        {
            if (_overlay == null) return;

            float detectionPercent = e.DetectionLevel / 100f;
            
            if (detectionPercent > 0.1f)
            {
                _overlay.style.display = DisplayStyle.Flex;
                
                // Pulse opacity
                float pulse = 0.5f + (Mathf.Sin(Time.time * _pulseFrequency) * 0.5f);
                float alpha = detectionPercent * 0.5f * pulse;
                
                _overlay.style.backgroundColor = new Color(_tintColor.r, _tintColor.g, _tintColor.b, alpha);
            }
            else
            {
                _overlay.style.display = DisplayStyle.None;
            }
        }
    }
}
