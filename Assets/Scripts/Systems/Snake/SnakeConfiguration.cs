using UnityEngine;

namespace SnakePrototype.Systems.Snake
{
    [CreateAssetMenu(fileName = "SnakeConfig", menuName = "SnakePrototype/SnakeConfig")]
    public class SnakeConfiguration : ScriptableObject
    {
        public float MoveInterval = 0.2f;
        public int StartLength = 3;
        public GameObject SegmentPrefab;
        
        [Header("Visuals")]
        public Material HeadMaterial;
        public Material BodyMaterial;
    }
}
