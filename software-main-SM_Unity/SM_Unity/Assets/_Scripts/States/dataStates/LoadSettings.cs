using UnityEngine;

/// <summary>
/// Get datapaths from general settings and save data in Datamanager.
/// Called at Start and on Reset of Datamanager.
/// </summary>
class LoadSettings : IState
{
    #region IState Functions

    /// <summary>
    /// Load saved setting files. The filepath is listed in the Gamemanager
    /// </summary>
    public void Enter()
    {
        GameManager.Instance.DebugText.text = "LoadSettings Enter";
        Debug.Log("LoadSettings Enter");

        //Load user data
        if (GameManager.Instance.GeneralSettings.NewUserData.Count != 0)
            DataManager.Instance.NewUserData = DataFile.LoadUserSets(GameManager.Instance.GeneralSettings.NewUserData);
        if (GameManager.Instance.GeneralSettings.IncompleteUserData.Count != 0)
            DataManager.Instance.IncompleteUserData = DataFile.LoadUserSets(GameManager.Instance.GeneralSettings.IncompleteUserData);
        if (GameManager.Instance.GeneralSettings.CompleteUserData.Count != 0)
            DataManager.Instance.CompleteUserData = DataFile.LoadUserSets(GameManager.Instance.GeneralSettings.CompleteUserData);
    }

    public void Execute() { }

    public void Exit() { }

    #endregion IState Functions
}
