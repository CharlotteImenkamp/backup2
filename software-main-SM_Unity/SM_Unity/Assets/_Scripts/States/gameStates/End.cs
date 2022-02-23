using UnityEngine;

/// <summary>
/// End game after price estimation.
/// </summary>
public class End : IState
{
    #region IState Functions

    public void Enter()
    {
        // Save data
        DataFile.OverwriteData<ApplicationData>(GameManager.Instance.GeneralSettings, GameManager.Instance.MainFolder, "generalSettings");

        GameManager.Instance.DebugText.text = "End::Enter()";
        Debug.Log("End::Enter()");

        // Call submanagers
        var SubManagers = GameManager.Instance.AttachedSubManagers;
        foreach (SubManager subManager in SubManagers)
            subManager.OnGameStateEntered(this.ToString());
    }

    // No repeated task, hence execute is empty
    public void Execute() { }

    public void Exit() { }

    #endregion IState Functions

}
