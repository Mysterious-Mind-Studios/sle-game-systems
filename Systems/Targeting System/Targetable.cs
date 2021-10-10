
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
        internal Transform aimPoint;
#else
        public Transform aimPoint { get; internal set; }
#endif
    }
}