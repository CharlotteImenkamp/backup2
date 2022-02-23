using UnityEngine;

/// <summary>
/// State to spawn one movable test object.
/// </summary>
public class LocationTest : IState
{
    #region IState Functions

    public void Enter()
    {
        GameManager.Instance.DebugText.text = "LocationTest::Enter()";
        Debug.Log("LocationTest::Enter()");

        // Call submanagers
        var SubManagers = GameManager.Instance.AttachedSubManagers;
        foreach (SubManager subManager in SubManagers)
            subManager.OnGameStateEntered(this.ToString());
    }

    // No repeated task, hence execute is empty
    public void Execute() { }

    public void Exit()
    {
        GameManager.Instance.DebugText.text = "LocationTest::Exit()";

        // Call submanagers
        var SubManagers = GameManager.Instance.AttachedSubManagers;
        foreach (SubManager subManager in SubManagers)
            subManager.OnGameStateLeft(this.ToString());
    }

    #endregion IState Functions

}
