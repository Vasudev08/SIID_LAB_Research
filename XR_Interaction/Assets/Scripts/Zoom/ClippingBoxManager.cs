using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ClippingBoxManager : MonoBehaviour
{
    public Transform userCamera;
    public Shader clippingShader; // Shader to not render parts of the model outside of the clipping box
    public float clippingBoxSize = 3f;
    public Renderer testRend;
    public Viewpoint targetViewpoint;
    public float padding = 1.1f;
    public float lerpSpeed = 2.0f;


    private List<Material> clippingMaterials = new List<Material>();
    private Renderer targetRenderer = new Renderer();

    private Vector3 currentCenter;
    private Vector3 currentExtents;
    
    void Start()
    {
        
        ApplyClippingMaterials();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateClippingBox();
    }


    void ApplyClippingMaterials()
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
            Vector3 extents = Vector3.one * clippingBoxSize;
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

    void UpdateClippingBox()
    {
        targetRenderer = targetViewpoint.modelRenderer;
        
        if (targetRenderer != null)
        {
            // Center & extents from the actual bounding box
            Vector3 center  = targetRenderer.bounds.center;
            Vector3 extents = targetRenderer.bounds.extents * padding;

            currentCenter = Vector3.Lerp(currentCenter, center, Time.deltaTime * lerpSpeed);
            currentExtents = Vector3.Lerp(currentExtents, extents, Time.deltaTime * lerpSpeed);

            
        }
        else
        {
            // Set center to current viewpoint pivot if there’s no targetRenderer
            Vector3 center  = targetViewpoint.pivot.position;
            Vector3 extents = Vector3.one * clippingBoxSize ;
            
            currentCenter = Vector3.Lerp(currentCenter, center, Time.deltaTime * lerpSpeed);
            currentExtents = Vector3.Lerp(currentExtents, extents, Time.deltaTime * lerpSpeed);

            
        }

        foreach (var mat in clippingMaterials)
        {
            mat.SetVector("_ClipBoxCenter", currentCenter);
            mat.SetVector("_ClipBoxExtents", currentExtents);
        }
    }

    /// <summary>
    /// Optional: draw the clipping box in the Scene view for debugging.
    /// </summary>
    void OnDrawGizmos()
    {
        // If you want to see the bounding box of the target
        // var bounds = targetRenderer.bounds;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(currentCenter, currentExtents * 2.0f);
        
        
    }
    
    // Draws a wireframe box around the selected object,
    // indicating world space bounding volume.
    public void OnDrawGizmosSelected()
    {
        var r = testRend;
        if (r == null)
            return;
        var bounds = r.bounds;
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(bounds.center, bounds.extents * 2);
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
