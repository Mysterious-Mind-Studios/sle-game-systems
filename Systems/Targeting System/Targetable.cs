
using UnityEngine;

namespace SLE.Systems.Targeting
{
    using SLE.Events;
    using SLE.Systems.Targeting.Data;

    [DisallowMultipleComponent]
    public sealed class Targetable : SLEComponent<Targetable>
    {
#if UNDER_DEVELOPMENT
        [SerializeField]
#endif
        internal Transform aimPoint;

        public Vector3    position => aimPoint.position;
        public Quaternion rotaton  => aimPoint.rotation;
    }
}