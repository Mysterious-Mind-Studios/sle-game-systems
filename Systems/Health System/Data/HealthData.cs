

namespace SLE.Systems.Health.Data
{
    public struct HealthData
    {
        public HealthData(in Health health)
        {
            max        = health.maxHealth;
            current    = health.currentHealth;
            normalized = Utils.Math.Normalized(current, max);
        }

        public HealthData(in HealthData other)
        {
            max        = other.max;
            current    = other.current;
            normalized = Utils.Math.Normalized(current, max);
        }

        public float max;
        public float current;
        public float normalized;
    }
}
