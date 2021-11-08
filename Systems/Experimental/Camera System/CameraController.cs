
using UnityEngine;
using Cinemachine;

namespace SLE.Systems.Experimental.Camera
{
    using SLE.Events;

    public class CameraController : SLEComponent<CameraController>
    {
        internal static event OnObjectChange<CameraController> onControllerDataChange;

#if UNITY_EDITOR
        [SerializeField] 
#endif
        internal CinemachineVirtualCamera _virtualCamera;

#if UNITY_EDITOR
        [Space]
        [SerializeField]
#endif
        internal float _sensivity;
        
#if UNITY_EDITOR
        [SerializeField]
#endif
        internal float _scrollSensivity;

#if UNITY_EDITOR
        [SerializeField]
        [Range(0, float.PositiveInfinity)]
#endif
        internal float _minZoom = 0;

#if UNITY_EDITOR
        [SerializeField]
        [Range(1, 1000)]
#endif
        internal float _maxZoom = 1;

#if UNITY_EDITOR
        [SerializeField]
        [Tooltip("This is the max distance the object can move in any direction based on world's origin (0,0,0).")]
#endif
        internal float _maxDistanceThreshold;

        [HideInInspector]
        internal CinemachineFramingTransposer _transposer;

        public float sensivity
        {
            get => _sensivity;
            set
            {
                _sensivity = value;
                onControllerDataChange(this);
            }
        }

        public float scrollSensivity
        {
            get => _scrollSensivity;
            set
            {
                _scrollSensivity = value;
                onControllerDataChange(this);
            }
        }

        public float minZoom
        {
            get => _minZoom;
            set
            {
                _minZoom = value;
                onControllerDataChange(this);
            }
        }

        public float maxZoom
        {
            get => _maxZoom;
            set
            {
                _maxZoom = value;
                onControllerDataChange(this);
            }
        }

        public float maxDistanceThreshold
        {
            get => _maxDistanceThreshold;
            set
            {
                _maxDistanceThreshold = value;
                onControllerDataChange(this);
            }
        }
    }
}
