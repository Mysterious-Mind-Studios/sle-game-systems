
using UnityEngine;
using UnityEngine.Jobs;

using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace SLE.Systems.Experimental.Camera.Jobs
{
    using SLE.Systems.Experimental.Camera.Data;

    [BurstCompile]
    struct UpdateCameraControllerTransformJob : IJobParallelForTransform
    {
        [ReadOnly]
        [NativeDisableUnsafePtrRestriction]
        internal unsafe ControllerData* controllerDataPtr;

        [ReadOnly]
        public float   deltaTime;
        [ReadOnly]
        public Vector3 axisInput;

        public void Execute(int index, TransformAccess transform)
        {
            if (!transform.isValid)
                return;

            unsafe
            {
                Vector3 position = transform.position;
                float3 posNorm  = math.normalize(position);

                Vector3 movementDirection = axisInput;

                float maxThreshold = controllerDataPtr[index].maxDistanceThreshold;

                float absX = math.abs(position.x);
                float absZ = math.abs(position.z);

                float roundedX = math.round(posNorm.x);
                float roundedZ = math.round(posNorm.z);

                if (roundedX == movementDirection.x)
                {
                    if (absX >= maxThreshold)
                    { 
                        movementDirection.x = 0;

                        position.x = math.round(position.x);
                    }
                }

                if (roundedZ == movementDirection.z)
                {
                    if (absZ >= maxThreshold)
                    {
                        movementDirection.z = 0;

                        position.z = math.round(position.z);
                    }
                }

                transform.position = position + movementDirection * controllerDataPtr[index].sensivity * deltaTime;
            }
        }
    }
}
