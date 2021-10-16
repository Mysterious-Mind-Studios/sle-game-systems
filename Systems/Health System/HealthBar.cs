
using UnityEngine;

namespace SLE.Systems.Health
{
    [RequireComponent(typeof(Health))]
    public sealed class HealthBar : SLEComponent<HealthBar>
    {
#if UNDER_DEVELOPMENT
        [SerializeField]
        [Tooltip("The transform object attached to this game object that the generated health bar will be positioned at. \n\n" +
                 "Note: Highly recommended to use an empty game object. The health bars are auto generated at runtime.")]
#endif
        internal Transform attachedPrefab;

#if UNDER_DEVELOPMENT
        [SerializeField]
        [Tooltip("Indicates whether the bar should keep looking at the main camera. \n" +
                 "Unchecking this option makes the bar be statically rotated in the same position.")]
#endif
        internal bool billboard = true;

        /// <summary>
        /// [Internal usage only]
        /// </summary>
        internal int healthComponentID;
    }
}

