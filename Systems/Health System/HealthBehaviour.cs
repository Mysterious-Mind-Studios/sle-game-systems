namespace SLE.Systems.Health
{
    public enum HealthBehaviour
    {
        /// <summary>
        /// Disables the component itself and the health bar associated with it.
        /// </summary>
        Disable,
        /// <summary>
        /// Set the entire game object associated with this component
        /// and the health bar associated with it inactive.
        /// </summary>
        DisableGameObject,
        /// <summary>
        /// Destroys permanently the component itself and the health bar associated with it.
        /// </summary>
        Destroy,
        /// <summary>
        /// [Not recommended] <br/>
        /// Destroys permanently the entire game object associated with this component and
        /// the health bar associated with it.
        /// </summary>
        DestroyGameObject
    }
}
