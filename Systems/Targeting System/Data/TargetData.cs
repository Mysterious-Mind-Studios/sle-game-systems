

using Unity.Mathematics;

namespace SLE.Systems.Targeting.Data
{
    public struct TargetData
    {
        public TargetData(in Targetable targetable)
        {
            layer    = targetable.gameObject.layer;
            position = targetable.aimPoint.position;
            rotation = targetable.aimPoint.rotation;
            state    = targetable.enabled ? TargetState.Valid : TargetState.Invalid;
        }
        public TargetData(in TargetData other)
        {
            layer    = other.layer;
            position = other.position;
            rotation = other.rotation;
            state    = other.state;
        }

        public int         layer;
        public float3      position;
        public quaternion  rotation;
        public TargetState state;
    }
}
