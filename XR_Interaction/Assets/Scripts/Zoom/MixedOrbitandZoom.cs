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

    // Public entry point for external scripts:
    public void TransitionToViewpoint(Viewpoint target_viewpoint)
    {
        StartCoroutine(MixedOrbitAndZoomRoutine(userViewpoint, target_viewpoint));
    }

    private IEnumerator MixedOrbitAndZoomRoutine(Viewpoint from_viewpoint, Viewpoint target_viewpoint)
    {
        // 1) Find the LCA between from_viewpoint and target_viewpoint
        Viewpoint lca = FindLeastCommonAncestor(from_viewpoint, target_viewpoint);

        // 2) Zoom Out to LCA vantage point
        yield return StartCoroutine(ZoomOutPhase(lca));

        // 3) Orbit around LCA (or around the ultimate target’s pivot)
        //    Use whichever pivot or position you prefer for “constant distance” orbit
        //yield return StartCoroutine(OrbitPhase(target_viewpoint.pivot, orbitDuration));

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
