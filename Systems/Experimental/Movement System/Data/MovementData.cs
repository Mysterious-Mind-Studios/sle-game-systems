using UnityEngine;


namespace SLE.Systems.Experimental.Movement.Data
{
    struct MovementData
    {
        public MovementData(in Movement movement)
        {
            speed = movement.speed;
            direction = movement.transform.TransformDirection(Vector3.forward);
        }
        public MovementData(in MovementData other)
        {
            speed = other.speed;
            direction = other.direction;
        }

        public float speed;
        public Vector3 direction;
    }
}