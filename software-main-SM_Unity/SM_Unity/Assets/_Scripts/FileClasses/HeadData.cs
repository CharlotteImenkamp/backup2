using System;
using UnityEngine;

/// <summary>
/// Class to hold head tracking specific data
/// </summary>
[System.Serializable]
public class HeadData
{
    #region Public Fields

    public Vector3 CameraPosition;
    public Quaternion CameraRotation;
    public Vector3 GazeOrigin;
    public Vector3 GazeDirection;
    public string TimeAfterStart;

    #endregion Public Fields

    #region Constructors

    public HeadData()
    {
        TimeAfterStart = Time.time.ToString();
    }

    public HeadData(Transform CameraTransform, Vector3 gazeOrigin, Vector3 gazeDirection)
    {
        // Check for null
        if (CameraTransform is null)
            throw new ArgumentNullException(nameof(CameraTransform));

        // Set parameters
        TimeAfterStart = Time.time.ToString();
        CameraPosition = CameraTransform.position;
        CameraRotation = CameraTransform.rotation;
        GazeOrigin = gazeOrigin;
        GazeDirection = gazeDirection;
    }

    #endregion Constructors

    #region Validation
    public bool IsValid()
    {
        if (CameraPosition == null || CameraRotation == null || GazeOrigin == null || GazeDirection == null)
            return false;
        else
            return true;
    }

    #endregion

    #region Public Functions

    /// <summary>
    /// Get camera position and rotation from transform
    /// </summary>
    /// <param name="t"></param>
    public void SetCameraParameters(Transform t)
    {
        if (t is null)
            throw new ArgumentNullException(nameof(t));

        TimeAfterStart = Time.time.ToString();
        CameraPosition = t.position;
        CameraRotation = t.rotation;
    }

    /// <summary>
    /// Set gaze origin and gaze direction
    /// </summary>
    /// <param name="gazeOrigin"></param>
    /// <param name="gazeDirection"></param>
    public void SetGazeParameters(Vector3 gazeOrigin, Vector3 gazeDirection)
    {
        TimeAfterStart = Time.time.ToString();
        GazeOrigin = gazeOrigin;
        GazeDirection = gazeDirection;
    }

    #endregion Public Functions

}
