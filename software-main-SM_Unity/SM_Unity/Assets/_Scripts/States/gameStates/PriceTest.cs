using UnityEngine;


/// <summary>
/// State where price task is trained with one object.
/// </summary>
public class PriceTest : IState
{
    #region IState Functions

    public void Enter()
    {
        GameManager.Instance.DebugText.text = "PriceTest::Enter()";
        Debug.Log("PriceTest::Enter()");

        // Call submanagers
        var SubManagers = GameManager.Instance.AttachedSubManagers;
        foreach (SubManager subManager in SubManagers)
            subManager.OnGameStateEntered(this.ToString());
    }

    // No repeated task, hence execute is empty
    public void Execute() { }

    public void Exit()
    {
        GameManager.Instance.DebugText.text = "PriceTest::Exit()";

        // Call submanagers
        var SubManagers = GameManager.Instance.AttachedSubManagers;
        foreach (SubManager subManager in SubManagers)
            subManager.OnGameStateLeft(this.ToString());
    }

    #endregion IState Functions

}
