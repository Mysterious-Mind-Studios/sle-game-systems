using UnityEngine;

namespace StateMachineSystem
{
    [RequireComponent(typeof(StateMachine))]
    public abstract class State : MonoBehaviour
    {
        protected const string nextStateTooltip = "Change the state of the current machine to this State when evaluated to true";
        protected const string defaultStateTooltip = "Change the state of the current machine to this state when evaluated to false.";

        [System.NonSerialized]
        private float refreshRate = -1f;
        [System.NonSerialized]
        protected float timer = 0f;

        protected StateMachine machine { get; private set; }

        #region Unity Callbacks
        
        private void OnEnable()
        {
            if (this != machine.currentState)
                enabled = false;
        }

        private void OnDisable()
        {
            if (this == machine.currentState)
                enabled = true;
        }

        protected void OnValidate()
        {
            if (machine == null)
                machine = GetComponent<StateMachine>();
        }

        private void Update()
        {
            OnStateUpdate();
            
            if (refreshRate <= 0)
            {
                OnStateRefresh();
                return;
            }

            timer += Time.deltaTime;

            if (timer < refreshRate) return;

            OnStateRefresh();
            timer = 0f;
        }

        #endregion

        protected virtual void OnStateInitialize(StateMachine machine = null) {}

        protected virtual void OnStateEnter() {}

        protected virtual void OnStateExit() {}

        /// <summary>
        /// On State Refresh is called every State refresh status.
        /// <para>Depending on it's machine refresh status settings, it can be faster or slower.</para>
        /// </summary>
        protected virtual void OnStateRefresh() {}

        /// <summary>
        /// On State Update is called every frame by the State's internal Update.
        /// </summary>
        protected virtual void OnStateUpdate() {}

        #region Public Methods

        public void Initialize(StateMachine machine)
        {
            this.machine = machine;

            OnStateInitialize(this.machine);

            switch (machine.stateUpdateQuality)
            {
                case StateRefreshRateQuality.performance:
                    refreshRate = 1f;
                    return;

                case StateRefreshRateQuality.balanced:
                    refreshRate = 0.5f;
                    return;

                default:
                case StateRefreshRateQuality.precision:
                    refreshRate = 0f;
                    return;
            }
        }

        public void StateEnter()
        {
            enabled = true;
            OnStateEnter();
        }
        
        public void StateExit()
        {
            OnStateExit();
            timer = 0f;
            enabled = false;
        }

        #endregion

        public static implicit operator bool(State state)
        {
            return state != null;
        }

        /// <returns> The name of the current state. </returns>
        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}