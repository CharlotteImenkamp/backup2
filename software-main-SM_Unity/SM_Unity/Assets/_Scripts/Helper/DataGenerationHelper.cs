using UnityEngine;

// TODO delete if not needed

public class DataGenerationHelper : MonoBehaviour
{
    [SerializeField]
    private bool saveNewSettings;
    // Start is called before the first frame update
    void Start()
    {
        if (saveNewSettings)
        {
            ApplicationData data = new ApplicationData();

            data = DataFile.Load<ApplicationData>("C:\\Users\\Student\\AppData\\LocalLow\\DefaultCompany\\AR_ProjV63\\DataFiles\\generalSettingsTest16150");



        }
    }
    //private void StartFile(BinaryFormatter bf, string filepath)
    //{
    //    string start = "{ \n \"entries\":[" + Environment.NewLine;
    //    File.AppendAllText(filepath, start);
    //}

    //private void AddLine(BinaryFormatter bf, ObjectData data, string filepath)
    //{
    //    string jsonString = JsonUtility.ToJson(data, true);

    //    jsonString += "," + System.Environment.NewLine;
    //    File.AppendAllText(filepath, jsonString);

    //}

    //public void DefaultUserSettings()
    //{
    //    var set = new userSettingsData();
    //    set.updateRate = 1.0f;
    //    set.UserID = 2;

    //    var t = new CustomObject("CoffeeCupGravity", new Vector3(-0.162803515791893f, -0.14000004529953004f, 1.7623728513717652f), new Quaternion(8.940696716308594e-8f, 0.005584994331002235f, -4.656612873077393e-10f, 0.9999845027923584f));

    //    var f = new CustomObject("Pear_MPI01_fbx", new Vector3(0f, 0.005f, 1.759580373764038f), new Quaternion(8.940696716308594e-8f, 0.005584994331002235f, -4.656612873077393e-10f, 0.9999845027923584f));
    //    set.gameObjects = new List<CustomObject> { t, f };

    //    DataManager.Instance.CurrentSettings = set;
    //}


    //private void EndFile(BinaryFormatter bf, string filepath)
    //{
    //    string ende = " \"END\"\n ]}\n";
    //    File.AppendAllText(filepath, ende);
    //}

    //userSettingsData SaveCurrentLayout()
    //{
    //    GameObject[] intObj = GameObject.FindGameObjectsWithTag("InteractionObject");
    //    List<CustomObject> obj = new List<CustomObject>(); 


    //    foreach (GameObject o in intObj)
    //    {
    //        obj.Add(new CustomObject(o.name, o.transform.position, o.transform.rotation)); 
    //    }

    //    userSettingsData res = new userSettingsData(
    //            obj,
    //            0.1f,
    //            1, 
    //            userSettingsData.userSet.AG,
    //            userSettingsData.gameState.pricesCompleted
    //            );

    //    return res; 
    //}

    //userSettingsData GenerateTestSet()
    //{
    //    userSettingsData test = new userSettingsData(
    //new List<CustomObject> {
    //            new CustomObject("eins", Vector3.zero, Quaternion.identity),
    //            new CustomObject("zwei", Vector3.zero, Quaternion.identity)},
    //0f, 1, userSettingsData.userSet.AG, userSettingsData.gameState.None);

    //    test.gameObjects = new List<CustomObject>();
    //    test.gameObjects.Add(new CustomObject("eins", Vector3.zero, Quaternion.identity));
    //    test.gameObjects.Add(new CustomObject("zwei", Vector3.zero, Quaternion.identity));
    //    test.updateRate = 0f;
    //    test.UserID = 1;
    //    test.set = userSettingsData.userSet.AG;

    //    return test; 
    //} 
    //private void SaveNewUserSettings(userSettingsData data, string foldername, string filename, string fileending)
    //{ 
    //    string persistentPath = Application.persistentDataPath; 

    //    string filepath = Path.Combine(persistentPath, filename + fileending);

    //    Debug.Log(filepath); 
    //    // save data in json 
    //    if (data != null)
    //    {
    //        if (File.Exists(filepath))
    //        {
    //            filename = filename + System.DateTime.Now; 
    //            filepath = persistentPath + foldername + filename + fileending;
    //        }
    //        string jsonString = JsonUtility.ToJson(data, true);
    //    jsonString += System.Environment.NewLine;
    //    File.AppendAllText(filepath, jsonString);

    //    File.WriteAllText(filepath, jsonString);
    //        Debug.Log(filepath); 
    //    }
    //    else
    //    {
    //        Debug.LogError("LoadSettings::SaveNewUserSettings no data to save");
    //    }
    //    //AddNewSettingsFileToGeneralSettings(filename, fileending); 
    //}

    ///// <summary>
    ///// Save local Filepath of settings file
    ///// </summary>
    ///// <param name="filePath">local path</param>
    ///// <param name="gameType"> </param>
    //void AddNewSettingsFileToGeneralSettings(string filePath, GameType gameType)        //\TODO USE!
    //{
    //    switch (gameType)
    //    {
    //        case GameType.None:
    //            GameManager.Instance.generalSettings.newSets.Add(filePath);
    //            break;
    //        case GameType.Prices:
    //            GameManager.Instance.generalSettings.incompleteUserData.Add(filePath); 
    //            break;
    //        case GameType.Locations:
    //            GameManager.Instance.generalSettings.completeUserData.Add(filePath); 
    //            break;
    //        default:
    //            throw new ArgumentOutOfRangeException("...DataGenerationHelper::AddNEwSettingsFileToGeneral Settings invalid gameType.");         
    //    }

    //    DataFile.Save<ApplicationData>(GameManager.Instance.generalSettings, GameManager.Instance.mainFolder, "generalSettings"); 
    //}
}
