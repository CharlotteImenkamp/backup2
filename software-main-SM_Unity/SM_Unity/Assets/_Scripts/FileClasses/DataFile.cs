using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Class holds methods to use for several data types.
/// </summary>
public class DataFile
{
    #region Private Fields

    private static string fileending = ".json";

    #endregion

    #region Load

    /// <summary>
    /// Try from persistent datapath first, then try from resources or generate a default set
    /// </summary>
    /// <param name="filepath">local path with filename without ending</param>
    /// <returns></returns>
    public static T SecureLoad<T>(string filepath) where T : new()
    {
        string jsonString;

        T newData = new T();

        // Load from persistent datapath
        string path = Path.Combine(Application.persistentDataPath, filepath);

        if (File.Exists(path + fileending))
        {
            jsonString = File.ReadAllText(path + fileending);
            JsonFile<T> file = JsonUtility.FromJson<JsonFile<T>>(jsonString);
            newData = file.entries[0];
        }
        else
        {
            // Else load from resources
            var textFile = Resources.Load<TextAsset>(filepath);
            if (textFile != null)
            {
                JsonFile<T> file = JsonUtility.FromJson<JsonFile<T>>(textFile.text);
                newData = file.entries[0];

                // Save in persistent datapath for next time
                Save(newData, Path.GetDirectoryName(filepath), Path.GetFileName(filepath));
            }
            else
            {
                GameManager.Instance.DebugText.text = "data not found at path " + filepath;
                Debug.LogError("data not found at path " + filepath);
            }
        }
        return newData;
    }

    /// <summary>
    /// Loads file from persistent datapath
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="completePath">Local path without ending</param>
    /// <returns></returns>
    public static T Load<T>(string completePath) where T : new()
    {
        string jsonString;

        // Load from persistent datapath
        string path = Path.Combine(Application.persistentDataPath, completePath);

        if (File.Exists(path + fileending))
        {
            jsonString = File.ReadAllText(path + fileending);
            JsonFile<T> file = JsonUtility.FromJson<JsonFile<T>>(jsonString);
            T newData = file.entries[0];

            // debug
            GameManager.Instance.DebugText.text = "data loaded from persistent Path.";
            Debug.Log("data loaded from persistent Path.");

            return newData;
        }
        else
        {
            throw new Exception("... path " + path + " not found");
        }
    }

    /// <summary>
    /// Helper Function to load user sets into game.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static List<DataManager.Data> LoadUserSets(List<string> path)
    {
        List<DataManager.Data> newData = new List<DataManager.Data>();

        // Get parameters from ´game manager
        int N = path.Count;

        // Filepath
        string mainFolder = GameManager.Instance.MainFolder;
        string userDataFolder = GameManager.Instance.GeneralSettings.UserDataFolder;

        // Load each file into own parameter and save in data manager
        for (int i = 0; i < N; i++)
        {
            var filePath = Path.Combine(mainFolder, userDataFolder, path[i]);
            var userData = DataFile.SecureLoad<UserSettingsData>(filePath);

            var objPath = Path.Combine(mainFolder, userDataFolder, "User" + userData.UserID.ToString(), GameManager.Instance.StartDataName + userData.UserID.ToString());
            var objData = DataFile.SecureLoad<ObjectData>(objPath);

            newData.Add(new DataManager.Data(objData, userData));
        }
        return newData;
    }

    #endregion Load 

    #region Save

    /// <summary>
    /// Save into persistent data path. Generates new name if it exists.
    /// Begin and end of the string are added automatically.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="folderAfterPersistentPath"></param>
    /// <param name="name"></param>
    public static string Save<T>(T data, string folderAfterPersistentPath, string name)
    {
        // Check arguments
        if (string.IsNullOrEmpty(folderAfterPersistentPath))
            throw new ArgumentException(" Argument kann nicht NULL oder leer sein.", nameof(folderAfterPersistentPath));

        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Argument kann nicht NULL oder leer sein.", nameof(name));

        // Make complete directory
        string directory = GenerateDirectory(Path.Combine(Application.persistentDataPath, folderAfterPersistentPath));
        string fileName = GenerateUniqueFileName(directory, name);

        string filePath = Path.Combine(directory, fileName);
        string jsonString;

        if (typeof(T) == typeof(ApplicationData))
            jsonString = StartSettingsFile();
        else
            jsonString = StartFile();

        jsonString += AddLine<T>(data);
        jsonString = jsonString.TrimEnd('\n').TrimEnd('\r').TrimEnd(',');
        jsonString += EndFile(false);

        // Overwrite existing text
        UnityEngine.Windows.File.WriteAllBytes(filePath + fileending, Encoding.ASCII.GetBytes(jsonString));

        // Debug
        GameManager.Instance.DebugText.text = "Data saved into persistent Path.";
        Debug.Log("Data saved into persistent Path: " + filePath);

        return fileName;
    }

