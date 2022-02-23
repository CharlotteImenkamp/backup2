using UnityEngine;

/// <summary>
/// State which is called after price task. Can either be followed by location test or price test of new user.
/// </summary>
public class Pause : IState
{
    #region IState Functions
    public void Enter()
    {
        GameManager.Instance.DebugText.text = "Pause::Enter()";
        Debug.Log("Pause::Enter()");

        // Save Data
        DataFile.OverwriteData<ApplicationData>(GameManager.Instance.GeneralSettings, GameManager.Instance.MainFolder, "generalSettings");

        // Call submanagers
        var SubManagers = GameManager.Instance.AttachedSubManagers;
        foreach (SubManager subManager in SubManagers)
            subManager.OnGameStateEntered(this.ToString());
    }

    // No repeated task, hence execute is empty
    public void Execute() { }

    public void Exit()
    {
        // Call submanagers
        var SubManagers = GameManager.Instance.AttachedSubManagers;
        foreach (SubManager subManager in SubManagers)
            subManager.OnGameStateLeft(this.ToString());
    }

    #endregion IState Functions
}
