using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ClippingBoxManager : MonoBehaviour
{
    Camera userCamera;
    public Viewpoint rootViewpoint;
    public Shader clippingShader; // Shader to not render parts of the model outside of the clipping box
    public float boundingBoxDistance = 2.0f;
    public float boundingBoxHalfSize = 3f;
    public Viewpoint targetViewpoint;
    public float padding = 1.1f;
    public float lerpSpeed = 2.0f;


    private List<Material> clippingMaterials = new List<Material>();
    private Renderer targetRenderer = new Renderer();

    [NonSerialized]
    public Vector3 currentCenter;
    [NonSerialized]
    public Vector3 currentExtents;

    void Awake()
    {
        Shader.WarmupAllShaders(); // Call before any materials are created
    }

    
    void Start()
    {
        userCamera = Camera.main;
        // SetClippingMaterials();
        // SetBoundingBox();
        StartCoroutine(DelayedInit());
    }

    IEnumerator DelayedInit()
    {
        yield return null; // let initial frame stabilize
        SetClippingMaterials();
        yield return null;
        SetBoundingBox();
    }

    void SetClippingMaterials()
    {
        foreach (Renderer renderer in this.GetComponentsInChildren<Renderer>())
        {
            Material[] original_mats = renderer.materials;
            Material[] new_mats = new Material[original_mats.Length];

            for (int i = 0; i < original_mats.Length; i++)
            {
                Material original = original_mats[i];
                Material clipping_material = new Material(clippingShader);

                if (original.HasProperty("_MainTex"))
                {
                    clipping_material.SetTexture("_MainTex", original.GetTexture("_MainTex"));
                }
                if (original.HasProperty("_Color"))
                {
                    clipping_material.SetColor("_Color", original.GetColor("_Color"));
                }
                new_mats[i] = clipping_material;
                clippingMaterials.Add(clipping_material);
            }
            renderer.materials = new_mats;

        }

        targetRenderer = targetViewpoint.modelRenderer;
        if (targetRenderer == null)
        {
            Vector3 center = targetViewpoint.pivot.position;
            Vector3 extents = Vector3.one * boundingBoxHalfSize;
            foreach (var mat in clippingMaterials)
            {
                mat.SetVector("_ClipBoxCenter", center);
                mat.SetVector("_ClipBoxExtents", extents);
            }

        }
        else
        {
            Vector3 center = targetRenderer.bounds.center;
            Vector3 extents = targetRenderer.bounds.extents;
            foreach (var mat in clippingMaterials)
            {
                mat.SetVector("_ClipBoxCenter", center);
                mat.SetVector("_ClipBoxExtents", extents);
            }
        }
    }

    void SetBoundingBox()
    {
        Vector3 center = userCamera.transform.position + Vector3.forward * boundingBoxDistance;
        currentCenter = center;
        Vector3 extents = Vector3.one * boundingBoxHalfSize;
        currentExtents = extents;

        Vector3 direction_to_center = currentCenter - rootViewpoint.pivot.position;
        this.transform.position = this.transform.position + direction_to_center;
        foreach (var mat in clippingMaterials)
        {
            mat.SetVector("_ClipBoxCenter", center);
            mat.SetVector("_ClipBoxExtents", extents);
        }

    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(currentCenter, currentExtents * 2.0f);
        
        
    }
    
    void OnDestroy()
    {
        // Clean up runtime-generated materials to avoid memory leaks
        foreach (var mat in clippingMaterials)
        {
            Destroy(mat);
        }
    }

    
}
