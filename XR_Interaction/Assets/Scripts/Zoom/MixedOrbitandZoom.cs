using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixedOrbitandZoom : MonoBehaviour
{
    public Transform userCamera;
    public Transform modelRoot;
    public Viewpoint userViewpoint; // current viewpoint the user is at

    [Header("Transition Timing")]
    public float zoomOutDuration = 1.0f;
    public float orbitDuration   = 1.0f;
    public float zoomInDuration  = 1.0f;
    public float rotateSpeed = 10f;

    // Public entry point for external scripts:
    public void TransitionToViewpoint(Viewpoint target_viewpoint)
    {
        StartCoroutine(MixedOrbitAndZoomRoutine(userViewpoint, target_viewpoint));
    }

    private IEnumerator MixedOrbitAndZoomRoutine(Viewpoint from_viewpoint, Viewpoint target_viewpoint)
    {
        Viewpoint lca = FindLeastCommonAncestor(from_viewpoint, target_viewpoint);

        
        yield return StartCoroutine(ZoomOutPhase(lca));

        yield return StartCoroutine(OrbitPhase(target_viewpoint));

        // 4) Zoom In to the target viewpoint
       //yield return StartCoroutine(ZoomInPhase(target_viewpoint));

        // Update current viewpoint reference
        userViewpoint = target_viewpoint;
    }

    private IEnumerator ZoomOutPhase(Viewpoint lca)
    {
        Vector3 start_scale = modelRoot.localScale;
        Vector3 target_scale = Vector3.one * lca.levelOfScale;

        Vector3 start_position = userCamera.position;
        Quaternion start_rotation = userCamera.rotation;

        Vector3 vantage_position = lca.pivot.position + lca.cameraOffsetPosition;
        Quaternion vantage_rotation = lca.cameraOffsetRotation;
        float elapsed_time = 0f;
        while (elapsed_time < zoomOutDuration)
        {
            elapsed_time += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed_time / zoomOutDuration);

            // Scale model
            modelRoot.localScale = Vector3.Lerp(start_scale, target_scale, t);

            // Move camera
            userCamera.position = Vector3.Lerp(start_position, vantage_position, t);

            // Rotate camera
            userCamera.rotation = Quaternion.Slerp(start_rotation, vantage_rotation, t);

            yield return null;
        }
    }

    private IEnumerator OrbitPhase(Viewpoint target_viewpoint)
    {
        Quaternion start_rotation = userCamera.rotation;

        Vector3 start_position = userCamera.position;
        float radius = Vector3.Distance(userCamera.position, target_viewpoint.pivot.localPosition);
        Vector3 direction_to_look = (target_viewpoint.pivot.position - userCamera.position).normalized; // view direction the camera needs to match to
        Quaternion end_rotation = Quaternion.identity;

        Vector3 end_position = target_viewpoint.pivot.position + (target_viewpoint.pivot.forward * radius);

        float elapsed_time = 0f;
        Debug.Log(Quaternion.Dot(userCamera.rotation, end_rotation));
        while (elapsed_time < orbitDuration)
        {   
            elapsed_time += Time.deltaTime;
            
            float t = Mathf.Clamp01(elapsed_time / orbitDuration);
            userCamera.position   = Vector3.Slerp(start_position, end_position, t);
            userCamera.rotation = Quaternion.Slerp(start_rotation, end_rotation, t);
            yield return null;
           
        }
        Debug.Log("Finished orbit for: " + target_viewpoint.name);
        
    }

    private Viewpoint FindLeastCommonAncestor(Viewpoint current_viewpoint, Viewpoint target_viewpoint)
    {
        HashSet<Viewpoint> ancestors = new HashSet<Viewpoint>();

        Viewpoint temp = current_viewpoint;

        while (temp != null)
        {
            ancestors.Add(temp);
            temp = temp.parent;
        }

        Viewpoint lca = target_viewpoint;

        while (lca != null && !ancestors.Contains(lca))
        {
            lca = lca.parent;
        }
        
        if (lca == null)
        {
            return userViewpoint;
        }
        return lca;
    }
}
