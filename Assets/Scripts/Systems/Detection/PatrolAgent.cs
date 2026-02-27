using UnityEngine;
using System.Collections.Generic;
using SnakePrototype.Core;
using SnakePrototype.Systems.Grid;
using SnakePrototype.Systems.Environment;
using SnakePrototype.Events;

namespace SnakePrototype.Systems.Detection
{
    /// <summary>
    /// Handles AI guard movement along a set of waypoints.
    /// Integrates with DetectionSource for state transitions.
    /// </summary>
    [AddComponentMenu("SnakePrototype/Systems/Detection/Patrol Agent")]
    public class PatrolAgent : MonoBehaviour
    {
        public enum AIState
        {
            Patrol,
            Alert,
            Search
        }

        [Header("Movement")]
        public List<Vector2Int> Waypoints = new();
        public float MoveSpeed = 2f;
        public float WaitTime = 1f;

        [Header("Visuals")]
        private Renderer[] _renderers;
        private MaterialPropertyBlock _mpb;
        private Palette _currentPalette;

        private AIState _currentState = AIState.Patrol;
        private int _currentWaypointIndex = 0;
        private float _waitTimer = 0f;
        private Vector3 _targetPosition;
        private GridManager _grid;

        private void Start()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            _mpb = new MaterialPropertyBlock();
            _grid = ServiceLocator.Get<GridManager>();
            
            GameEventManager.AddListener<PaletteChangedEvent>(OnPaletteChanged);
            
            if (Waypoints.Count > 0 && _grid != null)
            {
                transform.position = _grid.GetWorldPosition(Waypoints[0]);
                SetNextWaypoint();
            }
        }

        private void OnDestroy()
        {
            GameEventManager.RemoveListener<PaletteChangedEvent>(OnPaletteChanged);
        }

        private void OnPaletteChanged(PaletteChangedEvent e)
        {
            _currentPalette = e.Palette;
            UpdateMaterials();
        }

        private void UpdateMaterials()
        {
            if (_currentPalette == null || _renderers == null) return;
            
            _mpb.SetColor("_BaseColor", _currentPalette.GuardAccent);
            _mpb.SetColor("_AlertColor", _currentPalette.AlertAccent);
            
            foreach (var r in _renderers) r.SetPropertyBlock(_mpb);
        }

        private void Update()
        {
            float alertLevel = 0;
            var detection = ServiceLocator.Get<DetectionManager>();
            if (detection != null) alertLevel = detection.CurrentLevel / 100f;

            if (_mpb != null && _renderers != null)
            {
                _mpb.SetFloat("_AlertIntensity", alertLevel);
                foreach (var r in _renderers) r.SetPropertyBlock(_mpb);
            }

            switch (_currentState)
            {
                case AIState.Patrol:
                    UpdatePatrol();
                    break;
                case AIState.Alert:
                    // Stop and stare?
                    break;
            }
        }

        private void UpdatePatrol()
        {
            if (Waypoints.Count < 2) return;

            if (Vector3.Distance(transform.position, _targetPosition) < 0.05f)
            {
                _waitTimer += Time.deltaTime;
                if (_waitTimer >= WaitTime)
                {
                    _waitTimer = 0f;
                    _currentWaypointIndex = (_currentWaypointIndex + 1) % Waypoints.Count;
                    SetNextWaypoint();
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, _targetPosition, MoveSpeed * Time.deltaTime);
                
                // Rotation
                Vector3 direction = (_targetPosition - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10f);
                }
            }
        }

        private void SetNextWaypoint()
        {
            if (_grid != null && _currentWaypointIndex < Waypoints.Count)
            {
                _targetPosition = _grid.GetWorldPosition(Waypoints[_currentWaypointIndex]);
            }
        }

        public void SetState(AIState newState)
        {
            _currentState = newState;
            Debug.Log($"Guard {name} state: {newState}");
        }

        private void OnDrawGizmos()
        {
            if (Waypoints == null || Waypoints.Count == 0) return;
            
            Gizmos.color = Color.blue;
            for (int i = 0; i < Waypoints.Count; i++)
            {
                Vector3 pos = new Vector3(Waypoints[i].x, 0.5f, Waypoints[i].y);
                Gizmos.DrawSphere(pos, 0.2f);
                
                if (i < Waypoints.Count - 1)
                {
                    Vector3 next = new Vector3(Waypoints[i+1].x, 0.5f, Waypoints[i+1].y);
                    Gizmos.DrawLine(pos, next);
                }
                else if (Waypoints.Count > 2)
                {
                    Vector3 first = new Vector3(Waypoints[0].x, 0.5f, Waypoints[0].y);
                    Gizmos.DrawLine(pos, first);
                }
            }
        }
    }
}
