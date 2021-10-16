
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
#endif
        internal float _maxHealthPoints;

#if UNDER_DEVELOPMENT
        [SerializeField]
#endif        
        internal float _currentHealthPoints;

#if UNDER_DEVELOPMENT
        [SerializeField]
#endif
        internal DeathBehaviour onZeroHealth = DeathBehaviour.DisableGameObject;

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
