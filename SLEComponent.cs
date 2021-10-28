
using System;

using UnityEngine;

namespace SLE
{
    using SLE.Events;

    public class SLEComponent<T> : MonoBehaviour where T : MonoBehaviour
    {
        internal static event OnObjectCreate<T>  OnComponentCreate;
        internal static event OnObjectDestroy<T> OnComponentDestroy;
        internal static event OnObjectChange<T>  OnComponentEnable;
        internal static event OnObjectChange<T>  OnComponentDisable;
        
        private T _this;

        internal int _id = -1;
        public int id { get => _id; }

        protected void Awake()
        {
            _this = GetComponent<T>();

            OnComponentCreate?.Invoke(_this);
        }
        protected void OnEnable()
        {
            OnComponentEnable?.Invoke(_this);
        }
        protected void OnDisable()
        {
            OnComponentDisable?.Invoke(_this);
        }
        protected void OnDestroy()
        {
            OnComponentDestroy?.Invoke(_this);
        }

        public override int GetHashCode() => base.GetHashCode();
        public override bool Equals(object other)
        {
            return other is SLEComponent<T> sleComponent && 
                   sleComponent == this;
        }

        public static bool operator ==(SLEComponent<T> lhs, SLEComponent<T> rhs)
        {
            int lhsHash = lhs?.GetHashCode() ?? -1;
            int rhsHash = rhs?.GetHashCode() ?? -1;
            return lhsHash == rhsHash;
        }
        public static bool operator !=(SLEComponent<T> lhs, SLEComponent<T> rhs)
        {
            return !(lhs == rhs);
        }
    }
}
