
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using UnityEngine;
using Unity.Mathematics;

using Random = Unity.Mathematics.Random;

namespace SLE
{
    public static class Utils
    {
        static Random random = new Random(1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 SpreadedImpactPoint(in Transform transform, float spread)
        {
            Vector3 origin = transform.position;
            Vector3 forward = transform.forward;
            Vector3 up = transform.up;
            Vector3 right = transform.right;

            Vector3 impactPoint = origin + forward;

            if (spread == 0.0f)
                return impactPoint;

            impactPoint += up * random.NextFloat(-spread, spread);
            impactPoint += right * random.NextFloat(-spread, spread);
            impactPoint -= origin;

            impactPoint.Normalize();

            return impactPoint;
        }

        /// <param name="transform">
        /// c0 - position. <br/>
        /// c1 - forward.  <br/>
        /// c2 - up.       <br/>
        /// c3 - right.    <br/>
        /// </param>
        public static float3 SpreadedImpactPoint(in float3x4 transform, float spread)
        {
            float3 origin = transform.c0;
            float3 forward = transform.c1;
            float3 up = transform.c2;
            float3 right = transform.c3;

            float3 impactPoint = origin + forward;

            if (spread == 0.0f)
                return impactPoint;

            impactPoint += up * random.NextFloat(-spread, spread);
            impactPoint += right * random.NextFloat(-spread, spread);
            impactPoint -= origin;

            math.normalize(impactPoint);

            return impactPoint;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 SpreadedImpactPoint(in Vector3 origin,
                                                  in Vector3 forward,
                                                  in Vector3 up,
                                                  in Vector3 right,
                                                  float spread)
        {
            Vector3 impactPoint = origin + forward;

            if (spread == 0.0f)
                return impactPoint;

            impactPoint += up * random.NextFloat(-spread, spread);
            impactPoint += right * random.NextFloat(-spread, spread);
            impactPoint -= origin;

            impactPoint.Normalize();

            return impactPoint;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 SpreadedImpactPoint(in float3 origin,
                                                 in float3 forward,
                                                 in float3 up,
                                                 in float3 right,
                                                 float spread)
        {
            float3 impactPoint = origin + forward;

            if (spread == 0.0f)
                return impactPoint;

            impactPoint += up * random.NextFloat(-spread, spread);
            impactPoint += right * random.NextFloat(-spread, spread);
            impactPoint -= origin;

            math.normalize(impactPoint);

            return impactPoint;
        }

        /// <summary>
        /// Get <paramref name="value"/>²
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrd(in float value)
        {
            return value * value;
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 RandonDirection(float range)
        {
            return new Vector3(random.NextFloat(-range, range), 0, random.NextFloat(-range, range));
        }
    }

    public static unsafe class NativeCode
    {
        const string dll = "kernel32.dll";

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern ulong _msize(void* memblock);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern void free(void* block);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern void* malloc(ulong size);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern void* calloc(ulong count, ulong size);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern void* memcpy(void* dst, void* src, ulong size);
    }
}