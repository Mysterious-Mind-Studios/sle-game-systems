using System;
using UnityEngine; 

namespace FSMSystemV2.Generic
{
    [DisallowMultipleComponent]
    public abstract class StateMachine<T> : MonoBehaviour, IStateMachine<T>
    {
        [SerializeField]
        protected T _machine;

        [Tooltip("Visual debugging for the current machine behaviour. (Read Only)")]
        [SerializeField]
        protected string status = string.Empty;

        [SerializeField]
        private State<T> _defaultState;

        [Space]
        [SerializeField]
        protected StateRefreshQuality stateRefreshQuality;

        [Space]
        [SerializeField]
        [Tooltip("All the states the machine can alter to.")]
        protected State<T>[] _machineStates;

        [NonSerialized]
        private float refreshRate;
        [NonSerialized]
        private float timer;

        public IState<T> defaultState
        {
            get => _defaultState;
        }
        public IState<T> currentState
        {
            get;
            protected set;
        }

        protected void Awake()
        {
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

        protected void OnValidate()
        {
            if (_machine == null)
            {
                if (!TryGetComponent(out T machine)) return;
                
                _machine = machine;
            }
        }

        protected void Update()
        {
            timer += Time.deltaTime;

            if (timer < refreshRate) return;

            currentState?.Process(_machine);

            timer = 0f;
        }

        public abstract bool SetState(IState<T> state);

        public abstract void SetState<State>() where State : IState<T>;
    }
}
