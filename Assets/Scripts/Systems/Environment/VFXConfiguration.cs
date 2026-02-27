using UnityEngine;
using SnakePrototype.Core;
using SnakePrototype.Systems.Level;

namespace SnakePrototype.Systems.Environment
{
    [CreateAssetMenu(fileName = "VFXConfiguration", menuName = "SnakePrototype/VFXConfiguration")]
    public class VFXConfiguration : ScriptableObject
    {
        [Header("Prefabs")]
        public GameObject EnergyPickupVFX;
        public GameObject EnergyCollectionRipple;
        public GameObject SnakeDeathVFX;
        public GameObject LevelCompleteVFX;
        
        [Header("Settings")]
        public float DefaultDuration = 2.0f;
    }
}
