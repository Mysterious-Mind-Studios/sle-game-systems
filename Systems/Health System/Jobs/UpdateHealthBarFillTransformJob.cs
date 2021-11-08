
using UnityEngine;
using UnityEngine.Jobs;

using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;


namespace SLE.Systems.Health.Jobs
{
    using SLE.Systems.Health.Data;

    [BurstCompile]
    unsafe struct UpdateHealthBarFillTransformJob : IJobParallelForTransform
    {
        [ReadOnly]
        [NativeDisableUnsafePtrRestriction]
        internal HealthData* healthDataPtr;

        public void Execute(int index, TransformAccess barFillTransform)
        {
            barFillTransform.localScale = new Vector3(healthDataPtr[index].normalized, barFillTransform.localScale.y, barFillTransform.localScale.z);
        }
    }
}
