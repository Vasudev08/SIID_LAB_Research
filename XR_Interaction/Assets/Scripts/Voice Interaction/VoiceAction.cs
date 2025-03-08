using UnityEngine;


public class VoiceAction : MonoBehaviour
{
    public string chooseCommandMessage = "Please give a command for one of the three transformations: rotate, scale, translate";

    public void RotateObject(GameObject obj, float value)
    {
        obj.transform.Rotate(new Vector3(0, value, 0), Space.World);
    }

    public void TranslateObject(GameObject obj, float value)
    {
        obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z + value);
    }

    public void ScaleObject(GameObject obj, float value)
    {
        obj.transform.localScale *= value;
    }


    public bool ValidateResponse(string response)
    {
        return !string.IsNullOrEmpty(response) && response.ToLower() != "error";
    }

}
