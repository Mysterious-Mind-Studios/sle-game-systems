

using System.Runtime.CompilerServices;


namespace SLE.Systems.Health.Utils
{
    public static class Math
    {
        /// <summary>
        /// Min value is always 0.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Normalized(float current, float max)
        {
            return current / max;
        }

        /// <summary>
        /// Min value is always 0.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Normalized(int current, int max)
        {
            return (float)current / (float)max;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Normalized(float current, float min, float max)
        {
            return (current - min) / (max - min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Normalized(int current, int min, int max)
        {
            return (current - min) / (max - min);
        }
    }
}
