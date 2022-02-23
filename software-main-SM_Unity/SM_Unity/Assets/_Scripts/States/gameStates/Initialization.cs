using UnityEngine;

/// <summary>
/// Gamestate, which is called on GameManager::Start()
/// </summary>
public class Initialization : IState
{
    #region IState Functions

    /// <summary>
    /// Initialize all subManagers
    /// </summary>
    public void Enter()
    {
        GameManager.Instance.DebugText.text = "Initialization::Enter()";
        Debug.Log("Initialization::Enter()");

        // Call submanagers
        var SubManagers = GameManager.Instance.AttachedSubManagers;
        foreach (SubManager subManager in SubManagers)
            subManager.OnGameStateEntered(this.ToString());
    }

    // No repeated task, hence execute is empty
    public void Execute() { }

    public void Exit()
    {
        GameManager.Instance.DebugText.text = "Initialization::Exit()";

        // Call submanagers
        var SubManagers = GameManager.Instance.AttachedSubManagers;
        foreach (SubManager subManager in SubManagers)
            subManager.OnGameStateLeft(this.ToString());
    }

    #endregion IState Functions

}
