

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

        public int   max;
        public int   current;
        public float normalized;
    }
}
