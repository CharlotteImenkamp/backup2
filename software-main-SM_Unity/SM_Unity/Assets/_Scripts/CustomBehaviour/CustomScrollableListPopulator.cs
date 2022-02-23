using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Populate scrollable list
/// </summary>
public class CustomScrollableListPopulator : MonoBehaviour
{
    #region Private Fields

    // Objects to duplicate in scroll collection
    private GameObject[] dynamicItems;
    private int numItems;
    private ObjectCreator objectCreator;
    private GridObjectCollection gridObjectCollection;
    private ClippingBox clippingBox;
    private List<GameObject> instantatedObjects;

    #region Serialized Fields

    [SerializeField]
    [Tooltip("The ScrollingObjectCollection to populate, if left empty, the populator will create one")]
    private ScrollingObjectCollection scrollView;

    [SerializeField]
    [Tooltip("Parent Object")]
    private GameObject buttonObject;

    [SerializeField]
    [Tooltip("Text object to display user choice. If left empty, no text is displayed.")]
    private TextMeshPro text;

    [SerializeField]
    [Tooltip("Demonstrate lazy loading")]
    private bool lazyLoad;

    [SerializeField]
    [Tooltip("Indeterminate loader to hide / show for LazyLoad")]
    private GameObject loader;

    [SerializeField]
    private float cellWidth = 0.04f;

    [SerializeField]
    private float cellHeight = 0.4f;

    [SerializeField]
    private float cellDepth = 0.04f;

    [SerializeField]
    private int cellsPerTier = 3;

    [SerializeField]
    private int tiersPerPage = 5;

    [SerializeField]
    [Tooltip("Parent gameObject to set position of scrolling object collection. ")]
    private Transform scrollPositionRef = null;

    #endregion Serialized Fields 

    #endregion Private Fields

    #region MonoBehaviour Functions

    private void OnEnable()
    {
        // Make sure we find a collection
        if (scrollView == null)
            scrollView = GetComponentInChildren<ScrollingObjectCollection>();
    }

    #endregion MonoBehaviour Functions

    #region Public Functions

    /// <summary>
    /// Called in editor to generate a scrolling list
    /// </summary>
    /// <param name="listType">Determine type of list. Choose between "incompleteSet", "completeSet", "newSet" and "Objects".</param>
    public void MakeScrollingList(string listType)
    {
        ClearList();
        if (DataManager.Instance != null)
        {
            if (listType == "incompleteSet")
            {
                numItems = DataManager.Instance.IncompleteUserData.Count;
                StartCoroutine(UpdateList("Sets", loader, DataManager.Instance.IncompleteUserData));
            }
            else if (listType == "completeSet")
            {
                numItems = DataManager.Instance.CompleteUserData.Count;
                StartCoroutine(UpdateList("Sets", loader, DataManager.Instance.CompleteUserData));
            }
            else if (listType == "newSet")
            {
                numItems = DataManager.Instance.NewUserData.Count;
                StartCoroutine(UpdateList("Sets", loader, DataManager.Instance.NewUserData));
            }
            else if (listType == "Objects")
            {
                dynamicItems = Resources.LoadAll<GameObject>("Objects/");
                numItems = dynamicItems.Length;
                this.gameObject.SetActive(true);
                StartCoroutine(UpdateList("Objects", loader));
            }
            else
                throw new System.Exception("CustomToggleListPopulator: incorrect input");
        }
        else
            Debug.LogWarning("CustomScrollableListPopulator tried to get Datamananger instance, which was null.");
    }

    /// <summary>
    /// Called in UserInputHelper to combine user data and object data
    /// </summary>
    /// <returns></returns>
    public ObjectData GetInstantiatedObjects()
    {
        ObjectData newData = new ObjectData(objectCreator.InstantiatedObjects, Time.time, ObjectManager.GetPositionOffset());
        objectCreator.RemoveAllObjects();

        if (newData.IsValid())
            return newData;
        else
            throw new ArgumentNullException();
    }

    /// <summary>
    /// Smoothly moves the scroll container a relative number of tiers of cells.
    /// Attached to buttons next to scroll view
    /// </summary>
    public void ScrollByTier(int amount)
    {
        scrollView.MoveByTiers(amount);
    }

