using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Examples.Demos;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
///  Helper class to use functions from MonoBehaviour in ObjectManager
/// </summary>
public class ObjectCreator : ScriptableObject
{
    #region Public Fields

    public string PrefabFolderName { get => prefabFolderName; set => prefabFolderName = value ?? throw new ArgumentNullException(nameof(value), "Name cannot be null"); }
    public string BoundingBoxFolderName { get => boundingBoxFolderName; set => boundingBoxFolderName = value ?? throw new ArgumentNullException(nameof(value), "Name cannot be null"); }
    public List<GameObject> InstantiatedObjects { get => instantiatedObjects; set => instantiatedObjects = value ?? throw new ArgumentNullException(nameof(value), "Name cannot be null"); }

    #endregion Public Fields

    #region Private Fields

    // Folder name
    private string boundingBoxFolderName;
    private string soundFolderName;
    private string prefabFolderName;

    // Game objects
    private List<GameObject> instantiatedObjects;

    // Audioclip
    private AudioClip rotateStart;
    private AudioClip rotateStop;
    private AudioClip manStart;
    private AudioClip manStop;

    #endregion Private Fields

    #region MonoBehaviour Functions
    /// <summary>
    /// Load parameters from resources
    /// </summary>
    public void OnEnable()
    {
        // Parameters
        instantiatedObjects = new List<GameObject>();
        prefabFolderName = "Objects";
        boundingBoxFolderName = "BoundingBox";
        soundFolderName = "Sound";

        // Audio rotation
        var rotfileName = "/MRTK_Rotate_Start";
        rotateStart = Resources.Load<AudioClip>(soundFolderName + rotfileName);
        if (rotateStart == null)
            throw new FileNotFoundException("... ObjectManager::ApplyRelevantComponents no file {0} found", rotfileName);

        var fileName = "/MRTK_Rotate_Stop";
        rotateStop = Resources.Load<AudioClip>(soundFolderName + fileName);
        if (rotateStop == null)
            throw new FileNotFoundException("... ObjectManager::ApplyRelevantComponents no file {0} found", fileName);

        // Audio manipulation
        var fileNameManipulation = "/MRTK_Manipulation_End";
        manStop = Resources.Load<AudioClip>(soundFolderName + fileNameManipulation);
        if (manStop == null)
            throw new FileNotFoundException("... ObjectManager::ApplyRelevantComponents no file {0} found", fileNameManipulation);

        var fileNameManStart = "/MRTK_Manipulation_Start";
        manStart = Resources.Load<AudioClip>(soundFolderName + fileNameManStart);
        if (manStart == null)
            throw new FileNotFoundException("... ObjectManager::ApplyRelevantComponents no file {0} found", fileNameManStart);
    }
    #endregion MonoBehaviour Functions

    #region Public Functions

    #region Create Objects

    /// <summary>
    /// Spawn one object in scene
    /// </summary>
    /// <param name="obj">Object to spawn</param>
    /// <param name="parent">Parent game object</param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="config">Movement enabled or disabled</param>
    public void SpawnObject(GameObject obj, GameObject parent, Vector3 position, Quaternion rotation, ConfigType config)
    {
        ApplyRelevantComponents(obj);
        ApplyConfiguration(obj, config);

        var generatedObject = Instantiate(obj, position, rotation);

        // Add Sounds to Movement
        generatedObject.GetComponent<BoundsControl>().RotateStarted.RemoveAllListeners();
        generatedObject.GetComponent<BoundsControl>().RotateStarted.AddListener(() => HandleOnRotationStarted(generatedObject));

        generatedObject.GetComponent<BoundsControl>().RotateStopped.RemoveAllListeners();
        generatedObject.GetComponent<BoundsControl>().RotateStopped.AddListener(() => HandleOnRotationStopped(generatedObject));

        generatedObject.GetComponent<ObjectManipulator>().OnManipulationStarted.RemoveAllListeners();
        generatedObject.GetComponent<ObjectManipulator>().OnManipulationStarted.RemoveAllListeners();

        generatedObject.GetComponent<ObjectManipulator>().OnManipulationStarted.AddListener(HandleOnManipulationStarted);
        generatedObject.GetComponent<ObjectManipulator>().OnManipulationEnded.AddListener(HandleOnManipulationStopped);

        generatedObject.SetActive(true);

        generatedObject.name = generatedObject.name.Replace("(Clone)", "");
        generatedObject.transform.position = position;
        generatedObject.transform.rotation = rotation;
        generatedObject.transform.parent = parent.transform;

        instantiatedObjects.Add(generatedObject);
    }

