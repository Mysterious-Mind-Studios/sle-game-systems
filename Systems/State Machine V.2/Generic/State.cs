using UnityEngine;

namespace FSMSystemV2.Generic
{
    public abstract class State<T> : ScriptableObject, IState<T>
    {
        /// <summary>
        /// The text to show in the inspector to debug what action is being excecuted in the current state.
        /// <para>E.g: Moving, Attacking, etc...</para>
        /// </summary>
        protected abstract string statusText { get; }

        public abstract void OnEnter(T machine);
        public abstract void OnExit(T machine);
        public abstract void Process(T machine);

        public override string ToString()
        {
            return statusText;
        }

        public static implicit operator bool(State<T> state)
        {
            return state != null;
        }
    }
}