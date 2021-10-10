

using UnityEngine;


namespace SLE.Systems.Health.Data
{
    public struct HealthBarData
    {
        public HealthBarData(in HealthBar healthBar)
        {
            targetPosition = healthBar.barAnchor.position;
            updatePosition = !healthBar.gameObject.isStatic;
            updateRotation = healthBar.billboard;
        }

        public HealthBarData(in HealthBarData other)
        {
            targetPosition = other.targetPosition;
            updatePosition = other.updatePosition;
            updateRotation = other.updateRotation;
        }

        public Vector3 targetPosition;
        /// <summary>
        /// Is the game object static?
        /// </summary>
        public bool updatePosition;
        public bool updateRotation;
    }
}
