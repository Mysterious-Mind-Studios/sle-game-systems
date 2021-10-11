
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

            barFillTransform.localScale = new Vector3(healthData.normalized, barFillTransform.localScale.y, barFillTransform.localScale.z);
        }
    }
}
