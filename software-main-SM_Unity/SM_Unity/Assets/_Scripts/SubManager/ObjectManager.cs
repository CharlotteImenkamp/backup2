using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

/// <summary>
/// Submanager, which manages the spawning and removing of objects in scene. Highly 
/// connected with ObjectCreator class
/// </summary>
public class ObjectManager : SubManager
{
    #region Private Fields

    private GameObject[] interactionObjects;

    // Game objects
    private GameObject parentPlayTable;
    private GameObject parentSideTable;

    // Grid object collections
    private GridObjectCollection playTableObjectCollection;
    private GridObjectCollection sideTableObjectCollection;

    // Test scenarien
    private GameObject testObject;
    private Vector3 testPositionPrices;
    private Vector3 testPositionLocations;

    private DataManager.Data currentData;
    private ObjectCreator objectCreator;

    #endregion Private Fields

    #region SubManager Functions
    /// <summary>
    /// Initialization of parameters
    /// </summary>
    public void Initialize()
    {
        objectCreator = ScriptableObject.CreateInstance<ObjectCreator>();

        // Game objects
        parentPlayTable = GameManager.Instance.ParentPlayTable;
        parentSideTable = GameManager.Instance.ParentSideTable;

        // Object collections play table
        playTableObjectCollection = parentPlayTable.GetComponent<GridObjectCollection>();
        if (playTableObjectCollection == null)
        {
            playTableObjectCollection.SurfaceType = ObjectOrientationSurfaceType.Plane;
            playTableObjectCollection.CellHeight = 0.25f;
            playTableObjectCollection.CellWidth = 0.25f;
        }

        // Object collection side table
        sideTableObjectCollection = parentSideTable.GetComponent<GridObjectCollection>();
        if (sideTableObjectCollection == null)
        {
            sideTableObjectCollection.SurfaceType = ObjectOrientationSurfaceType.Plane;
            sideTableObjectCollection.CellHeight = 0.19f;
            sideTableObjectCollection.CellWidth = 0.19f;
        }

        // folder 
        objectCreator.PrefabFolderName = "Objects";
    }

    /// <summary>
    /// Reset parameters
    /// </summary>
    public override void Reset()
    {
        GameManager.Instance.DebugText.text = "ObjectManager::Reset";
        Debug.Log("ObjectManager::Reset");

        interactionObjects = null;
        testObject = null;
        currentData.Clear();
        objectCreator.Reset();
    }

    #region Gamestates

    /// <summary>
    /// Check, if any task needs to be done at new game state
    /// </summary>
    /// <param name="newState">State which is entered.</param>
    public override void OnGameStateEntered(string newState)
    {
        switch (newState)
        {
            case "Initialization":
                Initialize();
                break;

            case "SettingsMenu":
                break;

            case "LocationTest":
                // Spawn test object at side table
                CheckDefaultParameters();
                objectCreator.SpawnObject(testObject, parentSideTable, testPositionLocations, testObject.transform.rotation, ConfigType.MovementEnabled);
                DataManager.Instance.ObjectsInScene = objectCreator.InstantiatedObjects;
                break;

            case "LocationEstimation":
                // spawn objects at side table
                objectCreator.SpawnObjects(interactionObjects,
                    parentSideTable,
                    currentData.ObjData.GetObjectPositions(),
                    currentData.ObjData.GetObjectRotations(),
                    ConfigType.MovementEnabled);
                sideTableObjectCollection.UpdateCollection();
                DataManager.Instance.ObjectsInScene = objectCreator.InstantiatedObjects;
                break;

            case "PriceTest":
                // Spawn test object at main table
                CheckDefaultParameters();
                objectCreator.SpawnObject(testObject, parentPlayTable, testPositionPrices, testObject.transform.rotation, ConfigType.MovementDisabled);
                DataManager.Instance.ObjectsInScene = objectCreator.InstantiatedObjects;
                break;

            case "PriceEstimation":
                // Spawn objects at main table
                objectCreator.SpawnObjects(interactionObjects, parentPlayTable,
                    currentData.ObjData.GetObjectPositions(),
                    currentData.ObjData.GetObjectRotations(),
                    ConfigType.MovementDisabled,
                    GetPositionOffset()
                    );
                DataManager.Instance.ObjectsInScene = objectCreator.InstantiatedObjects;
                break;

            case "Pause":
                break;

            case "End":
                break;

            default:
                Debug.LogError("ObjectManager::OnGameStateEntered invalid State.");
                break;
        }
    }

    /// <summary>
    /// Check, if any task needs to be done before next game state
    /// </summary>
    /// <param name="oldState">State which is left.</param>
    public override void OnGameStateLeft(string oldState)
    {
        if (oldState == "LocationTest" || oldState == "LocationEstimation" || oldState == "PriceTest" || oldState == "PriceEstimation" || oldState == "Pause" || oldState == "End")
            objectCreator.RemoveAllObjects();
    }

    #endregion gameStates

    #endregion SubManager Functions

    #region Helper Functions

    /// <summary>
    /// Get offset of table, if it is moved 
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetPositionOffset()
    {
        return GameManager.Instance.InteractionObjectsInitialPosition - GameManager.Instance.InteractionObjects.transform.position;
    }

    /// <summary>
    /// Instantiate parameters if necessary
    /// </summary>
    private void CheckDefaultParameters()
    {
        currentData = DataManager.Instance.CurrentSet;

        if (interactionObjects == null)
        {
            if (currentData.ObjData != null)
                interactionObjects = objectCreator.CreateInteractionObjects(currentData.ObjData);
            else
                Debug.LogError("Choose Object Data!");
        }

        if (testObject == null)
        {
            testObject = objectCreator.CreateInteractionObject(currentData.ObjData);
            testPositionPrices = GameManager.Instance.SpawnPointGame.position;
            testPositionLocations = GameManager.Instance.SpawnPointSide.position;
        }
    }

    #endregion Helper Funcions
}