    /// <summary>
    /// Spawn object without offset
    /// </summary>
    /// <param name="gameObjects">Objects to spawn</param>
    /// <param name="parent">Parent game object</param>
    /// <param name="positions">Positions to spawn</param>
    /// <param name="rotations">Rotations to spawn</param>
    /// <param name="config">Movement enabled or disabled</param>
    public void SpawnObjects(GameObject[] gameObjects, GameObject parent, Vector3[] positions, Quaternion[] rotations, ConfigType config)
    {
        for (int i = 0; i < gameObjects.Length; i++)
            SpawnObject(gameObjects[i], parent, positions[i], rotations[i], config);
    }

    /// <summary>
    /// Spawn objects with offset
    /// </summary>
    /// <param name="gameObjects">Objects to spawn</param>
    /// <param name="parent">Parent game object</param>
    /// <param name="positions">Positions to spawn</param>
    /// <param name="rotations">Rotations</param>
    /// <param name="config">Movement enabled or disabled</param>
    /// <param name="offset">Offset of parent position</param>
    public void SpawnObjects(GameObject[] gameObjects, GameObject parent, Vector3[] positions, Quaternion[] rotations, ConfigType config, Vector3 offset)
    {
        for (int i = 0; i < gameObjects.Length; i++)
            SpawnObject(gameObjects[i], parent, positions[i] - offset, rotations[i], config);
    }

    /// <summary>
    /// Return first object of current Data to test object
    /// </summary>
    /// <param name="currentData"></param>
    /// <returns></returns>
    public GameObject CreateInteractionObject(ObjectData currentData)
    {
        // Set first object in list to test object
        GameObject loadedObj = Resources.Load<GameObject>(prefabFolderName + "/" + currentData.GameObjects[0].Objectname.ToString());

        if (loadedObj == null)
            throw new FileNotFoundException("... ObjectManager::CreateInteractionObject no file found");

        return loadedObj;
    }

    /// <summary>
    /// Load objects from resources
    /// </summary>
    /// <param name="currentData"></param>
    /// <returns></returns>
    public GameObject[] CreateInteractionObjects(ObjectData currentData)
    {
        int length = currentData.GameObjects.Count;
        GameObject[] objs = new GameObject[length];

        // Load objects from resources
        for (int i = 0; i < length; i++)
        {
            var loadedObj = Resources.Load<GameObject>(prefabFolderName + "/" + currentData.GameObjects[i].Objectname.ToString());
            if (loadedObj == null)
                throw new FileNotFoundException("... ObjectManager::CreateInteractionObjects Object " + currentData.GameObjects[i].Objectname.ToString() + " not found");
            else
                objs[i] = loadedObj;
        }

        return objs;
    }
    #endregion Create Objects

    #region Remove Objects
    public void RemoveAllObjects()
    {
        foreach (GameObject obj in instantiatedObjects)
        {
            Destroy(obj);
        }
        instantiatedObjects.Clear();
    }

    /// <summary>
    /// Remove objects
    /// </summary>
    public void Reset()
    {
        if (instantiatedObjects != null)
            RemoveAllObjects();
    }

    #endregion Remove Objects

    #endregion Public Functions

    #region Private Funcions

    #region Components
    /// <summary>
    /// Enable or disable movement of object
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="config"></param>
    private void ApplyConfiguration(GameObject obj, ConfigType config)
    {
        if (config == ConfigType.MovementDisabled)
        {
            try
            {
                if (obj.TryGetComponent(out BoundsControl bC))
                    bC.enabled = false;
                if (obj.TryGetComponent(out ObjectManipulator oM))
                    oM.enabled = false;
                if (obj.TryGetComponent(out NearInteractionGrabbable iG))
                    iG.enabled = false;
                if (obj.TryGetComponent(out Rigidbody rb))
                {
                    // Allow gravity to let the object fall on the table and adjust position
                    rb.useGravity = true;
                    rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                }
            }
            catch (InvalidCastException e)
            {
                throw new System.MemberAccessException("ObjectCreator:: ApplyConfiguration, not all Components found.", e);
            }
        }
        else if (config == ConfigType.MovementEnabled)
        {
            try
            {
                var comp = (BoundsControl)obj.GetComponent(typeof(BoundsControl));
                comp.enabled = true;
                comp.BoundsControlActivation = Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes.BoundsControlActivationType.ActivateByProximity;

                var oM = (ObjectManipulator)obj.GetComponent(typeof(ObjectManipulator));
                oM.enabled = true;

                var iG = (NearInteractionGrabbable)obj.GetComponent(typeof(NearInteractionGrabbable));
                iG.enabled = true;

                if (obj.TryGetComponent(out Rigidbody rb))
                {
                    rb.useGravity = true;
                    rb.constraints = RigidbodyConstraints.FreezeRotation;
                }

            }
            catch (InvalidCastException e)
            {
                throw new System.MemberAccessException("ObjectCreator:: ApplyConfiguration, not all Components found.", e);
            }

        }
        else
        {
            Debug.LogError("ObjectCreator::ApplyConfiguration wrong configType format.");
        }
    }

