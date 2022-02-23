using Microsoft.MixedReality.Toolkit.Examples.Demos;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using UnityEngine;

/// <summary>
/// Class to get access to bounds conrol of game object "InteractionObjects".
/// A BoxCollider and a 
/// </summary>
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(BoundsControl))]
public class ToggleBoundsControl : MonoBehaviour
{
    #region Private Fields

    private bool isResizeEnabled;
    private BoxCollider boxCollider;
    private BoundsControl boundsControl;

    #endregion Private Fields

    #region MonoBehaviour Functions

    // Start is called before the first frame update
    void Start()
    {
        isResizeEnabled = true;
        boxCollider = (BoxCollider)GetComponent(typeof(BoxCollider));
        boundsControl = (BoundsControl)GetComponent(typeof(BoundsControl));
    }

    #endregion MonoBehaviour Functions

    #region Public Functions

    /// <summary>
    /// Set bounds control of interaction area and update spawn point of objects. 
    /// Called in editor.
    /// </summary>
    /// <param name="enable">Activate bounds control if true, deactivate if false.</param>
    public void SetResize(bool enable)
    {
        if (!enable)
        {
            boxCollider.enabled = false;
            boundsControl.Active = false;
            isResizeEnabled = false;
        }
        else
        {
            boxCollider.enabled = true;
            boundsControl.Active = true;
            isResizeEnabled = true;
        }

        // Update spawn point of objects
        TetheredPlacement[] gameObjComp = FindObjectsOfType<TetheredPlacement>();
        foreach (TetheredPlacement tp in gameObjComp)
            tp.LockSpawnPoint();
    }

    /// <summary>
    /// Toggle bounds control of interaction area and update spawn point of objects. 
    /// Called in editor.
    /// </summary>
    public void ToggleResize()
    {
        if (isResizeEnabled)
        {
            boxCollider.enabled = false;
            boundsControl.Active = false;
            isResizeEnabled = false;
        }
        else
        {
            boxCollider.enabled = true;
            boundsControl.Active = true;
            isResizeEnabled = true;
        }

        // Update spawn point of objects
        TetheredPlacement[] gameObjComp = FindObjectsOfType<TetheredPlacement>();
        foreach (TetheredPlacement tp in gameObjComp)
            tp.LockSpawnPoint();
    }

    #endregion Public Functions
}
