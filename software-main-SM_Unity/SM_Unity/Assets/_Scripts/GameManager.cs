using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// First and only MonoBehaviour script, which needs to be in the scene. 
/// All references to scene components are stored here and other scripts can reach to the instance and read the parameters.
/// Provides access to other scripts via public functions, which are called in the editor.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Fields

    #region Editor

    [Header("Menu Objects")]
    [Tooltip("Add Menu Objects for UIManager")]
    public GameObject MainMenu;
    public GameObject StartMenu;
    public GameObject NewLayout;
    public GameObject PauseMenu;
    public GameObject ContinueWithLocationsButton;

    [Header("InteractionObjects")]
    public GameObject ParentPlayTable;
    public GameObject ParentSideTable;
    public GameObject InteractionObjects;

    [Header("Spawn Points")]
    public Transform SpawnPointGame;
    public Transform SpawnPointSide;

    [Header("BoundingBox")]
    public Material BoundingBox;
    public Material BoundingBoxGrabbed;
    public Material BoundingBoxHandleWhite;
    public Material BoundingBoxHandleBlueGrabbed;
    public GameObject BoundingBox_RotateHandle;

    [Header("Button")]
    public float ReactivationTimeUserButton = 10.0f;
    public GameObject UserButton;

    [Header("Data")]
    public float BackupPeriod = 60.0f;

    [Header("Debug")]
    public GameObject DebugTextObject;
    [NonSerialized]
    public TextMeshPro DebugText;

    #endregion Editor

    #region Object Manangement

    [NonSerialized]
    public Vector3 InteractionObjectsInitialPosition;

    #endregion Object Management

    #region File Management
    public ApplicationData GeneralSettings
    {
        get => generalSettings;
        set
        {
            generalSettings = value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null");
            DataManager.Instance.CompleteUserData = DataFile.LoadUserSets(generalSettings.CompleteUserData);
            DataManager.Instance.IncompleteUserData = DataFile.LoadUserSets(generalSettings.IncompleteUserData);
            DataManager.Instance.NewUserData = DataFile.LoadUserSets(generalSettings.NewUserData);

            if (generalSettings.IsValid())
                DataFile.OverwriteData<ApplicationData>(generalSettings, MainFolder, "generalSettings");
            else
                throw new ArgumentException(nameof(generalSettings), "is invalid");
        }
    }

    [NonSerialized]
    private ApplicationData generalSettings;

    public string MainFolder { get; internal set; }

    private float updateRate;

    public string StartDataName { get; internal set; }

    public float UpdateRate { get => updateRate; set => updateRate = value; }

    #endregion File

    #region Event

    // public event paramters 
    [NonSerialized]
    public UnityEvent OnUserButtonClicked;

    // private event parameters
    private bool buttonEnabled;

    #endregion Event

    #region Game Flow 

    [NonSerialized]
    public GameType GameType;

    #endregion Game Flow 

    #region Script Management

    // public
    public List<Type> AttachedManagerScripts { get => attachedManagerScripts; set => attachedManagerScripts = value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"); }
    public List<SubManager> AttachedSubManagers { get => attachedSubManagers; set => attachedSubManagers = value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"); }

    // private
    private List<Type> attachedManagerScripts;
    private List<SubManager> attachedSubManagers;

    #endregion Script Management

    #region Instance

    private static GameManager instance = null;
    public static GameManager Instance { get => instance; }

    #endregion

    #endregion Fields

    #region MonoBehaviour Functions

    /// <summary>
    /// Manage Instance and Add depending Scripts
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            Debug.LogError("Instance of GameManager destroyed.");
        }
    }

    /// <summary>
    /// Instantiate and set default parameters
    /// </summary>
    void Start()
    {
        buttonEnabled = true;

        // Debug
        DebugText = DebugTextObject.GetComponent<TextMeshPro>();

        // Check input parameters
        if (MainMenu == null || StartMenu == null || NewLayout == null || PauseMenu == null)
        {
            DebugText.text = "Not all Menu Parameters are Set in GameManager.";
            throw new ArgumentNullException("Not all Menu Parameters are Set in GameManager.");
        }

        // Events
        if (OnUserButtonClicked == null)
            OnUserButtonClicked = new UnityEvent();

        // InteractionObjects
        InteractionObjectsInitialPosition = InteractionObjects.transform.position;

        // File parameters
        MainFolder = "DataFiles";
        StartDataName = "StartLocationPrices";
        updateRate = 1.0f;

        generalSettings = DataFile.SecureLoad<ApplicationData>(Path.Combine(MainFolder, "generalSettings"));

        // Add managers of type monobehaviour
        attachedManagerScripts = new List<Type>();
        AddManagerToScene(typeof(GameStateManager));
        AddManagerToScene(typeof(DataManager));

        //Add submanager
        attachedSubManagers = new List<SubManager>();
        AddSubManager(new UIManager());
        AddSubManager(new ObjectManager());

        ResetToDefault();
    }

    #endregion MonoBehaviour Functions

    #region Button Functions

    /// <summary>
    /// Add to toggle buttons in editor, to get their input
    /// </summary>
    public void GetToggleState(GameObject obj)
    {
        if (obj.activeInHierarchy && obj.activeSelf)
        {
            if (obj.GetComponent<Interactable>().IsToggled)
                GameType = GameType.Prices;
            else
                GameType = GameType.Locations;
        }
    }

    /// <summary>
    /// Called in old settings menu on radio buttons and on pause menu at button "ContinueWithLocations".
    /// Sets game type and assigns listeners to user button.
    /// </summary>
    /// <param name="type">Either "Locations" or "Prices"</param>
    public void SetGameType(string type)
    {
        if (type == "Locations")
            GameType = GameType.Locations;
        else if (type == "Prices")
            GameType = GameType.Prices;
        else
            throw new ArgumentException("...GameManager SetGameType to " + type + " not possible.");
    }

    /// <summary>
    /// Called after function "SetGameType" to add listeners to "UserButton".
    /// The split is necessary to prevent wrong behaviour. 
    /// </summary>
    public void StartGame()
    {
        OnUserButtonClicked.RemoveAllListeners();
        OnUserButtonClicked.AddListener(() => GameStateManager.Instance.StartTestRun(GameManager.Instance.GameType));
    }

    /// <summary>
    /// Added to "UserButton" in Editor. 
    /// Invokes OnUserButtonClicked event and disables button for a time period
    /// </summary>
    public void UserButtonClicked()
    {
        // invoke event
        if (buttonEnabled)
            OnUserButtonClicked.Invoke();

        // deactivate user button
        UserButton.GetComponent<Interactable>().IsEnabled = false;
        UserButton.GetComponent<PressableButton>().enabled = false;
        UserButton.GetComponent<Interactable>().SetState(InteractableStates.InteractableStateEnum.Pressed, true);
        buttonEnabled = false;

        // reactivate user button
        StartCoroutine(EnableAfterSeconds());
    }

    /// <summary>
    /// Called in function "UserButtonClicked" to reenable user button. 
    /// </summary>
    IEnumerator EnableAfterSeconds()
    {
        yield return new WaitForSeconds(ReactivationTimeUserButton);

        UserButton.GetComponent<Interactable>().IsEnabled = true;
        UserButton.GetComponent<PressableButton>().enabled = true;
        UserButton.GetComponent<Interactable>().SetState(InteractableStates.InteractableStateEnum.Pressed, false);

        buttonEnabled = true;
    }

    /// <summary>
    /// Called in "HandMenu" in editor to set rotation to zero.
    /// </summary>
    public void ResetObjectRotation()
    {
        DataManager.Instance.ResetObjectRotation();
    }

    public void ResetObjectPosition()
    {
        DataManager.Instance.ResetObjectPosition();
    }

    /// <summary>
    /// Called in menu in editor when return buttons are pressed.
    /// </summary>
    public void ResetMenuValues()
    {
        ResetToDefault();
    }

    #endregion Button Functions

    #region Scene Functions

    /// <summary>
    /// Managers are added to the gameObject and collected in the list
    /// </summary>
    /// <param name="classType">Inherited type of Monobehaviour is required. Use with typeof().</param>
    private void AddManagerToScene(Type classType)
    {
        if (this.gameObject.GetComponent(classType) == null)
        {
            this.gameObject.AddComponent(classType);
            attachedManagerScripts.Add(classType);
        }
        else
        {
            DebugText.text = "GameManager::AddManagerToScene Manager already exists!";
            Debug.LogWarning("GameManager::AddManagerToScene Manager already exists!");
        }
    }

    /// <summary>
    /// Managers of type "SubManager" are created and collected in the list
    /// </summary>
    /// <param name="newSubManager"></param>
    private void AddSubManager(SubManager newSubManager)
    {
        if (newSubManager != null)
            attachedSubManagers.Add(newSubManager);
        else
        {
            DebugText.text = "GameManager::AddSubManager Failed to load SubManager";
            Debug.LogError("GameManager::AddSubManager Failed to load SubManager");
        }
    }

    #endregion Scene Functions

    #region Gameflow Functions

    /// <summary>
    /// Called in "PauseMenu" in editor. Resets attached scripts.
    /// </summary>
    public void NewUser()
    {
        ResetToDefault();
    }

    /// <summary>
    /// Calls reset function of all attached managers and submanagers and removes event listeners.
    /// </summary>
    private void ResetToDefault()
    {
        // Reset managers
        gameObject.GetComponent<GameStateManager>().ResetToDefault();
        gameObject.GetComponent<DataManager>().ResetToDefault();

        // Reset submanagers
        foreach (SubManager sub in attachedSubManagers)
            sub.Reset();

        // Reset event
        OnUserButtonClicked.RemoveAllListeners();

        GameType = GameType.None;
    }

    /// <summary>
    /// Called in "GeneralMenu" in editor on "QuitGame" button 
    /// Function does not work with hololens
    /// </summary>
    public void QuitGame()
    {
        // Application.Quit(); 
    }

    #endregion Gameflow Functions 

    #region File Functions

    /// <summary>
    /// Moves user set to next list in "generalSettings" parameter
    /// </summary>
    /// <param name="userID"> Current user ID </param>
    /// <param name="completedType">Game type, which is completed</param>
    public void UpdateGeneralSettings(string userID, GameType completedType)
    {
        if (generalSettings.IsValid())
        {
            if (completedType == GameType.Locations)
            {
                generalSettings.IncompleteUserData.Remove("User" + userID + "/" + "user" + userID);
                generalSettings.CompleteUserData.Add("User" + userID + "/" + "user" + userID);
            }
            else if (completedType == GameType.Prices)
            {
                generalSettings.NewUserData.Remove("User" + userID + "/" + "user" + userID);
                generalSettings.IncompleteUserData.Add("User" + userID + "/" + "user" + userID);
            }
            else
                throw new ArgumentException();
        }
        else
            throw new InvalidDataException("Invalid data");

    }

    #endregion File Functions
}




