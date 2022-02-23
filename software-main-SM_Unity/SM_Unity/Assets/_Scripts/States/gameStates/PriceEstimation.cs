using UnityEngine;

/// <summary>
/// State where Prices are estimated. 
/// </summary>
public class PriceEstimation : IState
{
    #region IState Functions

    public void Enter()
    {
        Debug.Log("PriceEstimation::Enter()");
        GameManager.Instance.DebugText.text = "PriceEstimation::Enter()";

        // Call submanagers
        var SubManagers = GameManager.Instance.AttachedSubManagers;
        foreach (SubManager subManager in SubManagers)
            subManager.OnGameStateEntered(this.ToString());
    }

    // No repeated task, hence execute is empty
    public void Execute() { }

    public void Exit()
    {
        GameManager.Instance.DebugText.text = "PriceEstimation::Exit()";

        // Call submanagers
        var SubManagers = GameManager.Instance.AttachedSubManagers;
        foreach (SubManager subManager in SubManagers)
            subManager.OnGameStateLeft(this.ToString());
    }
    #endregion IState Functions

}
