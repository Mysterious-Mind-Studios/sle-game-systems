﻿
using UnityEngine.Jobs;

using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace SLE.Systems.Health.Jobs
{
    using SLE.Systems.Health.Data;

    [BurstCompile]
    unsafe struct UpdateHealthBarTransformJob : IJobParallelForTransform
    {
        [ReadOnly, NativeDisableUnsafePtrRestriction]
        internal HealthBarData* healthBarDataPtr;

        [ReadOnly]
        public float3 mainCameraForward;

        public void Execute(int index, TransformAccess barTransform)
        {
            HealthBarData healthBarData = healthBarDataPtr[index];

            if (healthBarData.updateRotation)
                barTransform.rotation = quaternion.LookRotation(-mainCameraForward, math.up());
        }
    }
}