    /// <summary>
    /// Add components to object.
    /// </summary>
    /// <param name="loadedObj"></param>
    private void ApplyRelevantComponents(GameObject loadedObj)
    {
        loadedObj.tag = "InteractionObject";

        // Rigidbody
        var rb = loadedObj.EnsureComponent<Rigidbody>();
        rb.mass = 1;
        rb.drag = 0;
        rb.angularDrag = 0;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.freezeRotation = true;

        // BoxCollider
        var col = loadedObj.EnsureComponent<BoxCollider>();

        // Audio Source
        var audio = loadedObj.EnsureComponent<AudioSource>();

        // Tethered Placement
        var placementComp = loadedObj.EnsureComponent<TetheredPlacement>();
        placementComp.DistanceThreshold = 20.0f;

        // Near Interaction Grabbable
        var grabComp = loadedObj.EnsureComponent<NearInteractionGrabbable>();
        grabComp.ShowTetherWhenManipulating = false;
        grabComp.IsBoundsHandles = true;

        // ConstraintManager
        var constMan = loadedObj.EnsureComponent<ConstraintManager>();

        // RotationAxisConstraint
        var rotConst = loadedObj.EnsureComponent<RotationAxisConstraint>();
        rotConst.HandType = ManipulationHandFlags.OneHanded | ManipulationHandFlags.TwoHanded; ;
        rotConst.ConstraintOnRotation = AxisFlags.XAxis | AxisFlags.ZAxis;
        rotConst.UseLocalSpaceForConstraint = true;
        constMan.AddConstraintToManualSelection(rotConst);

        // Min Max Scale Constraint
        var scaleConst = loadedObj.EnsureComponent<MinMaxScaleConstraint>();
        scaleConst.HandType = ManipulationHandFlags.TwoHanded | ManipulationHandFlags.OneHanded; ;
        scaleConst.ProximityType = ManipulationProximityFlags.Far | ManipulationProximityFlags.Near;
        scaleConst.ScaleMaximum = 1;
        scaleConst.ScaleMinimum = 1;
        scaleConst.RelativeToInitialState = true;

        constMan.AddConstraintToManualSelection(scaleConst);

        // Custom Movement Constraint
        var moveConst = loadedObj.EnsureComponent<CustomMovementConstraint>();
        moveConst.HandType = ManipulationHandFlags.OneHanded | ManipulationHandFlags.TwoHanded;
        moveConst.ConstraintOnMovement = AxisFlags.YAxis;
        constMan.AddConstraintToManualSelection(moveConst);

        // Object Manipulator
        var objMan = loadedObj.EnsureComponent<ObjectManipulator>();
        objMan.AllowFarManipulation = false;
        objMan.EnableConstraints = true;
        objMan.ConstraintsManager = constMan;

        // BoundsControl
        var boundsControl = loadedObj.EnsureComponent<BoundsControl>();
        boundsControl.Target = loadedObj;
        boundsControl.BoundsControlActivation = Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes.BoundsControlActivationType.ActivateByProximity;
        boundsControl.BoundsOverride = col;
        boundsControl.CalculationMethod = Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes.BoundsCalculationMethod.RendererOverCollider;

        // DisplayConfig
        BoxDisplayConfiguration dispConfig = CreateInstance<BoxDisplayConfiguration>();
        dispConfig.BoxMaterial = GameManager.Instance.BoundingBox;
        dispConfig.BoxGrabbedMaterial = GameManager.Instance.BoundingBoxGrabbed;
        boundsControl.BoxDisplayConfig = dispConfig;

        // Scale Handle
        ScaleHandlesConfiguration config = CreateInstance<ScaleHandlesConfiguration>();
        config.ShowScaleHandles = false;
        boundsControl.ScaleHandlesConfig = config;

        // Translation Handle
        TranslationHandlesConfiguration tConfig = CreateInstance<TranslationHandlesConfiguration>();
        tConfig.ShowHandleForX = false;
        tConfig.ShowHandleForY = false;
        tConfig.ShowHandleForZ = false;
        boundsControl.TranslationHandlesConfig = tConfig;

        // Rotation Handle
        var rotationHandle = CreateInstance<RotationHandlesConfiguration>();
        rotationHandle.HandleMaterial = GameManager.Instance.BoundingBoxHandleWhite;
        rotationHandle.HandleGrabbedMaterial = GameManager.Instance.BoundingBoxHandleBlueGrabbed;
        rotationHandle.HandlePrefab = GameManager.Instance.BoundingBox_RotateHandle;
        boundsControl.RotationHandlesConfig = rotationHandle;

        boundsControl.RotationHandlesConfig = rotationHandle;
        boundsControl.RotationHandlesConfig.ShowHandleForX = false;
        boundsControl.RotationHandlesConfig.ShowHandleForY = true;
        boundsControl.RotationHandlesConfig.ShowHandleForZ = false;

        // Links Config
        var linksConfig = CreateInstance<LinksConfiguration>();
        linksConfig.ShowWireFrame = false;
        boundsControl.LinksConfig = linksConfig;

        boundsControl.ConstraintsManager = constMan;
    }

