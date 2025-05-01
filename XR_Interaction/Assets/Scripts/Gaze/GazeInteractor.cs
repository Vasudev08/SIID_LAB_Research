using System;
using System.Collections.Generic;
using UnityEngine;

public class GazeInteractor : MonoBehaviour
{
    [Header("Ray Settings")]
    [SerializeField] float maxRayDistance = 10f;
    [SerializeField] Transform rayReticle;
    [SerializeReference] float reticleDistance = 2f;
    public Vector3 reticleOffset = new Vector3();


    [NonSerialized] public GameObject currentTarget;  // Current gaze target. 
    private List<Color> previousColors = new List<Color>(); // Stores the original color of the model before the highlighting from gaze
    [NonSerialized] public bool onTarget;
    [NonSerialized] public Viewpoint gazeTargetViewpoint;

    void Update()
    {
        StartGaze();
    }

    void StartGaze()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        rayReticle.position = ray.origin + reticleDistance * ray.direction;
        rayReticle.position += reticleOffset;
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance))
        {
            GameObject target = hit.collider.gameObject;
            if (target != currentTarget)
            {
                if (currentTarget)
                {
                    OnGazeExit();
                }

                GazeTargetController gazeTargetController = target.GetComponent<GazeTargetController>();
                if (gazeTargetController)
                {
                    currentTarget = target;
                    OnGazeEnter();
                }
            }
        }
        else
        {
            if (currentTarget != null)
            {
                OnGazeExit();
            }
            
        }
    }

    void OnGazeEnter()
    {
        onTarget = true;
        GazeTargetController gazeTargetController = currentTarget.GetComponent<GazeTargetController>();
        gazeTargetViewpoint = gazeTargetController.respectiveViewpoint;
        foreach (var renderer in gazeTargetController.renderers)
        {
            previousColors.Add(renderer.material.color);
            renderer.material.color = Color.yellow;
        }
        

    }


    void OnGazeExit()
    {
        onTarget = false;
        GazeTargetController gazeTargetController = currentTarget.GetComponent<GazeTargetController>();
        gazeTargetViewpoint = null;
        for (int i = 0; i < gazeTargetController.renderers.Count; i++)
        {
            gazeTargetController.renderers[i].material.color = previousColors[i];
        }
        previousColors.Clear();
        currentTarget = null;
    }


}
