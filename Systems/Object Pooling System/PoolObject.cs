
using UnityEngine;

namespace SLE.Systems.ObjectPooling
{
    public abstract class PoolObject : MonoBehaviour
    { 
        internal Pool owner;

        internal void SetPool(Pool pool)
        {
            owner = pool;
        }
        public abstract void OnObjectUse();
        public void ReturnToPool()
        {
            owner?.ReturnItem(this);
        }
    }
}
