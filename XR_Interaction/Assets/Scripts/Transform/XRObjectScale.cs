using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class XRObjectScale : MonoBehaviour
{
    public InputActionProperty leftTrigger;  // Assign Left Controller Trigger
    public InputActionProperty rightTrigger;  // Assign Right Controller Trigger

    public float scaleSpeed = 0.5f;
    public Vector3 minScale = new Vector3(0.1f, 0.1f, 0.1f);
    public Vector3 maxScale = new Vector3(3f, 3f, 3f);
    public Transform objectTransform;

    [SerializeField] XRGrabInteractable grabInteractable;
    private bool isHeld = false;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void Update()
    {
        if (isHeld)
        {
            float leftTriggerValue = leftTrigger.action.ReadValue<float>();
            float rightTriggerValue = rightTrigger.action.ReadValue<float>();

            float scaleFactor = (rightTriggerValue - leftTriggerValue) * scaleSpeed * Time.deltaTime;
            Vector3 newScale = objectTransform.localScale + Vector3.one * scaleFactor;
            // Clamp to avoid extreme scaling
            objectTransform.localScale = Vector3.Max(minScale, Vector3.Min(maxScale, newScale));
        }
    }


    void OnGrab(SelectEnterEventArgs args)
    {
        isHeld = true;
    }


    void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;
    }

}
