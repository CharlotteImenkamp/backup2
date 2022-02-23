using UnityEngine;


/// <summary>
/// Gamestate, which is called in GameStateManager after nitialization
/// </summary>
public class SettingsMenu : IState
{
    #region IState Functions
    public void Enter()
    {
        GameManager.Instance.DebugText.text = "OpenSettingsMenu::Enter()";
        Debug.Log("OpenSettingsMenu::Enter()");

        // Call submanagers
        var SubManagers = GameManager.Instance.AttachedSubManagers;
        foreach (SubManager subManager in SubManagers)
            subManager.OnGameStateEntered(this.ToString());
    }

    // No repeated task, hence execute is empty
    public void Execute() { }

    public void Exit()
    {
        GameManager.Instance.DebugText.text = "OpenSettingsMenu::Exit()";

        // Call submanagers
        var SubManagers = GameManager.Instance.AttachedSubManagers;
        foreach (SubManager subManager in SubManagers)
            subManager.OnGameStateLeft(this.ToString());
    }

    #endregion IState Functions
}
