
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace SLE.Systems.Experimental.Camera.Data
{
    struct ControllerData
    {
        public ControllerData(in CameraController controller)
        {
            sensivity            = controller._sensivity;
            scrollSensibility    = controller._scrollSensivity;
            maxDistanceThreshold = controller._maxDistanceThreshold;
            minZoom              = controller._minZoom;
            maxZoom              = controller._maxZoom;
        }

        public ControllerData(in ControllerData other)
        {
            sensivity            = other.sensivity;
            scrollSensibility    = other.scrollSensibility;
            maxDistanceThreshold = other.maxDistanceThreshold;
            minZoom              = other.minZoom;
            maxZoom              = other.maxZoom;
        }

        public float sensivity;
        public float scrollSensibility;
        public float maxDistanceThreshold;
        public float minZoom;
        public float maxZoom;
    }
}
