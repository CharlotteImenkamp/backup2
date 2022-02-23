
/// <summary>
/// The statemachine structures the program in subtasks.
/// It should be initialized not more than once per manager.
/// </summary>
public class StateMachine
{
    #region Private Fields

    private IState currentState;
    private IState previousState;

    #endregion Private Fields

    #region Public Functions

    /// <summary>
    /// Change StateMachine to the new state. Calls the "Exit" function of the previous state. 
    /// and the "Enter" function of the new State.
    /// </summary>
    /// <param name="newState"> New state, which inherits from IState. </param>
    public void ChangeState(IState newState)
    {
        if (this.currentState != null)
        {
            this.currentState.Exit();
        }
        // set previous State
        this.previousState = this.currentState;

        // set current State
        this.currentState = newState;
        this.currentState.Enter();
    }

    /// <summary>
    /// Called by the monobehaviour update method
    /// </summary>
    public void ExecuteStateUpdate()
    {
        var runningState = this.currentState;
        if (runningState != null)
            runningState.Execute();
    }

    /// <summary>
    /// Comment: Not used at this point, but maybe on later versions.
    /// </summary>
    public void SwitchToPreviousState()
    {
        this.currentState.Exit();
        this.currentState = this.previousState;
        this.currentState.Enter();
    }

    /// <summary>
    /// Set StateMachine into Idle, e.g. on the end of the game
    /// </summary>
    public void SwitchToIdle()
    {
        this.previousState = currentState;
        if (currentState != null)
        {
            this.currentState.Exit();
            this.currentState = null;
        }
    }

    #endregion Public Functions
}
