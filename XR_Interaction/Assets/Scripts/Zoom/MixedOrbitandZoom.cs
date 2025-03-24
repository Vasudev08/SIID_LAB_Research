using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
        target_viewpoint.button.interactable = false;
        StartCoroutine(MixedOrbitAndZoomRoutine(userViewpoint, target_viewpoint));
    }

    private IEnumerator MixedOrbitAndZoomRoutine(Viewpoint from_viewpoint, Viewpoint target_viewpoint)
    {

        Viewpoint lca = FindLeastCommonAncestor(from_viewpoint, target_viewpoint);

        
        yield return StartCoroutine(ZoomOutPhase(lca));

        yield return StartCoroutine(OrbitPhase(lca, target_viewpoint));

        // 4) Zoom In to the target viewpoint
        yield return StartCoroutine(ZoomInPhase(target_viewpoint));

        // Update current viewpoint reference
        userViewpoint = target_viewpoint;
        target_viewpoint.button.interactable = true;
    }

    private IEnumerator ZoomOutPhase(Viewpoint lca)
    {
        Vector3 start_scale = modelRoot.localScale;
        Vector3 target_scale = Vector3.one * lca.levelOfScale;

        Vector3 start_position = userCamera.position;
        Quaternion start_rotation = userCamera.rotation;

        // Get radius of the circular arc that the LCA LoS is at. This is based on where the intended position of the camera at the LoS is at.
        float distance = Vector3.Distance(lca.pivot.localPosition, lca.pivot.localPosition + lca.cameraOffsetPosition);
        Vector3 direction_to_camera = (userCamera.position - lca.pivot.position).normalized;
        Debug.Log("Zoom out distance: " + distance);
        
        Vector3 vantage_position = lca.pivot.position + direction_to_camera * distance;
        Vector3 x = userCamera.position - lca.pivot.position;
        Debug.Log("Vantage Point: " + x +  " " + lca.pivot.position + " " + userCamera.position + " " + vantage_position);
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
            // userCamera.rotation = Quaternion.Slerp(start_rotation, vantage_rotation, t);

            yield return null;
        }
        Debug.Log("Finished Zoom out for: " + userViewpoint);
    }

    private IEnumerator OrbitPhase(Viewpoint lca, Viewpoint target_viewpoint)
    {
        Quaternion start_rotation = userCamera.rotation;

        Vector3 start_position = userCamera.position;
        // Distance from the lca pivot to the camera currently after zooming out.
        float radius = Vector3.Distance(userCamera.position, lca.pivot.position);
        
        // Final target position at the target viewpoint/LoS (***Not the end position to orbit to***)
        Vector3 position_at_target = target_viewpoint.pivot.position + target_viewpoint.cameraOffsetPosition;
        Vector3 direction_to_target = (position_at_target - lca.pivot.position).normalized; // Direction to the final target position at the target viewpoint
        Debug.Log("Orbit distance " + radius);
        Quaternion end_rotation = quaternion.identity;
        if (end_rotation.eulerAngles == Vector3.zero)
        {
            end_rotation = Quaternion.identity; // Set it to the identity. If it's just 0,0,0 in the inspector it will just snap instead of lerping for some reason
        }
        Vector3 end_position = lca.pivot.position + direction_to_target * radius;
        Debug.Log("End Pos: " + end_position + " Target " + target_viewpoint.pivot.position + " " + target_viewpoint.pivot.position + target_viewpoint.cameraOffsetPosition);
        float elapsed_time = 0f;
        Debug.Log("Rotation: " + userCamera.rotation + " Target Rot: " + target_viewpoint.cameraOffsetRotation + " " +Quaternion.Dot(userCamera.rotation, end_rotation));
        while (elapsed_time < orbitDuration)
        {   
            elapsed_time += Time.deltaTime;
            
            float t = Mathf.Clamp01(elapsed_time / orbitDuration);
            userCamera.position   = Vector3.Slerp(start_position, end_position, t);
            userCamera.rotation = Quaternion.Slerp(start_rotation, end_rotation, t);
            yield return null;
           
        }
        Debug.Log("Finished orbit for target: " + target_viewpoint.name);
        
    }

    private IEnumerator ZoomInPhase(Viewpoint target_viewpoint)
    {
        Vector3 start_scale = modelRoot.localScale;
        Vector3 start_position = userCamera.position;
        Quaternion start_rotation = userCamera.rotation;

        // Vector3 end_position = target_viewpoint.pivot.position + target_viewpoint.cameraOffsetPosition;
        Vector3 end_scale = Vector3.one * target_viewpoint.levelOfScale;
        Quaternion end_rotation = target_viewpoint.cameraOffsetRotation;



        float elapsed_time = 0f;
        while (elapsed_time < zoomInDuration)
        {
            Vector3 new_camera_offset_position = target_viewpoint.pivot.TransformPoint(target_viewpoint.cameraOffsetPosition);
            Debug.Log(new_camera_offset_position);
            new_camera_offset_position = new_camera_offset_position - target_viewpoint.pivot.position;
            Vector3 end_position = target_viewpoint.pivot.position + new_camera_offset_position;

            elapsed_time += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed_time / zoomInDuration);
            modelRoot.localScale = Vector3.Lerp(start_scale, end_scale, t);
            userCamera.position = Vector3.Lerp(start_position, end_position, t);
            userCamera.rotation = Quaternion.Slerp(start_rotation, end_rotation, t);
            yield return null;
        }

        Debug.Log("Finished zooming in for target: " + target_viewpoint.name);
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
