

using UnityEngine;


namespace SLE.Systems.Movement.Data
{
    struct MoverData
    {
        public MoverData(in Mover mover)
        {
            speed     = mover.speed;
            direction = mover.direction;
        }

        public MoverData(in MoverData other)
        {
            speed     = other.speed;
            direction = other.direction;
        }

        public float   speed;
        public Vector3 direction;
    }
}