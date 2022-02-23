using UnityEngine;

/// <summary>
/// Second part of game after location test
/// </summary>
public class LocationEstimation : IState
{
    #region IState Functions

    public void Enter()
    {
        GameManager.Instance.DebugText.text = "LocationEstimation::Enter()";
        Debug.Log("LocationEstimation::Enter()");

        // Call submanagers
        var SubManagers = GameManager.Instance.AttachedSubManagers;
        foreach (SubManager subManager in SubManagers)
            subManager.OnGameStateEntered(this.ToString());
    }


    // No repeated task, hence execute is empty
    public void Execute() { }

    public void Exit()
    {
        GameManager.Instance.DebugText.text = "LocationEstimation::Exit()";
        Debug.Log("LocationEstimation::Exit()");

        // Call submanagers
        var SubManagers = GameManager.Instance.AttachedSubManagers;
        foreach (SubManager subManager in SubManagers)
            subManager.OnGameStateLeft(this.ToString());
    }

    #endregion IState Functions

}