    /// <summary>
    /// Clears List Properties to start with a new list each time, the list is reactivated
    /// </summary>
    public void ClearList()
    {
        // Object Creator
        if (objectCreator == null)
        {
            objectCreator = ScriptableObject.CreateInstance<ObjectCreator>();
            objectCreator.PrefabFolderName = "Objects";
        }
        else
        {
            objectCreator.Reset();
            objectCreator = null;
            objectCreator = ScriptableObject.CreateInstance<ObjectCreator>();
        }

        // Instantiated Button Objects
        if (instantatedObjects == null)
            instantatedObjects = new List<GameObject>();
        else
        {
            foreach (GameObject item in instantatedObjects)
                Destroy(item);
        }

        // Update GridObjectCollection
        gridObjectCollection = scrollView.GetComponentInChildren<GridObjectCollection>();
        if (gridObjectCollection == null)
        {
            GameObject collectionGameObject = new GameObject("Grid Object Collection");
            collectionGameObject.transform.position = scrollView.transform.position;
            collectionGameObject.transform.rotation = scrollView.transform.rotation;

            gridObjectCollection = collectionGameObject.AddComponent<GridObjectCollection>();
            gridObjectCollection.CellWidth = cellWidth;
            gridObjectCollection.CellHeight = cellHeight;
            gridObjectCollection.SurfaceType = ObjectOrientationSurfaceType.Plane;
            gridObjectCollection.Layout = LayoutOrder.ColumnThenRow;
            gridObjectCollection.Columns = cellsPerTier;
            gridObjectCollection.Anchor = LayoutAnchor.UpperLeft;
            scrollView.AddContent(collectionGameObject);
        }
        gridObjectCollection.UpdateCollection();
        gridObjectCollection.gameObject.SetActive(true);


        // Find ScrollView
        if (scrollView == null)
        {
            scrollView = GetComponentInChildren<ScrollingObjectCollection>();

            if (scrollView == null)
            {
                GameObject newScroll = new GameObject("Scrolling Object Collection");
                newScroll.transform.parent = scrollPositionRef ? scrollPositionRef : transform;
                newScroll.transform.localPosition = Vector3.zero;
                newScroll.transform.localRotation = Quaternion.identity;
                newScroll.SetActive(false);
                scrollView = newScroll.AddComponent<ScrollingObjectCollection>();

                // Prevent the scrolling collection from running until we're done dynamically populating it.
                scrollView.CellWidth = cellWidth;
                scrollView.CellHeight = cellHeight;
                scrollView.CellDepth = cellDepth;
                scrollView.CellsPerTier = cellsPerTier;
                scrollView.TiersPerPage = tiersPerPage;
            }

        }

        // Reset text
        if (text != null)
            text.SetText("");

        // Activate Scroll View
        if (scrollView != null)
            scrollView.gameObject.SetActive(true);
        else
            throw new ArgumentNullException("Assign a Scrolling ObjectCollection as Child of CustomScrollableListPopulator.");

        // Clipping Box
        if (clippingBox == null)
            clippingBox = scrollView.GetComponentInChildren<ClippingBox>();

        // Parameters
        numItems = 0;
    }

    #endregion Public Functions

    #region Private Functions

    /// <summary>
    /// Used in coroutine to slowly load objects.
    /// </summary>
    /// <param name="listType">If list type is "Objects", chosenSet can be null.</param>
    /// <param name="loaderViz"></param>
    /// <param name="chosenSet"></param>
    /// <returns></returns>
    private IEnumerator UpdateList(string listType, GameObject loaderViz, List<DataManager.Data> chosenSet = null)
    {
        if (listType == "Sets" && chosenSet == null)
            throw new System.ArgumentException(" When chosing a Set List, the chosen Set cannot be null.");

        // Show loader
        loaderViz.SetActive(true);

        // Populate list
        for (int currItemCount = 0; currItemCount < numItems; currItemCount++)
        {
            // Instantiate list buttons
            var button = Instantiate<GameObject>(buttonObject, gridObjectCollection.transform);
            button.SetActive(true);

            if (listType == "Objects")
            {
                // Set label
                var obj = dynamicItems[currItemCount];
                button.GetComponent<ButtonConfigHelper>().MainLabelText = obj.name;

                // Add listener
                button.GetComponent<ButtonConfigHelper>().OnClick.AddListener(() => InstantiateObject(obj, button));
            }
            else if (listType == "Sets")
            {
                // Set label
                button.GetComponent<ButtonConfigHelper>().MainLabelText = "UserID " + chosenSet[currItemCount].UserData.UserID.ToString() + " SetType " + chosenSet[currItemCount].UserData.Set.ToString();
                var Set = chosenSet[currItemCount];

                // Add listener
                button.GetComponent<ButtonConfigHelper>().OnClick.AddListener(() => SaveSettings(button, Set));
            }
            else
                throw new System.ArgumentException("ListType has to be either \"Objects\" or \"Sets\". ");

            instantatedObjects.Add(button);

            // Update renderers for clippingBox
            var renderer = button.GetComponentsInChildren<MeshRenderer>();
            foreach (var r in renderer)
                clippingBox.AddRenderer(r);

            yield return null;
        }

        // List ist populated, hence hide loader and show list
        loaderViz.SetActive(false);
        scrollView.gameObject.SetActive(true);

        // Set up collection and scroll view
        gridObjectCollection.UpdateCollection();
        scrollView.UpdateContent();
    }

    /// <summary>
    /// Instantiate objects when chosen in new settings menu
    /// </summary>
    /// <param name="obj"> Game object to instantiate</param>
    /// <param name="button"> Reference to button object to disable it after use. </param>
    private void InstantiateObject(GameObject obj, GameObject button)
    {
        // Spawn object
        objectCreator.SpawnObject(obj, GameManager.Instance.ParentPlayTable, Vector3.zero, Quaternion.identity, ConfigType.MovementEnabled);

        // Disable button to prevent several objects of the same type in scene
        button.SetActive(false);

        // Update button and object collection
        gridObjectCollection.UpdateCollection();
        GameManager.Instance.ParentPlayTable.GetComponent<GridObjectCollection>().UpdateCollection();

        // Update renderers in clipping box
        var renderer = button.GetComponentsInChildren<MeshRenderer>();
        foreach (var r in renderer)
            clippingBox.RemoveRenderer(r);
    }

    /// <summary>
    /// Write chosen text, when list ist set list
    /// Save chosen set to DataManater
    /// </summary>
    /// <param name="button"></param>
    /// <param name="chosenSet"></param>
    private void SaveSettings(GameObject button, DataManager.Data chosenSet)
    {
        // Update text
        if (text != null)
            text.text = button.GetComponent<ButtonConfigHelper>().MainLabelText;
        else
            throw new MissingComponentException("Add text Object to Custom Scrollable List");

        // Save settings
        if (chosenSet.IsValid())
            DataManager.Instance.CurrentSet = chosenSet;
    }

    #endregion Private Functions
}
