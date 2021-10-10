
namespace FSMSystemV2.Generic
{
    public interface IStateMachine<T>
    {
        IState<T> defaultState { get; }
        IState<T> currentState { get; }

        bool SetState(IState<T> state);
        void SetState<State>() where State : IState<T>;
    }
}