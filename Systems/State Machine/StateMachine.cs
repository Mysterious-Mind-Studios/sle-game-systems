
using System;
using System.Collections.Generic;

using UnityEngine;

namespace SLE.Systems.StateMachine
{
    [DisallowMultipleComponent]
    public class StateMachine : MonoBehaviour,
#if UNITY_EDITOR
        ISerializationCallbackReceiver
#endif
    {
#if UNITY_EDITOR
        [Space]
        [SerializeField]
#endif
        private State _defaultState;

#if UNITY_EDITOR
        [Space]
        [SerializeField]
        private List<State> _statesList = new List<State>();

        private readonly HashSet<State> machineStates = new HashSet<State>();
#else
        private readonly HashSet<State> machineStates = new HashSet<State>();
#endif

        private State _state;

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
                return _state;
            }
        }

        private void Start()
        {
            State[] states = GetComponents<State>();

            foreach (var state in states)
            {
                machineStates.Add(state);
                
                state.Initialize(this);
            }

            _state = _defaultState;

            _state.EnterState();
        }
        private void Update()
        {
            _state.OnStateUpdate();
        }

        public bool SetState(State state)
        {
            bool success = false;

            if (machineStates.Contains(state))
            {
                _state.ExitState();

                _state = state;

                _state.EnterState();

                success = true;
            }

            return success;
        }
        public bool SetState<StateType>() where StateType : State
        {
            //if the state can be found in the list of states 
            //already created, switch to the existing version
            foreach (State state in machineStates)
            {
                if (state is StateType st)
                {
                    return SetState(st);
                }
            }

            //if the state is not found in the list,
            //see if it is on the gameobject.
            if (TryGetComponent(out State stateComponent))
            {
                stateComponent.Initialize(this);
                machineStates.Add(stateComponent);

                return SetState(stateComponent);
            }

            return false;
        }

        public T GetState<T>() where T : State
        {
            foreach (var state in machineStates)
            {
                if (state is T)
                    return (T)state;
            }
           
            return null;
        }

#if UNITY_EDITOR
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _statesList.Clear();
            _statesList.AddRange(machineStates);
        }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            machineStates.Clear();
            machineStates.SymmetricExceptWith(_statesList);
        }
#endif
    }
}