using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Viewpoint : MonoBehaviour
{
    public Viewpoint parent;
    public Button button;
    public List<Viewpoint> childrenViewpoints;
    public Renderer modelRenderer;

    [Header("Transform Data for This Level of Scale")]
    public float levelOfScale = 1f;            // Scale for the entire model at this LoS
    public Transform pivot;                    // Transform/pivot of this LoS. For example, center of whole body
    
    public Vector3 cameraOffsetPosition;
    public Quaternion cameraOffsetRotation;


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
