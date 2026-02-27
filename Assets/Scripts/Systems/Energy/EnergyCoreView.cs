using System.Collections.Generic;
using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Events;
using SnakePrototype.Systems.Grid;
using SnakePrototype.Systems.Environment;

namespace SnakePrototype.Systems.Energy
{
    public class EnergyCoreView : MonoBehaviour
    {
        [SerializeField] private GameObject _corePrefab;
        private MaterialPropertyBlock _mpb;
        private Palette _currentPalette;
        private Dictionary<Vector2Int, GameObject> _activeCores = new();

        private void Start()
        {
            _mpb = new MaterialPropertyBlock();
            GameEventManager.AddListener<EnergySpawnedEvent>(OnEnergySpawned);
            GameEventManager.AddListener<EnergyCollectedEvent>(OnEnergyCollected);
            GameEventManager.AddListener<EnergyCoresResetEvent>(OnCoresReset);
            GameEventManager.AddListener<PaletteChangedEvent>(OnPaletteChanged);
            
            // Initial Sync
            var manager = ServiceLocator.Get<EnergyCoreManager>();
            if (manager != null)
            {
                foreach (var pos in manager.ActiveCores)
                {
                    OnEnergySpawned(new EnergySpawnedEvent(pos));
                }
            }
        }

        private void OnPaletteChanged(PaletteChangedEvent e)
        {
            _currentPalette = e.Palette;
            ApplyPaletteToAll();
        }

        private void ApplyPaletteToAll()
        {
            if (_currentPalette == null) return;
            foreach (var go in _activeCores.Values)
            {
                ApplyPaletteToCore(go);
            }
        }

        private void ApplyPaletteToCore(GameObject go)
        {
            if (go == null || _currentPalette == null) return;
            var renderer = go.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                if (_mpb == null) _mpb = new MaterialPropertyBlock();
                _mpb.SetColor("_BaseColor", _currentPalette.CoreAccent);
                _mpb.SetFloat("_PulseSpeed", 2.0f);
                _mpb.SetFloat("_GlowIntensity", 1.5f);
                renderer.SetPropertyBlock(_mpb);
            }
        }

        private void OnCoresReset(EnergyCoresResetEvent e)
        {
            ClearAll();
        }

        private void OnEnergySpawned(EnergySpawnedEvent e)
        {
            if (_activeCores.ContainsKey(e.Position)) return;

            var grid = ServiceLocator.Get<GridManager>();
            if (grid != null && _corePrefab != null)
            {
                Vector3 worldPos = grid.GetWorldPosition(e.Position);
                var go = Instantiate(_corePrefab, worldPos, Quaternion.identity, transform);
                
                if (go.GetComponent<SnakePrototype.Utils.Billboard>() == null)
                {
                    go.AddComponent<SnakePrototype.Utils.Billboard>();
                }
                
                _activeCores[e.Position] = go;
                ApplyPaletteToCore(go);
            }
        }

        private void OnEnergyCollected(EnergyCollectedEvent e)
        {
            if (_activeCores.TryGetValue(e.Position, out var go))
            {
                Destroy(go);
                _activeCores.Remove(e.Position);
            }
        }

        private void ClearAll()
        {
            foreach (var go in _activeCores.Values)
            {
                if (go != null) Destroy(go);
            }
            _activeCores.Clear();
        }

        private void OnDestroy()
        {
            GameEventManager.RemoveListener<EnergySpawnedEvent>(OnEnergySpawned);
            GameEventManager.RemoveListener<EnergyCollectedEvent>(OnEnergyCollected);
            GameEventManager.RemoveListener<EnergyCoresResetEvent>(OnCoresReset);
            GameEventManager.RemoveListener<PaletteChangedEvent>(OnPaletteChanged);
        }
    }
}
