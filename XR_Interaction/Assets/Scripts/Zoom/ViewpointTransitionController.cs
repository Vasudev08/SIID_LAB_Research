using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class ViewpointTransitionController : MonoBehaviour
{
    public Transform userCamera;
    public Transform modelRoot;
    public Viewpoint modelRootViewpoint;
    public Viewpoint userViewpoint; // current viewpoint the user is at
    public ClippingBoxManager clippingBoxManager;
    public VoiceRecognition voiceRecognition;

    [Header("Transition Timing")]
    public float transitionBaseDuration = 1f; 
    public float minDurationFactor = 1f; 
    public float maxDurationFactor = 5f; 

    void Start()
    {
        TransitionToViewpoint(modelRootViewpoint);
    }
    // Public entry point for external scripts:
    public void TransitionToViewpoint(Viewpoint target_viewpoint)
    {
        
        target_viewpoint.button.interactable = false;
        voiceRecognition.inTransition = true;
        StartCoroutine(MixedOrbitAndZoomRoutine(userViewpoint, target_viewpoint));
    }

    private IEnumerator MixedOrbitAndZoomRoutine(Viewpoint from_viewpoint, Viewpoint target_viewpoint)
    {

        Viewpoint lca = FindLeastCommonAncestor(from_viewpoint, target_viewpoint);

        yield return StartCoroutine(ZoomOutPhase(lca));

        yield return StartCoroutine(ZoomInPhase(target_viewpoint));

        // Update current viewpoint reference
        userViewpoint = target_viewpoint;
        target_viewpoint.button.interactable = true;
        voiceRecognition.inTransition = false;
    }
    

    
    private IEnumerator ZoomOutPhase(Viewpoint lca)
    {
        clippingBoxManager.targetViewpoint = modelRootViewpoint;
        if (userViewpoint == lca)
        {
            yield break;
        }
        
        Vector3 start_position = modelRoot.position;
        Vector3 start_scale = modelRoot.localScale;

        Vector3 end_scale = Vector3.one * lca.levelOfScale;
        
        // Temporarily set the model to the end scale to get the center of the bounding box to move the viewpoint to.
        modelRoot.localScale = end_scale;
        Vector3 offset = clippingBoxManager.currentCenter - lca.pivot.position;
        modelRoot.localScale = start_scale;

        Vector3 end_position = modelRoot.position + offset;

        
        float elapsed_time = 0f;
        float scale_ratio = start_scale.x / end_scale.x;
        float transition_duration = transitionBaseDuration * Mathf.Clamp(Mathf.Log(scale_ratio, 10), minDurationFactor, maxDurationFactor);
        // Debug.Log("Zoom Out: " + transition_duration);
        while (elapsed_time < transition_duration)
        {
            elapsed_time += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed_time / transition_duration);
            
            modelRoot.localScale = Vector3.Lerp(start_scale, end_scale, t);

            modelRoot.position = Vector3.Lerp(start_position, end_position, t);

            

            yield return null;
        }
        modelRoot.localScale = end_scale;
        modelRoot.position = end_position;
        // Debug.Log("Finished Zoom out for: " + userViewpoint);
        userViewpoint = lca;
    }
    

    
    private IEnumerator ZoomInPhase(Viewpoint target_viewpoint)
    {
        clippingBoxManager.targetViewpoint = target_viewpoint;
        if (userViewpoint == target_viewpoint)
        {
            yield break;
        }  
        
        Vector3 start_position = modelRoot.position;
        Vector3 start_scale = modelRoot.localScale;

        Vector3 end_scale = Vector3.one * target_viewpoint.levelOfScale;
        
        // Temporarily set the model to the end scale to get the center of the bounding box to move the viewpoint to.
        modelRoot.localScale = end_scale;
        Vector3 offset = clippingBoxManager.currentCenter - target_viewpoint.pivot.position;
        modelRoot.localScale = start_scale;

        Vector3 end_position = modelRoot.position + offset;


        float elapsed_time = 0f;
        float scale_ratio = end_scale.x / start_scale.x;
        float transition_duration = transitionBaseDuration * Mathf.Clamp(Mathf.Log(scale_ratio, 10), minDurationFactor, maxDurationFactor);
        // Debug.Log("Zoom In: " + transition_duration);
        while (elapsed_time < transition_duration)
        {
            elapsed_time += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed_time / transition_duration);
            
            modelRoot.localScale = Vector3.Lerp(start_scale, end_scale, t);

            modelRoot.position = Vector3.Lerp(start_position, end_position, t);

            yield return null;
        }
        modelRoot.localScale = end_scale;
        modelRoot.position = end_position;
        // Debug.Log("Finished zooming in for target: " + target_viewpoint.name);
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
