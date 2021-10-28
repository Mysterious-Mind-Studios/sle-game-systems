
using UnityEngine;

namespace SLE.Systems.StateMachine
{
    [RequireComponent(typeof(StateMachine))]
    public abstract class State : MonoBehaviour
    {
        protected StateMachine machine { get; private set; }

<<<<<<< Updated upstream
        #region Unity Callbacks

        private void OnEnable()
        {
            if (machine)
            {
                if (machine.currentState != this)
                    enabled = false;
            }
=======
        protected virtual void OnStateInitialize(StateMachine machine = null) 
        {
>>>>>>> Stashed changes
        }
        protected virtual void OnStateEnter() 
        {
<<<<<<< Updated upstream
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
=======
>>>>>>> Stashed changes
        }
        protected virtual void OnStateExit() 
        {
        }

<<<<<<< Updated upstream
        public virtual void OnStateUpdate() 
        {
        }

        #region Public Methods

=======
>>>>>>> Stashed changes
        public void Initialize(StateMachine machine)
        {
            this.machine = machine;

            OnStateInitialize(machine);
        }

        public void EnterState()
        {
            OnStateEnter();
        }

<<<<<<< Updated upstream
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
=======
        public void ExitState()
        {
            OnStateExit();
        }

        public virtual void UpdateState()
        {
>>>>>>> Stashed changes
        }

        public static implicit operator bool(State state)
        {
<<<<<<< Updated upstream
=======
            return GetType().Name;
        }

        public static implicit operator bool(State state)
        {
>>>>>>> Stashed changes
            return state != null;
        }
    }
}