    /// <summary>
    /// Save into persistent data path. Generates new name if it exists
    /// </summary>
    /// <param name="data"></param>
    /// <param name="folderAfterPersistentPath"></param>
    /// <param name="name"></param>
    public static void OverwriteData<T>(T data, string folderAfterPersistentPath, string name)
    {
        // Prepare file path 
        string directory = Path.Combine(Application.persistentDataPath, folderAfterPersistentPath);
        string filePath = Path.Combine(directory, name);

        // Prepare file content
        string jsonString;
        if (typeof(T) == typeof(ApplicationData))
            jsonString = StartSettingsFile();
        else
            jsonString = StartFile();
        jsonString += AddLine<T>(data);
        jsonString = jsonString.TrimEnd('\n').TrimEnd('\r').TrimEnd(',');
        jsonString += EndFile(false);

        // Overwrite existing text
        UnityEngine.Windows.File.WriteAllBytes(filePath + fileending, Encoding.ASCII.GetBytes(jsonString));

        // Debug
        GameManager.Instance.DebugText.text = "Data overritten in " + filePath;
        Debug.Log("Data overritten in " + filePath);
    }

    /// <summary>
    /// Save complete json string. Begin and end of the string must be added beforehand.
    /// Generates new filename if exists and saves file.
    /// </summary>
    /// <param name="jsonString"></param>
    /// <param name="folderName"></param>
    /// <param name="name">without fileending</param>
    public static string Save(string jsonString, string folderName, string name)
    {
        string directory = GenerateDirectory(Path.Combine(Application.persistentDataPath, folderName));
        string fileName = GenerateUniqueFileName(directory, name);

        string filePath = Path.Combine(directory, fileName);

        // Override existing text
        UnityEngine.Windows.File.WriteAllBytes(filePath + fileending, Encoding.ASCII.GetBytes(jsonString));

        // Debug
        GameManager.Instance.DebugText.text = "Data saved into persistent Path.";
        Debug.Log("Data saved into " + filePath);

        return fileName;
    }

    /// <summary>
    ///  Save into persistent data path.
    /// </summary>
    /// <param name="jsonString"></param>
    /// <param name="folderName"></param>
    /// <param name="name"></param>
    public static void Overwrite(string jsonString, string folderName, string name)
    {
        // Prepare file path
        string directory = GenerateDirectory(Path.Combine(Application.persistentDataPath, folderName));
        string filePath = Path.Combine(directory, name);

        // Override existing text
        UnityEngine.Windows.File.WriteAllBytes(filePath + fileending, Encoding.ASCII.GetBytes(jsonString));

        // Debug
        GameManager.Instance.DebugText.text = "Data saved into persistent Path.";
        Debug.Log("Data overwritten in " + filePath);
    }

    #endregion Save

    #region Helper Functions

    /// <summary>
    /// Add equal beginning to every data file
    /// </summary>
    /// <returns></returns>
    public static string StartFile()
    {
        string id;
        try
        {
            id = DataManager.Instance.CurrentSet.UserData.UserID.ToString();
        }
        catch
        {
            id = "";
        }

        return "{\n \"start\": \"" + "User: " + id
             + " ," + DateTime.Now.ToString("F") + " \", "
            + Environment.NewLine
            + "\"entries\": \n[ \n ";
    }

    /// <summary>
    /// Add beginning to settings file
    /// </summary>
    /// <returns></returns>
    public static string StartSettingsFile()
    {
        return "{\n \"start\": \"" + DateTime.Now.ToString("F") + " \", "
            + Environment.NewLine
            + "\"entries\": \n[ \n ";
    }

    /// <summary>
    /// Convert data class into json string and add new line
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string AddLine<T>(T data)
    {
        string jsonString = JsonUtility.ToJson(data, true);
        jsonString += "," + System.Environment.NewLine;

        return jsonString;
    }

    /// <summary>
    /// Add equal end to every data file.
    /// </summary>
    /// <param name="backup"> If the end is of a backupfile, it is marked in the line.</param>
    /// <returns></returns>
    public static string EndFile(bool backup)
    {
        if (backup)
            return " \n], \n\"ende\" : \"BACKUP\"\n }";
        else
            return " \n], \n\"ende\" : \"END\"\n }";
    }

    /// <summary>
    /// Returns directorypath
    /// </summary>
    /// <param name="directroyPath"></param>
    /// <returns></returns>
    public static string GenerateDirectory(string directroyPath)
    {
        // Generate Directory
        if (!Directory.Exists(directroyPath))
            Directory.CreateDirectory(directroyPath);

        return directroyPath;
    }

    /// <summary>
    /// Return new filename
    /// </summary>
    /// <param name="directoryPath"></param>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static string GenerateUniqueFileName(string directoryPath, string filename)
    {
        string filePath = Path.Combine(directoryPath, filename + fileending);

        // File
        if (File.Exists(filePath))
        {
            // Unique name
            filename = filename + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
            filePath = Path.Combine(directoryPath, filename);
        }

        return filename;
    }

    #endregion Helper Functions 
}

/// <summary>
/// Helper class used in DataFile, to maintain a file structure 
/// </summary>
/// <typeparam name="T"></typeparam>
public class JsonFile<T>
{
    public string start;
    public T[] entries;
    public string ende;
}