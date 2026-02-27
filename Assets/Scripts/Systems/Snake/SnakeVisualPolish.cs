using UnityEngine;

namespace SnakePrototype.Systems.Snake
{
    /// <summary>
    /// Adds subtle 3D animations and visual polish to snake segments.
    /// </summary>
    [AddComponentMenu("SnakePrototype/Snake/Snake Visual Polish")]
    public class SnakeVisualPolish : MonoBehaviour
    {
        public bool IsHead = false;
        public float BobAmplitude = 0.05f;
        public float BobSpeed = 3f;
        public float RotationWobble = 5f;

        private Renderer _renderer;
        private MaterialPropertyBlock _mpb;
        private Vector3 _startLocalPos;
        private Quaternion _startLocalRot;
        private float _randomOffset;
        private float _dissolveValue = 1.0f;

        private void Start()
        {
            _renderer = GetComponentInChildren<Renderer>();
            _mpb = new MaterialPropertyBlock();
            _startLocalPos = transform.localPosition;
            _startLocalRot = transform.localRotation;
            _randomOffset = Random.value * Mathf.PI * 2;
            
            // Start dissolved in
            _dissolveValue = 1.0f;
        }

        private void Update()
        {
            float time = Time.time * BobSpeed + _randomOffset;
            
            // Subtle bobbing
            float bob = Mathf.Sin(time) * BobAmplitude;
            //transform.localPosition = _startLocalPos + Vector3.up * bob; // Old: Overwrites entire pos
            
            // New: Respect current X/Z, only modify Y offset relative to parent/world (or whatever base is)
            // But SnakeView moves transform.position.
            // If Billboard is active, it handles rotation.
            
            Vector3 currentPos = transform.localPosition;
            currentPos.y = _startLocalPos.y + bob; // Only override Y relative to initial Y
            transform.localPosition = currentPos;

            // Subtle rotation wobble
            float wobble = Mathf.Cos(time * 0.5f) * RotationWobble;
            //transform.localRotation = _startLocalRot * Quaternion.Euler(0, wobble, 0); // Old
            // New: Add wobble to current rotation? Or if Billboard sets rotation, this fights.
            // Let's assume Billboard wins if present, or we add local rotation on top.
            // For now, let's keep rotation simple to avoid fighting Billboard too much.
            // If No Billboard, we can wobble.
            if (GetComponent<SnakePrototype.Utils.Billboard>() == null)
            {
                transform.localRotation = _startLocalRot * Quaternion.Euler(0, wobble, 0);
            }
            else
            {
                // Billboard handles rotation, maybe rotate *child* mesh if we could?
                // Or just wobble Z (roll)? Billboards usually face camera (Y/X constrained).
                // Let's skip rotation wobble if Billboard is present to be safe.
            }

            // Scale pulsing for head
            if (IsHead)
            {
                float pulse = 1.0f + Mathf.Sin(time * 2f) * 0.02f;
                transform.localScale = Vector3.one * pulse;
            }

            // Smooth dissolve in
            if (_dissolveValue > 0)
            {
                _dissolveValue = Mathf.MoveTowards(_dissolveValue, 0f, Time.deltaTime * 2f);
                if (_renderer != null)
                {
                    _renderer.GetPropertyBlock(_mpb);
                    _mpb.SetFloat("_Dissolve", _dissolveValue);
                    _renderer.SetPropertyBlock(_mpb);
                }
            }
        }
    }
}
