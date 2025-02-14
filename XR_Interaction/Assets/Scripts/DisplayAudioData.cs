using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayAudioData : MonoBehaviour
{
    public string micDevice;
    public AudioClip micClip;
    [SerializeField] int sampleSize = 128; // Number of samples to capture
    private float[] samples;

    // Start is called before the first frame update
    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            micDevice = Microphone.devices[0]; // Use first available microphone
            micClip = Microphone.Start(micDevice, true, 10, 44100); // 10s buffer, 44.1kHz sample rate
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
        if (micDevice != null)
        {
            Debug.Log("Microphone started: " + micDevice);
        }
        // Get current mic position
        int micPosition = Microphone.GetPosition(micDevice) - sampleSize;
        if (micPosition < 0) return;

        // Get the latest audio data
        micClip.GetData(samples, micPosition);

        // Print raw audio sample data
        Debug.Log("Mic Data: " + string.Join(", ", samples));
        
    }
    void OnDestroy()
    {
        if (Microphone.IsRecording(micDevice))
        {
            Microphone.End(micDevice);
        }
    }
}
