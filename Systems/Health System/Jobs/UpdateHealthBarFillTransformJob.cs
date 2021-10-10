
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
        [ReadOnly, NativeDisableUnsafePtrRestriction]
        public HealthData* healthDataPtr;

        public void Execute(int index, TransformAccess barFillTransform)
        {
            ref HealthData healthData = ref healthDataPtr[index];

            Vector3 barFillTransformScale = barFillTransform.localScale;

            barFillTransformScale.x = healthData.normalized;

            barFillTransform.localScale = barFillTransformScale;
        }
    }
}
