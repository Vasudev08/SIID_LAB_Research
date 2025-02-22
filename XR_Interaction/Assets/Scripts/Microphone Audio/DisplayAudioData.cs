using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayAudioData : MonoBehaviour
{
    public string micDevice;
    public AudioClip micClip;
    [SerializeField] int sampleSize = 128; // Number of samples to capture
    public TMPro.TextMeshProUGUI textField;
    public float threshold = 0.1f;
    private float[] samples;
    public float sensibility = 10f;
    public MicSelectionManager micSelectionManager;

    // Start is called before the first frame update
    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            micDevice = Microphone.devices[0]; // Use first available microphone
            micClip = Microphone.Start(micDevice, true, 10, AudioSettings.outputSampleRate); // 10s buffer, sample rate
            micSelectionManager.StartMic(micDevice);
            samples = new float[sampleSize];
            // Debug.Log("Microphone started: " + micDevice);
        }
        else
        {
            Debug.LogError("No microphone detected!");
        }
    }

    // Update is called once per frame
    void Update()
    {
         
        if (micClip == null || !Microphone.IsRecording(micDevice)) return;
        
        // Get current mic position
        int micPosition = Microphone.GetPosition(micDevice) - sampleSize;
        if (micPosition < 0) return;
        samples = new float[sampleSize];
        micClip.GetData(samples, micPosition);
        float loudness = GetLoudness(micPosition) * sensibility; 
        
        if (loudness < threshold)
        {
            
            textField.text = "0";
            return;
        }

        // Print raw audio sample data
        // Debug.Log("Mic Data: " + string.Join(", ", samples));
        textField.text = string.Join(", ", samples);
        
    }
    public float GetLoudness(int startPosition)
    {
        float total_loudness = 0;
        for (int i = 0 ; i <  sampleSize; i++)
        {
            total_loudness += Mathf.Abs(samples[i]);
        }
        return total_loudness / sampleSize;
    }
    
    void OnDestroy()
    {
        if (Microphone.IsRecording(micDevice))
        {
            Microphone.End(micDevice);
        }
    }
}