    #endregion Components

    #region Manipulation And Rotation

    /// <summary>
    /// Add rotated object to list and removes rigidbody for performance purpose.
    /// </summary>
    /// <param name="eventData"></param>
    private void HandleOnManipulationStarted(ManipulationEventData eventData)
    {
        Debug.Log("Manipulation Started");

        // Play audio
        eventData.ManipulationSource.GetComponent<AudioSource>().PlayOneShot(manStart);

        // Destroy rigidbody
        Destroy(eventData.ManipulationSource.GetComponent<Rigidbody>());

        // Remove object from list
        DataManager.Instance.MovingObjects.Add(eventData.ManipulationSource);
    }

    /// <summary>
    /// Remove object from list and add rigidbody
    /// </summary>
    /// <param name="eventData"></param>
    private void HandleOnManipulationStopped(ManipulationEventData eventData)
    {
        Debug.Log("Manipulation Stopped");

        // Add rigidbody 
        Rigidbody rb = eventData.ManipulationSource.AddComponent<Rigidbody>();
        rb.mass = 1;
        rb.drag = 0;
        rb.angularDrag = 0;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.freezeRotation = true;

        // Play audio
        eventData.ManipulationSource.GetComponent<AudioSource>().PlayOneShot(manStop);

        // Remove object from list
        DataManager.Instance.MovingObjects.Remove(eventData.ManipulationSource);
    }

    /// <summary>
    /// Add rotated object to list and removes rigidbody for performance purpose
    /// </summary>
    /// <param name="generatedObject">Manipulated object</param>
    private void HandleOnRotationStarted(GameObject generatedObject)
    {
        Debug.Log("Rotation Started");

        // Play audio
        generatedObject.GetComponent<BoundsControl>().GetComponent<AudioSource>().PlayOneShot(rotateStart);

        // Destroy rigidbody
        Destroy(generatedObject.GetComponent<Rigidbody>());

        // Add object to list
        DataManager.Instance.MovingObjects.Add(generatedObject);
    }

    /// <summary>
    /// Removes object from list and reenables rigidbody.
    /// </summary>
    /// <param name="generatedObject">Manipulated object</param>
    private void HandleOnRotationStopped(GameObject generatedObject)
    {
        Debug.Log("Rotation Stopped");

        // Play audio
        generatedObject.GetComponent<BoundsControl>().GetComponent<AudioSource>().PlayOneShot(rotateStop);

        // Add rigidbody
        Rigidbody rb = generatedObject.AddComponent<Rigidbody>();
        rb.mass = 1;
        rb.drag = 0;
        rb.angularDrag = 0;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.freezeRotation = true;

        // Remove object
        DataManager.Instance.MovingObjects.Remove(generatedObject);
    }

    #endregion Manipulation And Rotation

    #endregion Private Functions

}
