using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Viewpoint : MonoBehaviour
{
    public Viewpoint parent;
    public Button button;
    public List<Viewpoint> childrenViewpoints;

    [Header("Transform Data for This Level of Scale")]
    public float levelOfScale = 1f;            // Scale for the entire model at this LoS
    public Transform pivot;       // Transform/pivot of this LoS. For example, center of whole body
    
    // Transform the viewpoint will be at the target viewpoint based on the pivot
    public Vector3 cameraOffsetPosition;
    public Quaternion cameraOffsetRotation;

    
    // Transform the viewpoint will zoom out to for the zoom out phase


    // Auto-update child-parent relationships when viewed in inspector
    private void OnValidate()
    {   
        if (childrenViewpoints != null && childrenViewpoints.Count > 0)
        {
            foreach (Viewpoint child in childrenViewpoints)
            {
                if (child != null)
                {
                    child.parent = this;
                }
            }
        }
    }
}
