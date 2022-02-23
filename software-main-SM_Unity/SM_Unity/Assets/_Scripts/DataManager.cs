using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


/// <summary>
/// Singelton class, which is instantiated in the editor by the GameManager script 
/// Holds data functions and parameters.
/// </summary>
public class DataManager : MonoBehaviour
{
    #region Instance

    private static DataManager instance = null;

    public static DataManager Instance { get => instance; }

    #endregion Instance and awake

    #region Public Fields

    public struct Data
    {
        ObjectData objData;
        UserSettingsData userData;

        public Data(ObjectData objData, UserSettingsData userData)
        {
            if (!objData.IsValid() || !userData.IsValid())
                throw new InvalidDataException();

            this.objData = objData ?? throw new ArgumentNullException(nameof(objData));
            this.userData = userData ?? throw new ArgumentNullException(nameof(userData));
        }

        public bool IsValid()
        {
            if (objData.IsValid() && userData.IsValid())
                return true;
            else
                return false;
        }

        public void Clear()
        {
            objData?.Clear();
            userData?.Clear();
        }

        public UserSettingsData UserData { get => userData; set => userData = value ?? throw new ArgumentNullException(nameof(value), "Name cannot be null"); }
        public ObjectData ObjData { get => objData; set => objData = value ?? throw new ArgumentNullException(nameof(value), "Name cannot be null"); }
    }

    // Objectdata
    public List<GameObject> ObjectsInScene { get => objectsInScene; set => objectsInScene = value ?? throw new ArgumentNullException(nameof(value), "Name cannot be null"); }
    public List<GameObject> MovingObjects { get => movingObjects; set => movingObjects = value ?? throw new ArgumentNullException(nameof(value), "Name cannot be null"); }

    // Headdata
    public HeadData CurrentHeadData { get => currentHeadData; set => currentHeadData = value ?? throw new ArgumentNullException(nameof(value), "Name cannot be null"); }

    // Userdata
    public List<ObjectData> NewSets { get => newSettings; set => newSettings = value ?? throw new ArgumentNullException(nameof(value), "Name cannot be null"); }
    public List<Data> IncompleteUserData { get => incompleteUserData; set => incompleteUserData = value ?? throw new ArgumentNullException(nameof(value), "Name cannot be null"); }
    public List<Data> CompleteUserData { get => completeUserData; set => completeUserData = value ?? throw new ArgumentNullException(nameof(value), "Name cannot be null"); }
    public List<Data> NewUserData { get => newUserData; set => newUserData = value ?? throw new ArgumentNullException(nameof(value), "Name cannot be null"); }
    public Data CurrentSet { get => currentSet; set => currentSet = value; }


    #endregion Public Fields

    #region Private Fields

    // Statemachine
    private static StateMachine dataStateMachine = new StateMachine();

    // Userdata
    private List<Data> newUserData;
    private List<Data> incompleteUserData;
    private List<Data> completeUserData;
    private Data currentSet;

    // Settings
    private List<ObjectData> newSettings;

    // Objectdata
    private List<GameObject> objectsInScene;
    private List<GameObject> movingObjects;

    // Headdata
    private HeadData currentHeadData;

    #endregion Private Fields

