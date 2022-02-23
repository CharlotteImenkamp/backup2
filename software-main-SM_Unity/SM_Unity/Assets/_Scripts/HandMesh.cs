using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class HandMesh : MonoBehaviour
{
    private MixedRealityHandTrackingProfile handTrackingProfile;

    // Start is called before the first frame update
    void Start()
    {
        MixedRealityInputSystemProfile inputSystemProfile = CoreServices.InputSystem?.InputSystemProfile;
        if (inputSystemProfile == null)
        {
            return;
        }

        handTrackingProfile = inputSystemProfile.HandTrackingProfile;

        if (handTrackingProfile != null)
        {
            handTrackingProfile.EnableHandMeshVisualization = true;
        }
    }

    /// <summary>
    /// Toggles hand mesh visualization
    /// </summary>
    public void OnToggleHandMesh()
    {
        if (handTrackingProfile != null)
        {
            handTrackingProfile.EnableHandMeshVisualization = !handTrackingProfile.EnableHandMeshVisualization;
        }
    }
}
