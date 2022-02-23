using UnityEngine;

/// <summary>
/// Submanager class to open and close menus, if necessary
/// </summary>
public class UIManager : SubManager
{
    #region Private Fields

    private GameObject generalSettingsMenu;
    private GameObject newSettingsMenu;
    private GameObject oldSettingsMenu;
    private GameObject pauseMenu;
    private GameObject[] allMenus;

    #endregion Private Fields

    #region SubManager Functions
    /// <summary>
    /// Initialization of parameters.
    /// Closes all menus.
    /// </summary>
    public void Initialize()
    {
        // Menus from GameManager
        generalSettingsMenu = GameManager.Instance.MainMenu;
        newSettingsMenu = GameManager.Instance.StartMenu;
        oldSettingsMenu = GameManager.Instance.NewLayout;
        pauseMenu = GameManager.Instance.PauseMenu;

        allMenus = new GameObject[] { generalSettingsMenu, newSettingsMenu, oldSettingsMenu, pauseMenu };

        CloseAllMenus();
    }

    /// <summary>
    /// Reset menu states
    /// </summary>
    public override void Reset()
    {
        CloseAllMenus();
        OpenMenu(generalSettingsMenu);
        GameManager.Instance.DebugText.text = "UIManager::Reset";
        Debug.Log("UIManager::Reset");
    }

    #region Gamestates

    /// <summary>
    /// Check, if any task needs to be done at new game state
    /// </summary>
    /// <param name="newState"></param>
    public override void OnGameStateEntered(string newState)
    {
        switch (newState)
        {
            case "Initialization":
                Initialize();
                break;

            case "SettingsMenu":
                OpenMenu(generalSettingsMenu);
                break;

            case "LocationTest":
                break;

            case "LocationEstimation":
                break;

            case "PriceTest":
                break;

            case "PriceEstimation":
                break;

            case "Pause":
                CloseAllMenus();
                OpenMenu(pauseMenu);
                GameManager.Instance.ContinueWithLocationsButton.SetActive(true);
                break;

            case "End":
                CloseAllMenus();
                OpenMenu(pauseMenu);
                GameManager.Instance.ContinueWithLocationsButton.SetActive(false);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Check, if any task needs to be done before next game state
    /// </summary>
    /// <param name="oldState">State which is left</param>
    public override void OnGameStateLeft(string oldState)
    {
        if (oldState == "SettingsMenu" || oldState == "Pause")
            CloseAllMenus();
    }

    #endregion Gamestates

    #endregion SubManager Functions

    #region Helper Functions

    /// <summary>
    /// Open specific menu and close others
    /// </summary>
    /// <param name="menu"></param>
    private void OpenMenu(GameObject menu)
    {
        foreach (GameObject obj in allMenus)
        {
            obj.SetActive(false);
        }

        menu.SetActive(true);
    }

    /// <summary>
    /// Close all menues
    /// </summary>
    private void CloseAllMenus()
    {
        foreach (GameObject obj in allMenus)
        {
            obj.SetActive(false);
        }
    }

    #endregion Helper Functions

}
