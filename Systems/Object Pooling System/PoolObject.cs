
using UnityEngine;

namespace SLE.Systems.ObjectPooling
{
    using SLE.Systems.Health;

    public class PoolObject : MonoBehaviour
    {
        internal Pool owner;

        internal void SetPool(Pool pool)
        {
            owner = pool;
        }
        public virtual void OnObjectUse() { }

        private void OnDisable()
        {
            if (!owner) return;

            owner.ReturnItem(this);
        }

        private void Update()
        {
            transform.Translate(Vector3.forward * 15 * Time.deltaTime);   
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out Health health))
                health.currentHealth -= 10;

            owner?.ReturnItem(this);
        }
    }
}
