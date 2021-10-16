
using System;

using UnityEngine;

using Unity.Mathematics;

namespace SLE.Systems.Movement
{
    using SLE.Events;

    public class Movement : SLEComponent<Movement>
    {
        public float speed;

        [HideInInspector]
        public Vector3 direction;
    }
}
