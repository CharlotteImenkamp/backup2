using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Prepare data, get periodic updates and save data. 
/// Logs data during price estimation and location estimation. 
/// </summary>
class LogData : IState
{
    #region Private Fields

    // Save 
    private string dataFolder;
    private string generalFolder;
    private string directoryPath;
    private string userID;

    // Filenames
    private string name_contLog;
    private string name_end;
    private string name_headData;
    private string name_start;

    // Unique filenames
    private string uqName_contLog;
    private string uqName_end;
    private string uqName_headData;
    private string uqName_start;

    // Json strings
    private string data_contLog;
    private string data_endState;
    private string data_startState;
    private string data_headData;

    // Data
    private float sampleRate; // in miliseconds

    // Time 
    private float prevTime, currTime, startTime;  // in seconds
    private bool firstLog;
    private float backupPeriod;  // in seconds

    // Game
    private GameType gameType;

    #endregion Private Fields

    #region Constructor

    /// <summary>
    /// Instantiates the State with the current game type. This decides whether the object data is logged or not.
    /// </summary>
    /// <param name="gameType"></param>
    public LogData(GameType gameType) => this.gameType = gameType;

    #endregion Constructor

    #region IState Functions

    /// <summary>
    /// Instantiate parameters, set default values and start data files
    /// </summary>
    public void Enter()
    {
        GameManager.Instance.DebugText.text = "LogData Enter";
        Debug.Log("LogData::Enter");

        // Get parameters from managers
        sampleRate = DataManager.Instance.CurrentSet.UserData.UpdateRate;
        userID = DataManager.Instance.CurrentSet.UserData.UserID.ToString();

        // Default
        data_contLog = "";
        data_endState = "";
        data_headData = "";

        // Time
        firstLog = true;
        backupPeriod = GameManager.Instance.BackupPeriod;

        // Save
        dataFolder = GameManager.Instance.GeneralSettings.UserDataFolder;
        generalFolder = GameManager.Instance.MainFolder;
        directoryPath = Path.Combine(Application.persistentDataPath, generalFolder, dataFolder, "User" + userID);

        // Directory
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        // Prepare files
        if (gameType == GameType.Locations)
        {
            PrepareHeadData();
            PrepareObjectData();
            GameManager.Instance.UpdateGeneralSettings(userID, GameType.Locations);
        }
        else if (gameType == GameType.Prices)
        {
            PrepareHeadData();
            GameManager.Instance.UpdateGeneralSettings(userID, GameType.Prices);
        }
        else
            throw new ArgumentException("LogData::Enter no valid GameType.");

        // Set time for update rate
        prevTime = Time.time;
        startTime = Time.time;
    }

    /// <summary>
    /// Periodically add data and save backup data in case of unexpected events.
    /// </summary>
    public void Execute()
    {
        currTime = Time.time;

        // Log frequency
        if (currTime - prevTime >= sampleRate)
        {
            // Add line to data
            if (gameType == GameType.Locations)
            {
                ExecuteHeadData();
                ExecuteObjectData();
            }
            else if (gameType == GameType.Prices)
                ExecuteHeadData();
            else
                throw new ArgumentException("LogData::Execute no valid GameType.");

            prevTime = currTime;
        }

        // Backup frequency
        if ((Time.time - startTime) % backupPeriod <= (0 + Time.deltaTime))
        {
            // Save data
            if (gameType == GameType.Locations)
            {
                BackupObjectData(firstLog);
                BackupHeadData(firstLog);
            }
            else if (gameType == GameType.Prices)
                BackupHeadData(firstLog);
            else
                throw new ArgumentException("LogData::Execute no valid GameType.");

            if (firstLog)
                firstLog = false;
        }
    }

