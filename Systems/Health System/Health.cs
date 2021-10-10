
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
        private int _maxHealthPoints;

        [SerializeField]
        private int _currentHealthPoints;

        [SerializeField]
        private HealthBehaviour _onZeroHealth = HealthBehaviour.DisableGameObject;

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
        public ref readonly HealthBehaviour onZeroHealth { get => ref _onZeroHealth; }
#else
        public int maxHealth                { get; internal set; }
        public int currentHealth            { get; internal set; }
        public HealthBehaviour onZeroHealth { get; internal set; }
#endif
    }
}
