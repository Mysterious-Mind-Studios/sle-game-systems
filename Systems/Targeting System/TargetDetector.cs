

using UnityEditor;
using UnityEngine;


namespace SLE.Systems.Targeting
{
    using SLE.Events;
    using SLE.Systems.Targeting.Data;

    [DisallowMultipleComponent]
    public sealed class TargetDetector : SLEComponent<TargetDetector>
    {
        internal static event OnObjectChange<TargetDetector> OnFixedTargetSet;

#if UNDER_DEVELOPMENT
        [Space]
        [SerializeField]
        internal Targetable target;

        [Space]
        [Tooltip("Used to define a custom origin point for the field of detection view . " +
                 "\n If it's not initialized then the object's position is used instead as a reference.")]
        [SerializeField]
        internal Transform fovOrigin;

        [Space]
        [SerializeField]
        internal TargetDetectorData detectorInfo;
#else
        public Targetable         target        { get; internal set; } 
        public Transform          fovOrigin     { get; internal set; }
        public TargetDetectorData detectorInsfo { get; internal set; }
#endif

        public void EnableDetection()
        {
            enabled = true;
            onComponentEnable(this);
        }
        public void DisableDetection()
        {
            enabled = false;
            onComponentDisable(this);
        }
        public void SetTarget(in Targetable target)
        {
            if (target)
            {
                this.target = target;
                OnFixedTargetSet(this);
            }
        }
        public void ClearTarget()
        {
            target = null;
        }
        public ref readonly Targetable GetCurrentTarget() => ref target;

#if UNITY_EDITOR

        [Space]
        [Header("Editor Settings")]
        [Space]
        [SerializeField] 
        bool drawFOV = true;

        private void OnDrawGizmos()
        {
            if (!drawFOV)      return;
            if (!detectorInfo) return;

            Gizmos.color  = Color.white;
            Handles.color = Color.red;

            var _fovPoint = fovOrigin ? fovOrigin.position : transform.position;

            Gizmos.DrawWireSphere(_fovPoint, detectorInfo.fovRadius);

            if (!target || !target.aimPoint) return;

            Handles.DrawLine(_fovPoint, target.aimPoint.position);
        }

#endif
    }
}