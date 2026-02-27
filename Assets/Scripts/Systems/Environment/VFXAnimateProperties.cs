using UnityEngine;

namespace SnakePrototype.Systems.Environment
{
    /// <summary>
    /// Simple helper to animate shader properties on a visual effect object.
    /// </summary>
    [AddComponentMenu("SnakePrototype/Environment/VFX Animate Properties")]
    public class VFXAnimateProperties : MonoBehaviour
    {
        public string PropertyName = "_Lifetime";
        public float Duration = 1.0f;
        public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public bool DestroyOnComplete = true;

        private Renderer _renderer;
        private MaterialPropertyBlock _mpb;
        private float _timer;

        private void Awake()
        {
            _renderer = GetComponentInChildren<Renderer>();
            _mpb = new MaterialPropertyBlock();
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(_timer / Duration);
            float value = Curve.Evaluate(normalizedTime);

            if (_renderer != null)
            {
                _renderer.GetPropertyBlock(_mpb);
                _mpb.SetFloat(PropertyName, value);
                _renderer.SetPropertyBlock(_mpb);
            }

            if (_timer >= Duration && DestroyOnComplete)
            {
                Destroy(gameObject);
            }
        }
    }
}
