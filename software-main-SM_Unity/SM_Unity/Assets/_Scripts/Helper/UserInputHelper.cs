using Microsoft.MixedReality.Toolkit.UI;
using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Used in "UserSettingsUI" and "ToggleCollection_Set" in Editor.
/// Helper class to get input of editor buttons and display results if neccessary.
/// </summary>
public class UserInputHelper : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    [Tooltip("Write ID here. Can be null.")]
    private TextMeshPro idObj;

    [SerializeField]
    [Tooltip("Write chosen set here. Can be null.")]
    private TextMeshPro setObj;

    [SerializeField]
    [Tooltip("Input list. Can be null.")]
    private CustomScrollableListPopulator newObjectList;

    #endregion Serialized Fields

    #region Private Fields

    // Dynamic text depending on user choice
    private string userID;
    private string userSet;

    // Static text
    private string idText;
    private string setText;

    private UserSet set;

    #endregion Private Fields

    #region Public Fields

    // ID from user input
    public string UserID { get => userID; set => userID = value ?? throw new ArgumentNullException(nameof(value), "Name cannot be null"); }

    // Set from user input
    public UserSet Set { get => set; set => set = value; }

    #endregion Public Fields

    #region MonoBehaviour Functions

    // Called before the first frame update
    // Set default values
    void Start()
    {
        idText = "User ID:";
        setText = "User Set: ";

        if (idObj != null)
            idObj.text = idText;

        userID = "";
        userSet = "";

        set = new UserSet();
    }

    /// <summary>
    /// Reset parameters
    /// </summary>
    private void Reset()
    {
        userID = "";
        userSet = "";

        if (idObj != null)
            idObj.text = idText + userID;

        if (setObj != null)
            setObj.text = setText + userSet;

        set = new UserSet();
    }
    #endregion MonoBehaviour Functions

    #region Button Functions

    /// <summary>
    /// Attached in editor buttons at onClick event
    /// </summary>
    /// <param name="obj">GameObject, which MainLabelText is checked for Input.</param>
    public void GetKeyInput(GameObject obj)
    {
        string text = obj.GetComponent<ButtonConfigHelper>().MainLabelText;

        // Switch text
        if (text == "1")
            userID += "1";
        else if (text == "2")
            userID += "2";
        else if (text == "3")
            userID += "3";
        else if (text == "4")
            userID += "4";
        else if (text == "5")
            userID += "5";
        else if (text == "6")
            userID += "6";
        else if (text == "7")
            userID += "7";
        else if (text == "8")
            userID += "8";
        else if (text == "9")
            userID += "9";
        else if (text == "0")
            userID += "0";
        else if (text == "Clear")
            userID = "";
        else if (text == "AG")
        {
            userSet = "AG";
            set = UserSet.AG;
        }
        else if (text == "JG")
        {
            userSet = "JG";
            set = UserSet.JG;
        }
        else if (text == "AE")
        {
            userSet = "AE";
            set = UserSet.AK;
        }

        // Set text
        idObj.text = idText + userID;
        setObj.text = setText + userSet;
    }

    /// <summary>
    /// Called on apply button in UserSettingsUI in editor
    /// </summary>
    public void GenerateNewData()
    {
        // Prepare settings
        UserSettingsData userData = new UserSettingsData(UserID, Set, GameManager.Instance.UpdateRate);
        ObjectData objData = newObjectList.GetInstantiatedObjects();

        // Save settings
        if (objData.IsValid() && userData.IsValid())
            DataManager.Instance.SetAndSaveNewSettings(new DataManager.Data(objData, userData));

        Reset();
    }

    /// <summary>
    /// Reset toggle collection when choosing user set. 
    /// Called on apply button and on return button to reset current index.
    /// </summary>
    /// <param name="idx"></param>
    public void ResetToggleList(int idx)
    {
        InteractableToggleCollection toggleCollection = GetComponent<InteractableToggleCollection>();
        if (toggleCollection != null)
        {
            if (idx <= toggleCollection.ToggleList.Length)
                toggleCollection.CurrentIndex = idx;
            else
                Debug.LogError("UserInputHelper::ResetToggleList Index exceeds List");
        }
        else
            Debug.LogError("UserInputHelper::ResetToggleList must be attached to an actor with a toggle collection");
    }

    #endregion Button Functions
}
