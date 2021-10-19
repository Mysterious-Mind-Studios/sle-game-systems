
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

        internal int _id = -1;
        public int id { get => _id; }

        protected void Awake()
        {
            T _this = GetComponent<T>();

            OnComponentCreate?.Invoke(_this);
        }
        protected void OnEnable()
        {
            T _this = GetComponent<T>();

            OnComponentEnable?.Invoke(_this);
        }
        protected void OnDisable()
        {
            T _this = GetComponent<T>();

            OnComponentDisable?.Invoke(_this);
        }
        protected void OnDestroy()
        {
            T _this = GetComponent<T>();

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
            int lhsHash = lhs.GetHashCode();
            int rhsHash = rhs.GetHashCode();
            return lhsHash == rhsHash;
        }
        public static bool operator !=(SLEComponent<T> lhs, SLEComponent<T> rhs)
        {
            return !(lhs == rhs);
        }
    }
}
