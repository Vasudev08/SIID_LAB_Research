using UnityEngine;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    public Button startVoiceButton;
    public Button stopVoiceButton;
    public VoiceRecognition voiceRecognition;

    void Start()
    {
        stopVoiceButton.interactable =false;
    }

    
    public void StartListening()
    {
        startVoiceButton.interactable = false;
        stopVoiceButton.interactable = true;
        voiceRecognition.StartRecording();
        Debug.LogAssertion("Start Button Works");
    }
    
    public void StopListening()
    {
        stopVoiceButton.interactable = false;
        startVoiceButton.interactable = true;
        voiceRecognition.StopRecording();
    }
}
