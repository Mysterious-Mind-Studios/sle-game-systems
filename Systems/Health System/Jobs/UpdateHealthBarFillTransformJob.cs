
using UnityEngine;
using UnityEngine.Jobs;

using Unity.Burst;
using Unity.Collections;


namespace SLE.Systems.Health.Jobs
{
    using SLE.Systems.Health.Data;

    [BurstCompile]
    unsafe struct UpdateHealthBarFillTransformJob : IJobParallelForTransform
    {
        [ReadOnly]
        [DeallocateOnJobCompletion]
        public NativeArray<HealthData> healthDataArray;

        public void Execute(int index, TransformAccess barFillTransform)
        {
            barFillTransform.localScale = new Vector3(healthDataArray[index].normalized, barFillTransform.localScale.y, barFillTransform.localScale.z);
        }
    }
}
