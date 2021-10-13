
using System.Collections.Generic;

using UnityEngine;

namespace SLE.Systems.ObjectPooling
{
    public class Pool : MonoBehaviour
    {
#if UNDER_DEVELOPMENT
        [SerializeField]
        internal PoolObject poolObject;

        [SerializeField]
        internal int poolSize;
#else
        internal GameObject prefab;
        internal int poolSize;
#endif

        Queue<PoolObject> poolQueue;

        private void Awake()
        {
            poolQueue = new Queue<PoolObject>(poolSize);

            for (int i = 0; i < poolSize; i++)
            {
                PoolObject poolObj = Instantiate(poolObject, transform);

                poolObj.SetPool(this);
                poolObj.gameObject.SetActive(false);

                poolQueue.Enqueue(poolObj);
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < poolQueue.Count; i++)
            {
                PoolObject po = poolQueue.Dequeue();

                po.SetPool(null);
            }
        }

        public PoolObject GetObject()
        {
            PoolObject item = poolQueue.Dequeue();
            poolQueue.Enqueue(item);

            item.transform.parent = null;
            item.gameObject.SetActive(true);

            item.OnObjectUse();

            return item;
        }

        internal void ReturnItem(PoolObject item)
        {
            if (item.owner.Equals(this))
            {
                item.transform.parent = transform;
                item.gameObject.SetActive(false);
            }
        }
    }
}
