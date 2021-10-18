
using UnityEngine;

namespace SLE.Systems.ObjectPooling
{
    public class PoolObject : MonoBehaviour
    { 
        internal Pool owner;

        public void ReturnToPool()
        {
            owner?.ReturnItem(this);
        }
    }
}
