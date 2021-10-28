
using UnityEngine;

namespace SLE.Systems.StateMachine
{
    [RequireComponent(typeof(StateMachine))]
    public abstract class State : MonoBehaviour
    {
        protected StateMachine machine { get; private set; }

        protected virtual void OnStateInitialize(StateMachine machine = null) 
        {
        }
        protected virtual void OnStateEnter() 
        {
        }
        protected virtual void OnStateExit() 
        {
        }
        
        public void Initialize(StateMachine machine)
        {
            this.machine = machine;
            OnStateInitialize(machine);
        }
        public void EnterState()
        {
            OnStateEnter();
        }
        public void ExitState()
        {
            OnStateExit();
        }

        public virtual void OnStateUpdate()
        {
        }

        public static implicit operator bool(State state)
        {
            return state != null;
        }
    }
}