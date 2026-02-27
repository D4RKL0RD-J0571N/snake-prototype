using UnityEngine;

namespace SnakePrototype.Systems.Grid
{
    [CreateAssetMenu(fileName = "GridConfig", menuName = "SnakePrototype/GridConfig")]
    public class GridConfiguration : ScriptableObject
    {
        public int Width = 30;
        public int Height = 30;
        public float CellSize = 1.0f;
        public Material GridMaterial;
    }
}
