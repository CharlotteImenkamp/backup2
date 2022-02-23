using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data class to hold object specific data
/// </summary>
[System.Serializable]
public class ObjectData
{
    #region Public Fields

    public float Time;
    public Vector3 PositionOffset;
    public List<CustomObject> GameObjects;

    #endregion Public Fields

    #region Validation 

    public bool IsValid()
    {
        if (GameObjects is null)
            return false;
        else
            return true;
    }

    #endregion 

    #region Constructors

    // Constructor to convert game object array to list 
    public ObjectData(GameObject[] movingObj, float time, Vector3 positionOffset)
    {
        if (movingObj is null)
            throw new System.ArgumentNullException(nameof(movingObj));

        // Set parameters
        GameObjects = new List<CustomObject>();
        this.Time = time;
        this.PositionOffset = positionOffset;

        // Convert game object to custom object
        foreach (GameObject obj in movingObj)
        {
            var intObj = new CustomObject(obj.name, obj.transform.position, obj.transform.rotation);
            GameObjects.Add(intObj);
        }
    }

    // Constructor with game object list
    public ObjectData(List<GameObject> movingObj, float time, Vector3 positionOffset)
    {
        // Set parameters
        GameObjects = new List<CustomObject>();
        this.Time = time;
        this.PositionOffset = positionOffset;

        // Convert game object to custom object
        foreach (GameObject obj in movingObj)
        {
            // Remove naming conventions from instantiating
            if (obj.name.Contains("(Clone)"))
                obj.name = obj.name.Replace("(Clone)", "");

            var intObj = new CustomObject(obj.name, obj.transform.position, obj.transform.rotation);
            GameObjects.Add(intObj);
        }
    }

    // Constructor with custom object list 
    public ObjectData(List<CustomObject> objList, float time, Vector3 positionOffset)
    {
        this.GameObjects = objList;
        this.Time = time;
        this.PositionOffset = positionOffset;
    }

    public ObjectData() { }

    #endregion Constructors

    #region Public Functions

    /// <summary>
    /// Returns global positions of game object list
    /// </summary>
    /// <returns></returns>
    public Vector3[] GetObjectPositions()
    {
        Vector3[] positions = new Vector3[GameObjects.Count];
        for (int i = 0; i < GameObjects.Count; i++)
            positions[i] = GameObjects[i].GlobalPosition;

        return positions;
    }

    /// <summary>
    /// Returns global rotations of game object list
    /// </summary>
    /// <returns></returns>
    public Quaternion[] GetObjectRotations()
    {
        Quaternion[] rotations = new Quaternion[GameObjects.Count];
        for (int i = 0; i < GameObjects.Count; i++)
        {
            rotations[i] = GameObjects[i].GlobalRotation;
        }
        return rotations;
    }

    public void Clear()
    {
        GameObjects.Clear();
        PositionOffset = Vector3.zero;
        Time = 0.0f;
    }


    #endregion Public Functions
}

/// <summary>
/// Helper class, which is only used in object data. 
/// Object, which holds only the relevant information
/// </summary>
[System.Serializable]
public class CustomObject
{
    public string Objectname;
    public Vector3 GlobalPosition;
    public Quaternion GlobalRotation;

    public CustomObject(string name, Vector3 position, Quaternion rotation)
    {
        this.Objectname = name;
        this.GlobalPosition = position;
        this.GlobalRotation = rotation;
    }
}