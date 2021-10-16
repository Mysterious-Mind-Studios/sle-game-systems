
using UnityEngine;

#if UNDER_DEVELOPMENT
#if UNITY_EDITOR
using UnityEditor;
#endif
#endif

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
#endif
        internal Targetable _target;

#if UNDER_DEVELOPMENT
        [Space]
        [Tooltip("Used to define a custom origin point for the field of detection view . " +
                 "\n If it's not initialized then the object's position is used instead as a reference.")]
        [SerializeField]
#endif
        internal Transform _fovOrigin;

#if UNDER_DEVELOPMENT
        [Space]
        [SerializeField]
#endif
        internal TargetDetectorData _detectorInfo;

#if UNDER_DEVELOPMENT
        [Space]
        [SerializeField]
#endif
        internal LayerMask _targetLayer;
        public Targetable target
        {
            get => _target;
            set
            {
                if (value)
                {
                    _target = value;
                    OnFixedTargetSet(this);
                }
            }
        }

#if UNDER_DEVELOPMENT
#if UNITY_EDITOR

        [Space]
        [Header("Editor Settings")]
        [Space]
        [SerializeField] 
        bool drawFOV = true;

        private void OnDrawGizmos()
        {
            if (!drawFOV)      return;
            if (!_detectorInfo) return;

            Gizmos.color  = Color.white;
            Handles.color = Color.red;

            var _fovPoint = _fovOrigin ? _fovOrigin.position : transform.position;

            Gizmos.DrawWireSphere(_fovPoint, _detectorInfo.fovRadius);

            if (!_target || !_target.aimPoint) return;

            Handles.DrawLine(_fovPoint, _target.aimPoint.position);
        }

#endif
#endif
    }
}