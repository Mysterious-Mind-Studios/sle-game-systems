
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
        internal Transform barAnchor;

        [SerializeField]
        [Tooltip("A multiplier for the bar size. The actual bar size will be multiplied by these values in each axis. \n" +
                 "A value of one in each axis will make the bar keep it's original size.")]
        internal Vector2 barScale = Vector2.one;

        [SerializeField]
        [Tooltip("Indicates whether the bar should keep looking at the main camera. \n" +
                 "Unchecking this option makes the bar be statically rotated in the same position.")]
        internal bool billboard = true;
#else
        internal Transform barAnchor;
        internal Vector2   barScale = Vector2.one;
        internal bool      billboard = true;
#endif

        /// <summary>
        /// [Internal usage only]
        /// </summary>
        internal int healthComponentID;
        /// <summary>
        /// [Internal usage only]
        /// </summary>
        internal GameObject generatedHealthBar;
    }
}

