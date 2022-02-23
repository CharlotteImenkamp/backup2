using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

/// <summary>
/// Movement Constraint to add to constraint manager of game objects.
/// Prevents movement through table.
/// </summary>
public class CustomMovementConstraint : TransformConstraint
{
    #region Private Fields

    #region Serialized Fields

    [SerializeField]
    [Tooltip("Apply constraint or not.")]
    private bool useConstraint = true;

    [SerializeField]
    [Tooltip("Use Objects Transformation to calculate movement Boundaries ")]
    private Transform referenceTransform;

    [SerializeField]
    [EnumFlags]
    [Tooltip("Constrain movement along an axis")]
    private AxisFlags constraintOnMovement = 0;

    #endregion Serialized Fields

    private float minYAxis;

    #endregion Private Fields

    #region Public Fields

    public AxisFlags ConstraintOnMovement
    {
        get => constraintOnMovement;
        set => constraintOnMovement = value;
    }

    public override TransformFlags ConstraintType => TransformFlags.Move;

    // Use objects transformation to calculate movement boundaries
    public Transform ReferenceTransform { get => referenceTransform; set => referenceTransform = value; }

    #endregion Public Fields

    #region Public Functions

    /// <summary>
    /// Initialize with world position.
    /// </summary>
    /// <param name="worldPose"></param>
    public override void Initialize(MixedRealityTransform worldPose)
    {
        base.Initialize(worldPose);
        GetLowerBorder();
    }

    /// <summary>
    /// Apply Constraint to game object
    /// </summary>
    /// <param name="transform"></param>
    public override void ApplyConstraint(ref MixedRealityTransform transform)
    {
        if (useConstraint)
        {
            Vector3 position = transform.Position;

            // Apply constraints on y-axis if neccessary
            if (constraintOnMovement.HasFlag(AxisFlags.YAxis))
            {
                if (transform.Position.y <= minYAxis)
                    position.y = worldPoseOnManipulationStart.Position.y;
            }
            transform.Position = position;
        }
    }

    #endregion Public Functions

    #region Helper Functions

    /// <summary>
    /// Find constraint plane 
    /// </summary>
    private void GetLowerBorder()
    {
        if (referenceTransform == null)
            referenceTransform = GameObject.FindGameObjectWithTag("MovementConstraint").transform;

        minYAxis = referenceTransform.position.y;
    }

    #endregion Helper Functions
}
