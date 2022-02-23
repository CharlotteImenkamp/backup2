using System.Collections.Generic;

// Class to save Application Data
[System.Serializable]
public class ApplicationData
{
    #region Constructor
    /// <summary>
    /// Helper method to generate default file for debugging
    /// </summary>
    /// <returns></returns>
    public static ApplicationData DefaultGeneralSettingsFile()
    {
        ApplicationData defaultData = new ApplicationData();

        defaultData.UserDataFolder = "data";
        defaultData.NewUserData = new List<string> { };
        defaultData.IncompleteUserData = new List<string> { };
        defaultData.CompleteUserData = new List<string> { };

        return defaultData;
    }

    #endregion constructor

    #region Validation
    public bool IsValid()
    {
        if (UserDataFolder == "")
            return false;
        else if (NewUserData == null || IncompleteUserData == null || CompleteUserData == null)
            return false;
        else
            return true;
    }

    #endregion

    #region Public Fields

    // Folders 
    public string UserDataFolder;

    // Sets
    public List<string> NewUserData;
    public List<string> IncompleteUserData;
    public List<string> CompleteUserData;

    #endregion Public Fields
}

