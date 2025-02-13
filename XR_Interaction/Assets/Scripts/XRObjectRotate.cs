using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class XRObjectRotate : MonoBehaviour
{
    public InputActionProperty leftJoystick;  // Assign Left Controller Grip
    public InputActionProperty rightJoystick;  // Assign Right Controller Grip

    public float rotationSpeed = 50f;
    public Transform objectTransform;

    [SerializeField] XRGrabInteractable grabInteractable;
    private bool isHeld = false;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        // grabInteractable.selectEntered.AddListener(OnGrab);
        // grabInteractable.selectExited.AddListener(OnRelease);
    }

    void Update()
    {
        if (grabInteractable.isSelected)
        {
            
            Vector2 leftInput = leftJoystick.action.ReadValue<Vector2>();
            Vector2 rightInput = rightJoystick.action.ReadValue<Vector2>();

            float yRotation = rightInput.x * rotationSpeed * Time.deltaTime;
            float xRotation = leftInput.y * rotationSpeed * Time.deltaTime;
            
            objectTransform.Rotate(Vector3.up, yRotation, Space.Self);
            objectTransform.Rotate(Vector3.right, xRotation, Space.Self);
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
