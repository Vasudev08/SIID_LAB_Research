using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class XRObjectRotate : MonoBehaviour
{
    public InputActionProperty leftJoystick;  // Assign Left Controller Joystick
    public InputActionProperty rightJoystick;  // Assign Right Controller Joystick

    public float rotationSpeed = 50f;
    public Transform objectTransform;

    [SerializeField] UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    

    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
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
}
