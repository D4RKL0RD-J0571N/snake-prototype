using System.Collections.Generic;
using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Events;    
using SnakePrototype.Systems.Grid;
using SnakePrototype.Systems.Environment;

namespace SnakePrototype.Systems.Snake
{
    /// <summary>
    /// Visual bridge for the Snake system. 
    /// Listens for movement events and updates the visual GameObjects.
    /// </summary>
    public class SnakeView : MonoBehaviour
    {
        [Header("Assets")]
        [SerializeField] private SnakeConfiguration _config;
        [SerializeField] private Transform _segmentContainer;

        private List<GameObject> _visualSegments = new();
        public Transform HeadTransform => _visualSegments.Count > 0 ? _visualSegments[0].transform : null;

        private float _lastMoveTime;

private void Update()
{
    var grid = ServiceLocator.Get<GridManager>();
    var snakeManager = ServiceLocator.Get<SnakeManager>();
    
    if (grid == null || snakeManager == null) return;

     var body = snakeManager.BodyParts;
     
     bool countChanged = false;
     // Sync count first
     while (_visualSegments.Count < body.Count)
     {
         // Check Config
         if (_config == null || _config.SegmentPrefab == null)
         {
             Debug.LogError("SnakeView: Config or Segment Prefab is not assigned! Skipping visual update.");
             return;
         }

         // If no container, just spawn in root (or use self as parent?)
         // Ideally we want a clean hierarchy, but lets not fail if it's missing.
         Transform parent = _segmentContainer != null ? _segmentContainer : transform;

         var go = Instantiate(_config.SegmentPrefab, parent);
         // Auto-add billboard
         if (go.GetComponent<SnakePrototype.Utils.Billboard>() == null) go.AddComponent<SnakePrototype.Utils.Billboard>();
         
         // Add Polish
         var polish = go.AddComponent<SnakeVisualPolish>();
         polish.IsHead = _visualSegments.Count == 0;
         
         // Start at the tail (or head if new) to prevent flying in from zero
         if (_visualSegments.Count > 0) go.transform.position = _visualSegments[_visualSegments.Count-1].transform.position;
         else go.transform.position = grid.GetWorldPosition(body[0]);
         
          _visualSegments.Add(go);
          countChanged = true;

          // Apply Materials from Consolidated Config
          var renderer = go.GetComponentInChildren<Renderer>();
          if (renderer != null)
          {
              int index = _visualSegments.Count - 1;
              if (index == 0 && _config.HeadMaterial != null) renderer.sharedMaterial = _config.HeadMaterial;
              else if (_config.BodyMaterial != null) renderer.sharedMaterial = _config.BodyMaterial;
          }
      }
     while (_visualSegments.Count > body.Count)
     {
         Destroy(_visualSegments[_visualSegments.Count - 1]);
         _visualSegments.RemoveAt(_visualSegments.Count - 1);
         countChanged = true;
     }

     if (countChanged) RefreshAllColors();

     if (countChanged) RefreshAllColors();

     // Smooth Move
     float speed = 15f;
     for (int i = 0; i < body.Count; i++)
     {
         Vector3 targetPos = grid.GetWorldPosition(body[i]);
         
         float t = Time.deltaTime * speed;
         _visualSegments[i].transform.position = Vector3.Lerp(_visualSegments[i].transform.position, targetPos, t);
         
         // Ensure Billboard
         if (_visualSegments[i].GetComponent<SnakePrototype.Utils.Billboard>() == null)
            _visualSegments[i].AddComponent<SnakePrototype.Utils.Billboard>();
     }
}





        private MaterialPropertyBlock _mpb;
        private Palette _currentPalette;
        private GameState _currentState;

        private void Start()
        {
            _mpb = new MaterialPropertyBlock();
            GameEventManager.AddListener<GameStateChangedEvent>(OnGameStateChanged);
            GameEventManager.AddListener<PaletteChangedEvent>(OnPaletteChanged);
            GameEventManager.AddListener<LevelStartedEvent>(OnLevelStarted);

            // Sync initial state
            var paletteManager = ServiceLocator.Get<ColorPaletteManager>();
            if (paletteManager != null && paletteManager.CurrentPalette != null)
            {
                _currentPalette = paletteManager.CurrentPalette;
            }
            RefreshAllColors();
        }

        private void OnLevelStarted(LevelStartedEvent e)
        {
            // Snap visual segments immediately to prevent lerping from previous level positions
            var grid = ServiceLocator.Get<GridManager>();
            var snakeManager = ServiceLocator.Get<SnakeManager>();
            if (grid == null || snakeManager == null) return;

            var body = snakeManager.BodyParts;
            // Force sync count first if needed (though Update would do it, we want it NOW for snapping)
            Update(); 

            for (int i = 0; i < _visualSegments.Count && i < body.Count; i++)
            {
                _visualSegments[i].transform.position = grid.GetWorldPosition(body[i]);
            }
        }

        private void OnPaletteChanged(PaletteChangedEvent e)
        {
            _currentPalette = e.Palette;
            RefreshAllColors();
        }

        private void OnGameStateChanged(GameStateChangedEvent e)
        {
            _currentState = e.NewState;
            RefreshAllColors();
        }

        private void RefreshAllColors()
        {
            if (_currentPalette == null)
            {
                var pm = ServiceLocator.Get<ColorPaletteManager>();
                if (pm != null && pm.CurrentPalette != null) _currentPalette = pm.CurrentPalette;
                else return;
            }
            
            // Use CoreAccent for vibrancy
            Color baseColor = _currentPalette.CoreAccent;
            Color emissionColor = _currentPalette.CoreAccent;

            if (_currentState == GameState.GameOver)
            {
                baseColor = _currentPalette.AlertAccent;
                emissionColor = _currentPalette.AlertAccent;
            }
            
            for (int i = 0; i < _visualSegments.Count; i++)
            {
                float ratio = _visualSegments.Count > 1 ? (float)i / (_visualSegments.Count - 1) : 0f;
                SetSegmentProperties(_visualSegments[i], baseColor, emissionColor, ratio);
            }
        }

        private void SetSegmentProperties(GameObject seg, Color color, Color emission, float ratio)
        {
            // 1. Try SpriteRenderer (Common for 2D prototypes)
            var spriteRenderer = seg.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }

            // 2. Try MaterialPropertyBlock for MeshRenderers
            var renderer = seg.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                if (_mpb == null) _mpb = new MaterialPropertyBlock();
                
                _mpb.SetColor("_BaseColor", color);
                _mpb.SetColor("_Color", color);
                _mpb.SetColor("_EmissionColor", emission);
                _mpb.SetFloat("_GradientRatio", ratio);
                renderer.SetPropertyBlock(_mpb);
            }

            if (spriteRenderer == null && renderer == null)
            {
                Debug.LogWarning($"SnakeView: No renderer found on {seg.name} or children!");
            }
        }

        private void OnDestroy()
        {
            GameEventManager.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
            GameEventManager.RemoveListener<PaletteChangedEvent>(OnPaletteChanged);
            GameEventManager.RemoveListener<LevelStartedEvent>(OnLevelStarted);
        }
    }
}
