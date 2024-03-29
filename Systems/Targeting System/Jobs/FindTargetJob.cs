﻿
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;

namespace SLE.Systems.Targeting.Jobs
{
    using SLE.Systems.Targeting.Data;

    [BurstCompile]
    unsafe struct FindTargetJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction]
        internal DetectorData* detectorData;

        [ReadOnly, NativeDisableUnsafePtrRestriction]
        internal TargetData* targetData;

        [ReadOnly]
        public int targetDataLength;

        [ReadOnly]
        public float time;

        public void Execute(int index)
        {
            ref DetectorData detector = ref detectorData[index];

            float sqrdRadius;

            float3 detectorPos;
            float3 closestTargetPos = float3.zero;

            switch (detector.state)
            {
                case DetectorState.Active:
                    {
                        float elapsedTime = time - detector.lastDetectionTime;

                        if (elapsedTime > detector.refreshRate)
                        {
                            detector.lastDetectionTime = time;
                            detector.targetIndex       = -1;

                            detectorPos = detector.position;

                            sqrdRadius = Utils.Sqrd(detector.detectionRadius);

                            for (int i = 0; i < targetDataLength; i++)
                            {
                                ref readonly TargetData target = ref targetData[i];

                                switch (target.state)
                                {
                                    case TargetState.Valid:
                                        {
                                            if (((1 << target.layer) & detector.detectionLayer) == 0)
                                                continue;

                                            float sqrdDistanceToTarget = math.distancesq(detectorPos, target.position);

                                            if (sqrdDistanceToTarget == 1 ||
                                                sqrdDistanceToTarget > sqrdRadius)
                                                continue;

                                            if (detector.targetIndex == -1)
                                            {
                                                detector.targetIndex = i;
                                                closestTargetPos = target.position;
                                                continue;
                                            }

                                            float sqrdDistanceToClosest = math.distancesq(detectorPos, closestTargetPos);

                                            if (sqrdDistanceToTarget < sqrdDistanceToClosest)
                                            {
                                                detector.targetIndex = i;
                                                closestTargetPos = target.position;
                                            }
                                        }
                                        continue;
                                }
                            }
                        }
                    }
                    break;

                default:
                    return;
            }
        }
    }
}