    #region MonoBehaviour Functions

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            Debug.LogError("Instance of DataManager destroyed.");
        }
        else
            instance = this;
    }

    /// <summary>
    /// Initialize Parameters at start
    /// </summary>
    void Start()
    {
        ResetToDefault();
    }

    /// <summary>
    /// Called every frame.
    /// Execute update of statemachine and update data.
    /// </summary>
    private void Update()
    {
        currentHeadData.SetCameraParameters(Camera.main.transform);
        currentHeadData.SetGazeParameters(CoreServices.InputSystem.EyeGazeProvider.GazeOrigin, CoreServices.InputSystem.EyeGazeProvider.GazeDirection);

        dataStateMachine.ExecuteStateUpdate();
    }

    #endregion MonoBehaviour Functions

    #region Public Functions

    #region Save

    /// <summary>
    /// Updates settings in GameManager and saves the data.
    /// </summary>
    /// <param name="data">Data to save</param>
    public void SetAndSaveNewSettings(Data data)
    {
        if (data.IsValid() && data.ObjData.GameObjects.Count != 0)
        {
            // GameManager
            string dataFolder = GameManager.Instance.GeneralSettings.UserDataFolder + "/User" + data.UserData.UserID.ToString();
            string mainFolder = GameManager.Instance.MainFolder;

            // FileNames
            string startDataName = GameManager.Instance.StartDataName + data.UserData.UserID.ToString();
            string userFileName = "user" + data.UserData.UserID.ToString();

            // Save into user folder and into settings folder
            DataFile.Save<ObjectData>(data.ObjData, Path.Combine(mainFolder, dataFolder), startDataName);
            DataFile.Save<UserSettingsData>(data.UserData, Path.Combine(mainFolder, dataFolder), userFileName);

            // Add to general settings
            GameManager.Instance.GeneralSettings.NewUserData.Add("User" + data.UserData.UserID.ToString() + "/" + userFileName);

            var dat = GameManager.Instance.GeneralSettings;
            GameManager.Instance.GeneralSettings = dat;
        }
        else
        {
            GameManager.Instance.DebugText.text = "DataManager::SetCurrentSettings no valid data.";
            throw new ArgumentNullException("No valid data");
        }
    }

    #endregion Save

    #region Reset

    /// <summary>
    /// Resets rotation of all objects in the scene. This is useful, if the user cannot reset the rotation by the manipulation handles. 
    /// </summary>
    public void ResetObjectRotation()
    {
        foreach (GameObject obj in ObjectsInScene)
        {
            var a = currentSet.ObjData.GameObjects;
            var b = a.Find(x => x.Objectname.Equals(obj.name));
            var c = b.GlobalRotation;
            obj.transform.rotation = c;
            obj.transform.rotation = Quaternion.Euler(obj.transform.rotation.x, 0f, transform.rotation.z);
        }
    }

    /// <summary>
    /// Resets position of all objcts in the scene.
    /// Last option, if anything does not work as expected.
    /// </summary>
    public void ResetObjectPosition()
    {
        if (GameManager.Instance.GameType == GameType.Prices)
        {
            foreach (GameObject obj in ObjectsInScene)
            {
                var a = currentSet.ObjData.GameObjects;
                var b = a.Find(x => x.Objectname.Equals(obj.name));
                var c = b.GlobalPosition;
                obj.transform.position = c;
            }
        }

        if (GameManager.Instance.GameType == GameType.Locations)
        {
            GameManager.Instance.ParentSideTable.GetComponent<GridObjectCollection>().UpdateCollection();
        }
    }

    /// <summary>
    /// Instantiates parameters and changes state to start state.
    /// </summary>
    public void ResetToDefault()
    {
        // Parameters
        if (newSettings == null)
            newSettings = new List<ObjectData>();

        if (completeUserData == null)
            completeUserData = new List<Data>();

        if (incompleteUserData == null)
            incompleteUserData = new List<Data>();

        if (newUserData == null)
            newUserData = new List<Data>();

        if (movingObjects == null)
            movingObjects = new List<GameObject>();

        if (objectsInScene == null)
            objectsInScene = new List<GameObject>();

        if (currentHeadData == null)
            currentHeadData = new HeadData();

        // Start loading data
        dataStateMachine.ChangeState(new LoadSettings());
    }

    #endregion Reset

    #region Log 

    /// <summary>
    /// start, if GameState changed to Location Estimation or PriceEstimation
    /// </summary>
    public void StartDataLogging()
    {
        dataStateMachine.ChangeState(new LogData(GameManager.Instance.GameType));
    }

    public void StopDataLogging()
    {
        dataStateMachine.SwitchToIdle();
    }

    #endregion Log

    #endregion Public Functions
}
