using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HuggingFace.API;
using UnityEngine.UI;
using System;
using System.IO;
using WebRtcVadSharp;

public class VoiceRecognition : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private TMPro.TextMeshProUGUI inputText;
    [SerializeField] private AudioSource audioSource;
    public VoiceAction voiceAction;
    public float loudnessThreshold = 0.1f; // Threshold for audio sample to be above to detect a "voice" or noise
    public float silenceDuration = 1f; // Seconds of silence to consider the end of  speech.



    private string micDevice;
    private int sampleRate = 16000;
    private AudioClip clip;
    private byte[] wavData;

    private bool recording;
    private string recognizedSpeech;

    
    private List<float> speechBuffer = new List<float>(); // Buffer to store audio samples above threshold.
    private float silenceTimer = 0f; // Amount of time elapsed after last audio sample above threshold.
    private int lastSamplePosition = 0; // Last sample position in the audio clip.
    private int currentPosition; // Current sample position in the audio clip.
    private int sampleDiff; // Difference between current and last sample position.
    private bool isRecording; // Flag to check if we have detected a loud enough noise and can start "recording" for ASR.
    private WebRtcVad VAD; // Voice activity detector
    private int messagesSent = 0;

    private void Start()
    {
        // WebRTC initialization if needed
        VAD = new WebRtcVad();
        VAD.OperatingMode = OperatingMode.Aggressive;

        recognizedSpeech = "";
        inputText.text = "Initializing...";
        isRecording = false;

        if (Microphone.devices.Length > 0)
        {
            micDevice = Microphone.devices[0];
            Debug.Log("Using microphone: " + micDevice);
        }
        else
        {
            Debug.LogError("No microphone detected!");
        }
            
        /*
        foreach (var device in Microphone.devices)
        {
            Debug.Log("Microphone available: " + device);
        }
        */

        clip = Microphone.Start(micDevice, true, 10, sampleRate);
        while (Microphone.GetPosition(micDevice) <= 0) { } // Controls latency. 0 means no latency.
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
        inputText.text = "Listening...";
        
    }

    private void Update() 
    {   
        // If the microphone is not recording, return
        if (clip == null || !Microphone.IsRecording(micDevice)) 
        {
            return;
        }

        // Get the current recording position of the microphone
        currentPosition = Microphone.GetPosition(micDevice);
        sampleDiff = currentPosition - lastSamplePosition;

        if (sampleDiff < 0)
        {
            // Wrap around to the beginning of the clip if looping
            sampleDiff += clip.samples;
        }

        if (sampleDiff > 0)
        {
            float[] samples = new float[sampleDiff * clip.channels];
            clip.GetData(samples, lastSamplePosition);
            lastSamplePosition = currentPosition;

            // Check if the audio samples are above the threshold
            ProcessSamples(samples);

        }

    }

    private void ProcessSamples(float[] samples)
    {   
        byte[] pcmData = ConvertToPCM16(samples); // Convert Unity's float samples to 16-bit PCM
        if (VAD.HasSpeech(pcmData, SampleRate.Is16kHz, FrameLength.Is10ms))
        {
            // Start recording if first valid sample detected
            if (!isRecording)
            {
                Debug.Log("Speech Started");
                isRecording = true;
                silenceTimer = 0f;
            }
            speechBuffer.AddRange(samples);
        }
        else if (isRecording)
        {
            
            //speechBuffer.AddRange(samples);
            silenceTimer += Time.deltaTime;

            if (silenceTimer >= silenceDuration)
            {
                Debug.Log("Speech Ended, processing segment.");
                isRecording = false;
                silenceTimer = 0f;
                wavData = EncodeAsWAV(speechBuffer.ToArray(), clip.frequency, clip.channels);
                inputText.text = wavData[0].ToString();
                Debug.Log(wavData[0]);
                speechBuffer.Clear();
                // SendRecording();

            }
        }
        
        
        /* Old Code
        foreach(var sample in samples)
        {   
            // if we get a sample above the threshold add it to the speech buffer
            if (Mathf.Abs(sample) > loudnessThreshold)
            {
                // Start recording if first valid sample detected
                if (!isRecording)
                {
                    Debug.Log("Speech Started");
                    isRecording = true;
                    silenceTimer = 0f;
                }
                speechBuffer.Add(sample);
            }
            else // if there is no noise or silence only add it to the speech buffer if we are already recording
            {
                if (isRecording)
                {
                    speechBuffer.Add(sample);
                    silenceTimer += Time.deltaTime;

                    if (silenceTimer >= silenceDuration)
                    {
                        Debug.Log("Speech Ended, processing segment.");
                        isRecording = false;
                        silenceTimer = 0f;
                        wavData = EncodeAsWAV(speechBuffer.ToArray(), clip.frequency, clip.channels);
                        inputText.text = wavData.ToString();
                        Debug.Log(wavData);
                        speechBuffer.Clear();
                        // SendRecording();

                    }
                }
                
                

            }
                
        }
        */
    }


    /*
    private void StartRecording()
    {
        if (string.IsNullOrEmpty(micDevice))
        {
            Debug.LogError("No microphone available!");
            return;
        }

        inputText.color = Color.white;
        inputText.text = "Recording...";
        startButton.interactable = false;
        stopButton.interactable = true;

        clip = Microphone.Start(micDevice, false, 10, 44100);
        audioSource.clip = clip;
        audioSource.loop = true;
        recording = true;
        
        while (!(Microphone.GetPosition(micDevice) > 0)){}
        // Now that mic has data, start playback
        audioSource.Play();
       
    }
    */

    /*
    private void StopRecording()
    {
       
        var position = Microphone.GetPosition(micDevice);
        Microphone.End(micDevice);
        audioSource.Stop();
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        wavData = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        
        SendRecording();
    }

    */

    private void SendRecording() {
        messagesSent += 1;
        Debug.Log(messagesSent);
        inputText.color = Color.yellow;
        inputText.text = "Sending...";
        stopButton.interactable = false;
        HuggingFaceAPI.AutomaticSpeechRecognition(wavData, response => {
            inputText.color = Color.white;
            inputText.text = response;
            recognizedSpeech = response;
            voiceAction.ManipulateObject(recognizedSpeech);
            startButton.interactable = true;
        }, error => {
            inputText.color = Color.red;
            inputText.text = error;
            recognizedSpeech = "error";
            startButton.interactable = true;
        });
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels) 
    {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2)) {
            using (var writer = new BinaryWriter(memoryStream)) {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples) {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }
    
    private byte[] ConvertToPCM16(float[] samples)
    {
        short[] intData = new short[samples.Length];

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * short.MaxValue); // Convert float (-1 to 1) → short (-32768 to 32767)
        }

        byte[] bytes = new byte[intData.Length * 2]; // Each short (16-bit) needs 2 bytes
        Buffer.BlockCopy(intData, 0, bytes, 0, bytes.Length);
        return bytes;
    }


    void OnDestroy()
    {
        if (Microphone.IsRecording(micDevice))
        {
            Microphone.End(micDevice);
        }
    }

    public string GetRecognizedSpeech()
    {
        return recognizedSpeech;
    }

}
