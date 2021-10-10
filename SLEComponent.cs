
using System;

using UnityEngine;

namespace SLE
{
    using SLE.Events;

    public class SLEComponent<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static OnObjectChange<T> onComponentEnable;
        protected static OnObjectChange<T> onComponentDisable;

        internal static event OnObjectCreate<T>  OnComponentCreate;
        internal static event OnObjectDestroy<T> OnComponentDestroy;
        internal static event OnObjectChange<T>  OnComponentEnable  { add => onComponentEnable += value; remove => onComponentEnable -= value; }
        internal static event OnObjectChange<T>  OnComponentDisable { add => onComponentDisable += value; remove => onComponentDisable -= value; }

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

            onComponentEnable?.Invoke(_this);
        }
        protected void OnDisable()
        {
            T _this = GetComponent<T>();

            onComponentDisable?.Invoke(_this);
        }
        protected void OnDestroy()
        {
            T _this = GetComponent<T>();

            OnComponentDestroy?.Invoke(_this);
        }

        public override int GetHashCode() => base.GetHashCode();
        public override bool Equals(object other) => base.Equals(other);

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
