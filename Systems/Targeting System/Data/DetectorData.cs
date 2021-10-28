
using Unity.Mathematics;

namespace SLE.Systems.Targeting.Data
{
    public struct DetectorData
    {
        public DetectorData(in TargetDetector detector)
        {
            targetIndex       = -1;

            lastDetectionTime = 0;
            fixedTarget       = detector._hasFixedTarget;
            detectionLayer    = detector._targetLayer;
            refreshRate       = detector._detectorInfo.refreshRate;
            detectionRadius   = detector._detectorInfo.fovRadius;
            position          = detector._fovOrigin.position;
            state             = detector.enabled ? DetectorState.Active : DetectorState.Inactive;
        }
        public DetectorData(in DetectorData other)
        {
            targetIndex       = other.targetIndex;

            lastDetectionTime = other.lastDetectionTime;
            fixedTarget       = other.fixedTarget;
            detectionLayer    = other.detectionLayer;
            refreshRate       = other.refreshRate;
            detectionRadius   = other.detectionRadius;
            position          = other.position;
            state             = other.state;
        }

        public bool          fixedTarget;

        public int           targetIndex;
        public int           detectionLayer;
        public float         refreshRate;
        public float         detectionRadius;
        public float         lastDetectionTime;
        public float3        position;
        public DetectorState state;
    }
}
