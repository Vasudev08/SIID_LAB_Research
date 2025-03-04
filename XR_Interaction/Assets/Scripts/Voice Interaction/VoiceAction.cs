using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class VoiceAction : MonoBehaviour
{
    
    public Transform currentObject;
    [SerializeField] private float defaultRotationDegrees = 45f;
    [SerializeField] private float defaultTranslationDistance = 1f;
    [SerializeField] private float defaultScaleFactor = 2f;

    private Dictionary<string, Action<float>> transformCommands;
    private string chooseCommandMessage = "Please give a command for one of the three transformations: rotate, scale, translate";
    void Start()
    {
        transformCommands = new Dictionary<string, Action<float>>
        {
            { "rotate", RotateObject },
            { "scale", ScaleObject   },
            { "translate", TranslateObject },
            { "move", TranslateObject }
        };
    }

    public void ManipulateObject(string response)
    { 
        if (!ValidateResponse(response))
        {
            Debug.LogError("Could not execute your command. " + chooseCommandMessage);
            return;
        }

        string lowercase_response = response.ToLower();

        foreach (var command in transformCommands)
        {
            if (lowercase_response.Contains(command.Key))
            {
               
                float value = 1f;
                switch (command.Key)
                {
                    case "rotate":
                        value = defaultRotationDegrees;
                        break;
                    case "scale":
                        value = defaultScaleFactor;
                        break;
                    case "translate":
                    case "move":
                        value = defaultTranslationDistance;
                        break;
                }

                
                // Use regex to capture a number (integer or float, including negatives)
                Match match = Regex.Match(lowercase_response, @"(-?\d+(\.\d+)?)");
                if (match.Success)
                {
                    if (!float.TryParse(match.Value, out value))
                    {
                        value = defaultRotationDegrees;
                    }
                }
                // Execute the command with the parsed value
                command.Value.Invoke(value);
                return; // Stop after processing one command
            }
             
        }

        Debug.LogWarning("Could not find a command in your message. " + chooseCommandMessage);
        
        

    }
    
    public void RotateObject(float value)
    {
        currentObject.Rotate(new Vector3(0, value, 0), Space.Self);
    }

    public void TranslateObject(float value)
    {
        currentObject.position = new Vector3(currentObject.position.x, currentObject.position.y, currentObject.position.z + value);
    }

    public void ScaleObject(float value)
    {
        currentObject.localScale *= value;
    }


    public bool ValidateResponse(string response)
    {
        return !string.IsNullOrEmpty(response) && response.ToLower() != "error";
    }

}
