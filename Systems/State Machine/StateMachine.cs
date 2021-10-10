using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachineSystem
{
    [DisallowMultipleComponent]
    public abstract class StateMachine : MonoBehaviour, ISerializationCallbackReceiver
    {
        private const string stateTooltip = "Current behaviour of the State Machine. (Read Only)";

        [Space]
        [Tooltip(stateTooltip)]
        [SerializeField]
        private string status;

        [Space]
        [SerializeField]
        private State _defaultState;

        [Space]
        [SerializeField]
        private List<State> _statesList;

        [Space]
        [SerializeField]
        private StateRefreshRateQuality _stateUpdateQuality;

        private readonly HashSet<State> statesList = new HashSet<State>();

        [NonSerialized]
        private State _currentState;

        public State defaultState
        {
            get
            {
                return _defaultState;
            }
        }

        public State currentState
        {
            get
            {
                return _currentState;
            }
        }

        public StateRefreshRateQuality stateUpdateQuality
        {
            get => _stateUpdateQuality;
        }

        private void Start()
        {
            foreach (var state in GetComponents<State>())
            {
                if (statesList.Add(state))
                    state.Initialize(this);
            }

            SetState(defaultState);
        }

        /// <summary>
        /// Switch the currentState to a specific State object
        /// </summary>
        /// <param name="state">
        /// The state object to set as the currentState</param>
        /// <returns>Whether the state was changed</returns>
        public bool SetState(State state)
        {
            bool success = false;

            if (state)
            {
                if (state != _currentState)
                {
                    State oldState = _currentState;
                    _currentState = state;

                    if (oldState)
                        oldState.StateExit();

                    _currentState.StateEnter();

                    status = _currentState.ToString();

                    success = true;
                }
                else
                    return true;
            }

            return success;
        }

        /// <summary>
        /// Switch the currentState to a State of a the given type.
        /// </summary>
        /// <typeparam name="StateType">
        /// The type of state to use for the currentState</typeparam>
        /// <returns>Whether the state was changed</returns>
        public bool SetState<StateType>() where StateType : State
        {
            bool success;

            //if the state can be found in the list of states 
            //already created, switch to the existing version
            foreach (State state in statesList)
            {
                if (state is StateType)
                {
                    success = SetState(state);
                    return success;
                }
            }

            //if the state is not found in the list,
            //see if it is on the gameobject.
            if (TryGetComponent(out State stateComponent))
            {
                stateComponent.Initialize(this);
                statesList.Add(stateComponent);
                success = SetState(stateComponent);
                return success;
            }

            //if it is not on the gameobject,
            //make a new instance
            State newState = gameObject.AddComponent<StateType>();
            newState.Initialize(this);
            statesList.Add(newState);
            success = SetState(newState);

            return success;
        }

        /// <summary>
        /// First set the target State on the StateMachine, and them return it.
        /// </summary>
        /// <typeparam name="StateType"> The State target type. </typeparam>
        /// <returns> Whether the newly set State. </returns>
        public StateType SetAndGetState<StateType>() where StateType : State
        {
            SetState<StateType>();
            return GetState<StateType>();
        }

        /// <summary>
        /// Try to get the target State whether on the StateMachine's List or added Component.
        /// </summary>
        /// <typeparam name="State_T">
        /// The type of the State target to look for in the StateMachine. </typeparam>
        /// <returns>Whether the existing State on the StateMachine. </returns>     
        public StateType GetState<StateType>() where StateType : State
        {
            foreach (var state in statesList)
            {
                if (state is StateType)
                    return state as StateType;
            }
            TryGetComponent<StateType>(out var stateComponent);
            return stateComponent;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _statesList = new List<State>(statesList);
        }
        void ISerializationCallbackReceiver.OnAfterDeserialize() { }
    }
}