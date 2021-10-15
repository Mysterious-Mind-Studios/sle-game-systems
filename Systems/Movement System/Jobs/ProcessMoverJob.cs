
using UnityEngine;
using UnityEngine.Jobs;

using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;


namespace SLE.Systems.Movement.Jobs
{
    using SLE.Systems.Movement.Data;

    [BurstCompile]
    unsafe struct ProcessMoverJob : IJobParallelForTransform
    {
        static readonly Vector3 InvalidDirection = Vector3.positiveInfinity;

        [ReadOnly, NativeDisableUnsafePtrRestriction]
        public MoverData* moverData;

        [ReadOnly]
        public float deltaTime;

        public void Execute(int index, TransformAccess moverTransform)
        {
            MoverData data = moverData[index];

            if (data.direction == InvalidDirection)
                return;

            moverTransform.position += data.direction * data.speed * deltaTime; 
        }
    }
}
