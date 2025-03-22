using UnityEngine;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    public Button startVoiceButton;
    public Button stopVoiceButton;
    public VoiceRecognition voiceRecognition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stopVoiceButton.interactable =false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartListening()
    {
        startVoiceButton.interactable = false;
        stopVoiceButton.interactable = true;
        voiceRecognition.StartRecording();
    }
    
    public void StopListening()
    {
        stopVoiceButton.interactable = false;
        startVoiceButton.interactable = true;
        voiceRecognition.StopRecording();
    }
}
