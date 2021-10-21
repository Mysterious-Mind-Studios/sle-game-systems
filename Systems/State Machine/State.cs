
using UnityEngine;

namespace SLE.Systems.StateMachine
{
    [RequireComponent(typeof(StateMachine))]
    public abstract class State : MonoBehaviour
    {
        protected StateMachine machine { get; private set; }

        #region Unity Callbacks

        private void OnEnable()
        {
            if (machine)
            {
                if (machine.currentState != this)
                    enabled = false;
            }
        }

        private void OnDisable()
        {
            if (machine)
            {
                if (machine.currentState == this)
                    enabled = true;
            }
        }

        #endregion

        protected virtual void OnStateInitialize(StateMachine machine = null) 
        {
        }
        protected virtual void OnStateEnter() 
        {
        }
        protected virtual void OnStateExit() 
        {
        }

        public virtual void OnStateUpdate() 
        {
        }

        #region Public Methods

        public void Initialize(StateMachine machine)
        {
            this.machine = machine;

            OnStateInitialize(machine);
        }

        public void StateEnter()
        {
            enabled = true;
            OnStateEnter();
        }

        public void StateExit()
        {
            OnStateExit();
            enabled = false;
        }

        #endregion

        /// <returns> The name of the current state. </returns>
        public override string ToString()
        {
            return GetType().Name;
        }

        public static implicit operator bool(State state)
        {
            return state != null;
        }
    }
}