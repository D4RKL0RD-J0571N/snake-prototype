using UnityEngine;

namespace SnakePrototype.Utils
{
    public class Billboard : MonoBehaviour
    {
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void LateUpdate()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
                if (_camera == null) return;
            }

            transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward,
                             _camera.transform.rotation * Vector3.up);
        }
    }
}
