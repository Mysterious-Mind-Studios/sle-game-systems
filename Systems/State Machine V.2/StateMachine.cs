using System;
using UnityEngine;

namespace FSMSystemV2
{
    [DisallowMultipleComponent]
    public abstract class StateMachine : MonoBehaviour, IStateMachine
    {
#if UNITY_EDITOR
        [Tooltip("Visual debugging for the current machine behaviour. (Read Only)")]
        [SerializeField]
        protected string status = string.Empty;
#endif

        [SerializeField] 
        private State _defaultState;
        
        [Space]
        [SerializeField] 
        protected StateRefreshQuality stateRefreshQuality;
        
        [Space]
        [SerializeField]
        [Tooltip("All the states the machine can alter to.")]
        protected State[] _machineStates;

        [NonSerialized] 
        private float refreshRate;
        [NonSerialized] 
        private float timer;

        public IState defaultState 
        { 
            get => _defaultState; 
        }
        public IState currentState 
        {
            get; 
            protected set;
        }

        protected void Awake() 
        {
            currentState = defaultState;

            switch (stateRefreshQuality)
            {
                case StateRefreshQuality.performance:
                    refreshRate = 1f;
                    break;

                case StateRefreshQuality.balanced:
                    refreshRate = 0.5f;
                    break;

                case StateRefreshQuality.precision:
                    refreshRate = 0f;
                    break;
            }
        }

        protected void LateUpdate() 
        {
            timer += Time.deltaTime;

            if (timer < refreshRate) return;

            currentState?.Process(this);

            timer = 0f;
        }

        protected void OnDestroy()
        {
            currentState = null;
        }

        public abstract bool SetState(IState state);
        public abstract void SetState<State>() where State : IState;
    }
}