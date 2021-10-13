

using UnityEngine;


namespace SLE.Systems.Health
{
    public enum HealthBehaviour
    {
#if UNDER_DEVELOPMENT
        [Tooltip("Disables the component itself and the health bar associated with it.")]
#else
        /// <summary>
        /// Disables the component itself and the health bar associated with it.
        /// </summary>
#endif
        Disable,
#if UNDER_DEVELOPMENT
        [Tooltip("Set the entire game object associated with this component and the health bar associated with it inactive.")]
#else
        /// <summary>
        /// Set the entire game object associated with this component
        /// and the health bar associated with it inactive.
        /// </summary>
#endif
        DisableGameObject,
#if UNDER_DEVELOPMENT
        [Tooltip("Destroys permanently the component itself and the health bar associated with it.")]
#else
        /// <summary>
        /// Destroys permanently the component itself and the health bar associated with it.
        /// </summary>
#endif
        Destroy,
#if UNDER_DEVELOPMENT
        [Tooltip("[Not recommended]\nDestroys permanently the entire game object associated with this component and the health bar associated with it.")]
#else
        /// <summary>
        /// [Not recommended] <br/>
        /// Destroys permanently the entire game object associated with this component and
        /// the health bar associated with it.
        /// </summary>
#endif
        DestroyGameObject
    }
}
