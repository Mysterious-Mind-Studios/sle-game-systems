
using System;

using UnityEngine;

using Unity.Mathematics;

namespace SLE.Systems.Health
{
    public sealed class Health : SLEComponent<Health>
    {
        internal static event Action<Health> OnHealthChange;

#if UNDER_DEVELOPMENT
        [SerializeField]
        internal int _maxHealthPoints;

        [SerializeField]
        internal int _currentHealthPoints;

        [SerializeField]
        internal HealthBehaviour onZeroHealth = HealthBehaviour.DisableGameObject;
#else
        internal int _maxHealthPoints;
        internal int _currentHealthPoints;
        internal HealthBehaviour onZeroHealth = HealthBehaviour.DisableGameObject;
#endif

        public int maxHealth
        {
            get => _maxHealthPoints;
            set
            {
                _maxHealthPoints = math.max(0, value);
                OnHealthChange(this);
            }
        }
        public int currentHealth
        {
            get => _currentHealthPoints;
            set
            {
                _currentHealthPoints = math.max(0, value);
                OnHealthChange(this);
            }
        }
    }
}
