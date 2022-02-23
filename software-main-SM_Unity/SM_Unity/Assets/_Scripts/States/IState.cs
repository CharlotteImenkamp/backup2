
/// <summary>
/// Interface to define state methods.
/// Operates in combination with the StateMachine.
/// </summary>
public interface IState
{

    void Enter();

    void Execute();

    void Exit();

}
