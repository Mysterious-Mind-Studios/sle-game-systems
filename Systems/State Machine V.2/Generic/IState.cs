namespace FSMSystemV2.Generic
{
    public interface IState<in T>
    {
        void OnEnter(T machine);
        void Process(T machine);
        void OnExit(T machine);
    }
}