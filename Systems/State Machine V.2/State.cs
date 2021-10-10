using UnityEngine;

namespace FSMSystemV2
{
    public abstract class State : ScriptableObject, IState
    {
        /// <summary>
        /// The text to show in the inspector to debug what action is being excecuted in the current state.
        /// <para>E.g: Moving, Attacking, etc...</para>
        /// </summary>
        protected abstract string statusText { get; }

        public abstract void OnEnter(IStateMachine machine);
        public abstract void Process(IStateMachine machine);
        public abstract void OnExit(IStateMachine machine);

        public override string ToString()
        {
            return statusText;
        }

        public static implicit operator bool(State state)
        {
            return state != null;
        }
    }
}