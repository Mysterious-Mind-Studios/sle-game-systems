
using UnityEngine;

namespace SLE.Systems.Experimental.Movement
{
    using SLE;

    public class Movement : SLEComponent<Movement>
    {
        public float speed;

        [HideInInspector]
        public Vector3 direction;
    }
}
