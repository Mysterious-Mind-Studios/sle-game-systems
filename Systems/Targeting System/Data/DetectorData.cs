
using Unity.Mathematics;

namespace SLE.Systems.Targeting.Data
{
    public struct DetectorData
    {
        public DetectorData(in TargetDetector detector)
        {
            targetIndex       = -1;
            lastDetectionTime = 0;
            refreshRate       = detector.detectorInfo.refreshRate;
            detectionRadius   = detector.detectorInfo.fovRadius;
            position          = detector.fovOrigin.position;
            state             = detector.enabled ? DetectorState.Active : DetectorState.Inactive;
        }
        public DetectorData(in DetectorData other)
        {
            targetIndex       = other.targetIndex;
            refreshRate       = other.refreshRate;
            detectionRadius   = other.detectionRadius;
            lastDetectionTime = other.lastDetectionTime;
            position          = other.position;
            state             = other.state;
        }

        public int           targetIndex;
        public float         refreshRate;
        public float         detectionRadius;
        public float         lastDetectionTime;
        public float3        position;
        public DetectorState state;
    }
}
