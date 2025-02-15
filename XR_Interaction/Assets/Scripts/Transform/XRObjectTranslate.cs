using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class XRObjectTranslate : MonoBehaviour
{
    public InputActionProperty leftJoystick;  // Assign Left Controller Joystick
    public InputActionProperty rightJoystick;  // Assign Right Controller Joystick

    public float moveSpeed = 0.5f;

    public Transform objectTransform;

    [SerializeField] XRGrabInteractable grabInteractable;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        
    }

    void Update()
    {
        if (grabInteractable.isSelected)
        {
            Vector2 leftInput = leftJoystick.action.ReadValue<Vector2>();
            Vector2 rightInput = rightJoystick.action.ReadValue<Vector2>();

            float x_movement = leftInput.x;
            float y_movement = rightInput.y;

            Vector3 target_direction = new Vector3(x_movement, 0, y_movement);
            objectTransform.position += target_direction * moveSpeed * Time.deltaTime;
        }
    }



}
