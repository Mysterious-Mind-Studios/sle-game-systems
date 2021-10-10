
using UnityEngine;

namespace SLE.Systems.Targeting.Data
{
    [CreateAssetMenu(menuName = "Targeting System/Target detector data")]
    public class TargetDetectorData : ScriptableObject
    {
        [Space]
        public float fovRadius;

        [Range(0.0f, float.PositiveInfinity)]
        public float refreshRate = 0.1f;

        [Space]
        public LayerMask targetLayers;
    }
}