    /// <summary>
    /// End log files and save 
    /// </summary>
    public void Exit()
    {
        if (gameType == GameType.Locations)
        {
            EndHeadData();
            EndObjectData();
        }
        else if (gameType == GameType.Prices)
        {
            EndHeadData();
        }
        else
            throw new ArgumentException("LogData::Exit no valid GameType.");

        GameManager.Instance.DebugText.text = "LogData Exit";
    }

    #endregion IState Functions

    #region Backup Functions

    /// <summary>
    /// Overwrite or saves copy of data to prevent data loss
    /// </summary>
    /// <param name="firstSave">If this is true, a new unique file is generated. If not, the previous file is overwritten.</param>
    private void BackupObjectData(bool firstSave)
    {
        // Last object positions
        // Copy data
        var tmp_end = data_endState + DataFile.AddLine<ObjectData>(GetObjectsInScene());

        // Keep json format
        tmp_end = tmp_end.TrimEnd('\n').TrimEnd('\r').TrimEnd(',');

        tmp_end += DataFile.EndFile(true);

        // Save copy
        var tmp_contLog = data_contLog.TrimEnd('\n').TrimEnd('\r').TrimEnd(',');
        tmp_contLog += DataFile.EndFile(true);

        // First object positions
        // Copy data and keep json format
        var tmp_start = data_startState.TrimEnd('\n').TrimEnd('\r').TrimEnd(',');
        tmp_start = tmp_start + DataFile.EndFile(true);

        if (firstSave)
        {
            // Save file and get unique file name to overwrite file later
            uqName_contLog = DataFile.Save(tmp_contLog, directoryPath, name_contLog);
            uqName_end = DataFile.Save(tmp_end, directoryPath, name_end);
            uqName_start = DataFile.Save(tmp_start, directoryPath, name_start);
        }
        else
        {
            // Overwrite file 
            DataFile.Overwrite(tmp_contLog, directoryPath, uqName_contLog);
            DataFile.Overwrite(tmp_end, directoryPath, uqName_end);
            DataFile.Overwrite(tmp_start, directoryPath, uqName_start);
        }
    }

    /// <summary>
    /// Overwrite or saves copy of data to prevent data loss
    /// </summary>
    /// <param name="firstSave">If this is true, a new unique file is generated. If not, the previous file is overwritten.</param>
    private void BackupHeadData(bool firstSave)
    {
        // Copy data and keep json format
        var tmp_headData = data_headData.TrimEnd('\n').TrimEnd('\r').TrimEnd(',');
        tmp_headData += DataFile.EndFile(true);

        // Save file and get unique file name to overwrite file later
        if (firstSave)
            uqName_headData = DataFile.Save(tmp_headData, directoryPath, name_headData);
        else
            DataFile.Overwrite(tmp_headData, directoryPath, uqName_headData);
    }

    #endregion 

    #region Data Functions

    /// <summary>
    /// Handles moving objects, start positions and end positions. 
    /// Generates filenames and writes first lines of files.
    /// </summary>
    void PrepareObjectData()
    {
        // Filenames
        var currentSet = DataManager.Instance.CurrentSet;
        name_contLog = "MovingObject" + GameManager.Instance.GameType.ToString() + currentSet.UserData.UserID.ToString();
        name_end = "EndObject" + GameManager.Instance.GameType.ToString() + currentSet.UserData.UserID.ToString();
        name_start = "StartObject" + GameManager.Instance.GameType.ToString() + currentSet.UserData.UserID.ToString();

        // Start writing
        data_contLog += DataFile.StartFile();
        data_endState += DataFile.StartFile();
        data_startState += DataFile.StartFile();
        data_startState += DataFile.AddLine<ObjectData>(GetObjectsInScene());
    }

    /// <summary>
    /// Add new data, if an object is manipulated
    /// </summary>
    void ExecuteObjectData()
    {
        var data = GetMovingObject();
        if (data != null && data.GameObjects.Count != 0)
            data_contLog += DataFile.AddLine<ObjectData>(data);
    }

