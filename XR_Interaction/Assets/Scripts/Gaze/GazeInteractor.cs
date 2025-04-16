using System;
using UnityEngine;

public class GazeInteractor : MonoBehaviour
{
    [SerializeField] float maxRayDistance = 10f;
    [SerializeField] Transform rayReticle;
    [SerializeReference] float reticleDistance = 2f;


    [NonSerialized] public GameObject currentTarget;   
    private Color oldColor;

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        rayReticle.position = ray.origin + reticleDistance * ray.direction;
        
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance))
        {
            GameObject target = hit.collider.gameObject;
            if (target != currentTarget)
            {
                if (currentTarget)
                {
                    OnGazeExit();
                }

                currentTarget = target;
                if (currentTarget.GetComponent<Renderer>())
                {
                    oldColor = currentTarget.GetComponent<Renderer>().material.color;
                }
                
                OnGazeEnter();
            }
        }
        else
        {
            if (currentTarget != null)
            {
                OnGazeExit();
                currentTarget = null;
            }
            
        }
    }

    void OnGazeEnter()
    {
        if (currentTarget.GetComponent<Renderer>())
        {
            currentTarget.GetComponent<Renderer>().material.color = Color.green;
        }
    }


    void OnGazeExit()
    {
        if (currentTarget.GetComponent<Renderer>())
        {
            currentTarget.GetComponent<Renderer>().material.color = oldColor;
        }
    }


}
