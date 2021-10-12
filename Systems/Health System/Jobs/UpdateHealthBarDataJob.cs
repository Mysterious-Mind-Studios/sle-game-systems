
using UnityEngine.Jobs;

using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;

namespace SLE.Systems.Health.Jobs
{
    using SLE.Systems.Health.Data;

    [BurstCompile]
    unsafe struct UpdateHealthBarDataJob : IJobParallelForTransform
    {
        [NativeDisableUnsafePtrRestriction]
        public HealthBarData* healthBarDataPtr;

        public void Execute(int index, TransformAccess barAnchorTransform)
        {
            if (!barAnchorTransform.isValid) return;

            HealthBarData healthBarData = healthBarDataPtr[index];

            healthBarData.targetPosition = barAnchorTransform.position;
        }
    }
}
