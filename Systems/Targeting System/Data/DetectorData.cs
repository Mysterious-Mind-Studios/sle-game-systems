
using Unity.Mathematics;

namespace SLE.Systems.Targeting.Data
{
    public struct DetectorData
    {
        public DetectorData(in TargetDetector detector)
        {
            targetIndex       = -1;
            targetLayer       = detector._targetLayer;
            lastDetectionTime = 0;
            refreshRate       = detector._detectorInfo.refreshRate;
            detectionRadius   = detector._detectorInfo.fovRadius;
            position          = detector._fovOrigin.position;
            state             = detector.enabled ? DetectorState.Active : DetectorState.Inactive;
        }
        public DetectorData(in DetectorData other)
        {
            targetIndex       = other.targetIndex;
            targetLayer       = other.targetLayer;
            refreshRate       = other.refreshRate;
            detectionRadius   = other.detectionRadius;
            lastDetectionTime = other.lastDetectionTime;
            position          = other.position;
            state             = other.state;
        }

        public int           targetIndex;
        public int           targetLayer;
        public float         refreshRate;
        public float         detectionRadius;
        public float         lastDetectionTime;
        public float3        position;
        public DetectorState state;
    }
}
