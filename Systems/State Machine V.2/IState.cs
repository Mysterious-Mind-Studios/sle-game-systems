namespace FSMSystemV2
{
    public interface IState
    {
        void OnEnter(IStateMachine machine);

        void Process(IStateMachine machine);

        void OnExit(IStateMachine machine);
    }
}