
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
        internal float _maxHealthPoints;

        [SerializeField]
        internal float _currentHealthPoints;

        [SerializeField]
        internal DeathBehaviour onZeroHealth = DeathBehaviour.DisableGameObject;
#else
        internal int _maxHealthPoints;
        internal int _currentHealthPoints;
        internal HealthBehaviour onZeroHealth = HealthBehaviour.DisableGameObject;
#endif

        public float maxHealth
        {
            get => _maxHealthPoints;
            set
            {
                _maxHealthPoints = math.max(0.0f, value);
                OnHealthChange(this);
            }
        }
        public float currentHealth
        {
            get => _currentHealthPoints;
            set
            {
                _currentHealthPoints = math.max(0.0f, value);
                OnHealthChange(this);
            }
        }
    }
}
