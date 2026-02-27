using UnityEngine;
using SnakePrototype.Core;

namespace SnakePrototype.Systems.Grid
{
    public class GridManager : IGameService
    {
        private GridConfiguration _config;
        private bool[,] _walls;
        private int _currentWidth;
        private int _currentHeight;

        public GridConfiguration Config => _config;
        public int Width => _currentWidth;
        public int Height => _currentHeight;
        public float CellSize => _config != null ? _config.CellSize : 1f;
        public GridManager(GridConfiguration config)
        {
            _config = config;
            if (_config == null)
            {
                Debug.LogError("GridManager: Missing Configuration!");
                _currentWidth = 28;
                _currentHeight = 20;
            }
            else
            {
                _currentWidth = _config.Width;
                _currentHeight = _config.Height;
            }
            Resize(_currentWidth, _currentHeight);
        }

        public void Initialize()
        {
            Debug.Log($"GridManager Initialized: {Width}x{Height}");
        }

        public void Resize(int width, int height)
        {
            _currentWidth = width;
            _currentHeight = height;
            _walls = new bool[width, height];
        }

        public void SetWall(int x, int y, bool isWall)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                _walls[x, y] = isWall;
            }
        }

        public bool IsWall(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                return _walls[x, y];
            }
            return true; // Outside bounds is treated as wall for collision
        }

        public Vector3 GetWorldPosition(Vector2Int gridPos)
        {
            float size = _config != null ? _config.CellSize : 1f;
            return new Vector3(gridPos.x * size, 0.0f, gridPos.y * size);
        }

        public bool IsBounds(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < Width && pos.y >= 0 && pos.y < Height;
        }

        public Vector2Int GetRandomPosition()
        {
            return new Vector2Int(Random.Range(0, Width), Random.Range(0, Height));
        }

        public System.Collections.Generic.List<Vector2Int> GetRandomSpawnPoints(int amount)
        {
            var result = new System.Collections.Generic.List<Vector2Int>();
            var freeCells = new System.Collections.Generic.List<Vector2Int>();
            
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (!IsWall(x, y))
                    {
                        freeCells.Add(new Vector2Int(x, y));
                    }
                }
            }

            for (int i = 0; i < amount && freeCells.Count > 0; i++)
            {
                int idx = Random.Range(0, freeCells.Count);
                result.Add(freeCells[idx]);
                freeCells.RemoveAt(idx);
            }
            return result;
        }

        public void Shutdown() { }
    }
}