    /// <summary>
    /// Add last lines to data files and overwrite backup files.
    /// </summary>
    void EndObjectData()
    {
        // Last object positions
        data_endState += DataFile.AddLine<ObjectData>(GetObjectsInScene());

        // Last line
        data_endState = data_endState.TrimEnd('\n').TrimEnd('\r').TrimEnd(',');
        data_endState += DataFile.EndFile(false);

        data_contLog = data_contLog.TrimEnd('\n').TrimEnd('\r').TrimEnd(',');
        data_contLog += DataFile.EndFile(false);

        data_startState = data_startState.TrimEnd('\n').TrimEnd('\r').TrimEnd(',');
        data_startState += DataFile.EndFile(false);

        // Overwrite backup files
        if (uqName_end == "" || uqName_end == null)
            DataFile.Overwrite(data_endState, directoryPath, name_end);
        else
            DataFile.Overwrite(data_endState, directoryPath, uqName_end);

        if (uqName_contLog == "" || uqName_contLog == null)
            DataFile.Overwrite(data_contLog, directoryPath, name_contLog);
        else
            DataFile.Overwrite(data_contLog, directoryPath, uqName_contLog);

        if (uqName_start == "" || uqName_start == null)
            DataFile.Overwrite(data_startState, directoryPath, name_start);
        else
            DataFile.Overwrite(data_startState, directoryPath, uqName_start);
    }

    /// <summary>
    /// Generate filenames and write first line of file
    /// </summary>
    void PrepareHeadData()
    {
        // Filenames
        var currentSet = DataManager.Instance.CurrentSet;
        name_headData = "HeadData" + GameManager.Instance.GameType.ToString() + currentSet.UserData.UserID.ToString();

        directoryPath = DataFile.GenerateDirectory(directoryPath);
        name_headData = DataFile.GenerateUniqueFileName(directoryPath, name_headData);

        // Start writing
        data_headData += DataFile.StartFile();
    }

    /// <summary>
    /// Add new data to data string
    /// </summary>
    void ExecuteHeadData()
    {
        // Add data to string
        var data = GetCurrentHeadData();
        if (data != null && data.IsValid())
            data_headData += DataFile.AddLine<HeadData>(data);
    }

    /// <summary>
    /// Add last lines to data file and overwrite backup file.
    /// </summary>
    void EndHeadData()
    {
        // End logging
        data_headData = data_headData.TrimEnd('\n').TrimEnd('\r').TrimEnd(',');
        data_headData += DataFile.EndFile(false);

        // Overwrite backup files
        if (uqName_headData == "" || uqName_headData == null)
            DataFile.Overwrite(data_headData, directoryPath, name_headData);
        else
            DataFile.Overwrite(data_headData, directoryPath, uqName_headData);
    }

    #endregion  Data Functions

    #region Get Data Functions

    /// <summary>
    /// Get object data from data manager
    /// </summary>
    /// <returns></returns>
    private ObjectData GetMovingObject()
    {
        return new ObjectData(DataManager.Instance.MovingObjects, Time.time, ObjectManager.GetPositionOffset());
    }

    /// <summary>
    /// Get object data from data manager
    /// </summary>
    /// <returns></returns>
    private ObjectData GetObjectsInScene()
    {
        if (DataManager.Instance.ObjectsInScene != null)
            return new ObjectData(DataManager.Instance.ObjectsInScene, Time.time, ObjectManager.GetPositionOffset());
        else
            return new ObjectData(new System.Collections.Generic.List<GameObject>(), Time.time, ObjectManager.GetPositionOffset());
    }

    /// <summary>
    /// Get head data from data manager
    /// </summary>
    /// <returns></returns>
    private HeadData GetCurrentHeadData()
    {
        if (DataManager.Instance.CurrentHeadData.IsValid())
            return DataManager.Instance.CurrentHeadData;
        else
            throw new InvalidDataException();
    }

    #endregion Get Data Functions

}

