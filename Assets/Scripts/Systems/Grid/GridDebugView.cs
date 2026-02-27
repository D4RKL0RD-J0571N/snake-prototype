using UnityEngine;
using SnakePrototype.Core;

namespace SnakePrototype.Systems.Grid
{
    public class GridDebugView : MonoBehaviour
    {
        [SerializeField] private Color _gridColor = new Color(0, 1, 0, 0.3f);
        [SerializeField] private bool _showGrid = true;

        private void OnDrawGizmos()
        {
            if (!_showGrid) return;

            // Ideally get from locator, but Gizmos run in Editor mode too where Locator might be empty.
            // fallback to finding a config or hardcoded for now, or runtime only.
            if (Application.isPlaying)
            {
                var grid = ServiceLocator.Get<GridManager>();
                if (grid != null)
                {
                    Gizmos.color = _gridColor;
                    for (int x = 0; x < grid.Width; x++)
                    {
                        for (int y = 0; y < grid.Height; y++)
                        {
                            Vector3 center = grid.GetWorldPosition(new Vector2Int(x, y));
                            Gizmos.DrawWireCube(center, new Vector3(1, 0.1f, 1));
                        }
                    }
                }
            }
        }
    }
}
