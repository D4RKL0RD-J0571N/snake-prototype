using UnityEngine;
using System.Collections.Generic;
using SnakePrototype.Core;
using SnakePrototype.Systems.Grid;
using SnakePrototype.Events;
using SnakePrototype.Systems.Environment;

namespace SnakePrototype.Systems.Grid
{
    /// <summary>
    /// Visual representation of the neon grid using a procedurally generated mesh.
    /// </summary>
    public class GridView : MonoBehaviour, IGameService
    {
        #region Fields
        [Header("Settings")]
        [SerializeField] private Material _gridMaterial;
        
        private MeshRenderer _renderer;
        private MaterialPropertyBlock _mpb;
        #endregion

        #region IGameService
        public void Initialize()
        {
            _renderer = GetComponent<MeshRenderer>();
            if (_renderer == null) _renderer = gameObject.AddComponent<MeshRenderer>();
            
            var filter = GetComponent<MeshFilter>();
            if (filter == null) filter = gameObject.AddComponent<MeshFilter>();

            _mpb = new MaterialPropertyBlock();
            
            GameEventManager.AddListener<PaletteChangedEvent>(OnPaletteChanged);
            GameEventManager.AddListener<LevelGeneratedEvent>(OnLevelGenerated);
            GameEventManager.AddListener<DetectionLevelChangedEvent>(OnDetectionChanged);
            GameEventManager.AddListener<GameStateChangedEvent>(OnGameStateChanged);

            Debug.Log("GridView Initialized.");
        }

        public void Shutdown()
        {
            GameEventManager.RemoveListener<PaletteChangedEvent>(OnPaletteChanged);
            GameEventManager.RemoveListener<LevelGeneratedEvent>(OnLevelGenerated);
            GameEventManager.RemoveListener<DetectionLevelChangedEvent>(OnDetectionChanged);
            GameEventManager.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
        }
        #endregion

        #region Event Handlers
        private void OnLevelGenerated(LevelGeneratedEvent e)
        {
            RebuildGridMesh();
        }

        private void OnPaletteChanged(PaletteChangedEvent e)
        {
            if (_mpb == null) _mpb = new MaterialPropertyBlock();
            
            // Set colors in MPB
            _mpb.SetColor("_BaseColor", e.Palette.GridAccent);
            _mpb.SetColor("_BackgroundColor", e.Palette.Background);
            
            if (_renderer != null)
            {
                // Priority: Inspector assigned material > GridConfig material
                Material mat = _gridMaterial;
                if (mat == null)
                {
                    var grid = ServiceLocator.Get<GridManager>();
                    mat = grid?.Config?.GridMaterial;
                }

                if (mat != null)
                {
                    _renderer.sharedMaterial = mat;
                }
                else
                {
                    Debug.LogWarning("GridView: No Grid Material found in Inspector or GridConfig!");
                }

                _renderer.SetPropertyBlock(_mpb);
            }

            // Update background if possible (e.g. Camera background)
            if (Camera.main != null)
            {
                Camera.main.backgroundColor = e.Palette.Background;
                Camera.main.clearFlags = CameraClearFlags.SolidColor;
            }
        }

        private void OnDetectionChanged(DetectionLevelChangedEvent e)
        {
            float intensity = e.DetectionLevel / 100f;
            if (_mpb != null && _renderer != null)
            {
                _mpb.SetFloat("_AlertIntensity", intensity);
                _renderer.SetPropertyBlock(_mpb);
            }
        }

        private void OnGameStateChanged(GameStateChangedEvent e)
        {
            // Optional visual response to game state
        }
        #endregion

        #region Visual Logic
        private void RebuildGridMesh()
        {
            var grid = ServiceLocator.Get<GridManager>();
            if (grid == null) return;

            var filter = GetComponent<MeshFilter>();
            if (filter == null) return;

            int w = grid.Width;
            int h = grid.Height;
            float s = grid.CellSize;
            
            Mesh mesh = new Mesh();
            mesh.name = "GridSurface";

            float width = w * s;
            float height = h * s;
            
            // Create a plane covering the grid exactly
            Vector3[] verts = new Vector3[4]
            {
                new Vector3(0, -0.1f, 0), 
                new Vector3(width, -0.1f, 0),
                new Vector3(0, -0.1f, height),
                new Vector3(width, -0.1f, height)
            };
            
            Vector2[] uvs = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };

            int[] tris = new int[6]
            {
                0, 2, 1,
                2, 3, 1
            };

            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.triangles = tris;
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            filter.mesh = mesh;
        }
        #endregion
    }
}
