using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public MixedOrbitandZoom mixedOrbitAndZoom;
    public GameObject startingViewpoint;
    public Viewpoint nextViewpoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mixedOrbitAndZoom.userViewpoint = startingViewpoint.GetComponent<Viewpoint>();
        mixedOrbitAndZoom.TransitionToViewpoint(nextViewpoint);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
