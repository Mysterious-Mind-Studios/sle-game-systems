namespace FSMSystemV2
{
    public interface IStateMachine
    {
        IState defaultState { get; }
        IState currentState { get; }

        bool SetState(IState state);
        void SetState<State>() where State : IState;
    }
}