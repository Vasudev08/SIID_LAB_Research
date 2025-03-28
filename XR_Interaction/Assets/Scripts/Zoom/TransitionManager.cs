using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public ViewpointTransitionController mixedOrbitAndZoom;
    public Viewpoint startingViewpoint;
    public Viewpoint nextViewpoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        mixedOrbitAndZoom.userCamera.position = startingViewpoint.pivot.position + startingViewpoint.cameraOffsetPosition;
        mixedOrbitAndZoom.userCamera.rotation = startingViewpoint.cameraOffsetRotation;
        mixedOrbitAndZoom.userViewpoint = startingViewpoint;
        //mixedOrbitAndZoom.TransitionToViewpoint(startingViewpoint);
    }

    
}
