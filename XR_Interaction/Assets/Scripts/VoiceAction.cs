using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceAction : MonoBehaviour
{
    
    public Transform currentObject;
    [SerializeField] private float rotationDegrees = 45f;

    public void ManipulateObject(string response)
    { 
        bool gotResponse = ValidateResponse(response);
        if (gotResponse)
        {
            if(response.ToLower().Contains("rotate"))
            {
                currentObject.Rotate(new Vector3(0, rotationDegrees, 0), Space.Self);
            }
        }
        

    }

    public bool ValidateResponse(string response)
    {
        if (response != "error" && response != null)
        {
            return true;
        }
        return false;
    }

}
