
/// <summary>
/// Data class to hold user specific data
/// </summary>
[System.Serializable]
public class UserSettingsData
{
    #region Constructors

    public UserSettingsData() { }

    public UserSettingsData(string userID, UserSet set, float updateRate)
    {
        UserID = userID;
        this.Set = set;
        this.UpdateRate = updateRate;
    }

    #endregion  Constructors

    #region Public Fields

    // User
    public string UserID;
    public UserSet Set;

    // Save
    public float UpdateRate;

    #endregion

    #region Validation

    public bool IsValid()
    {
        if (UserID != "")
            return true;
        else
            return false;
    }

    #endregion

    #region Public Functions

    public void Clear()
    {
        UserID = "";
        Set = UserSet.None;
        UpdateRate = 0.0f;
    }

    #endregion Public Functions
}



