using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HuggingFace.API;
using UnityEngine.UI;
using System;
using System.IO;

public class VoiceRecognition : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private TMPro.TextMeshProUGUI inputText;
    [SerializeField] private AudioSource audioSource;
    public VoiceAction voiceAction;

    private string micDevice;
    private AudioClip clip;
    private byte[] bytes;
    private bool recording;
    private string recognizedSpeech;
    

    private void Start()
    {
        recognizedSpeech = "";
        startButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);
        stopButton.interactable = false;

        if (Microphone.devices.Length > 0)
            micDevice = Microphone.devices[0];
        else
            Debug.LogError("No microphone detected!");
        
        foreach (var device in Microphone.devices)
        {
            Debug.Log("Microphone available: " + device);
        }

        micDevice = Microphone.devices[0];
        
    }

    private void Update() 
    {
        if (recording && Microphone.GetPosition(micDevice) >= clip.samples) {
            StopRecording();
        }

        if (recording && Microphone.GetPosition(micDevice) <= 0)
        {
            Debug.LogWarning("Microphone position reset, restarting playback...");
            audioSource.Stop();
            audioSource.Play();
        }
        
    }


    
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

    private void StopRecording()
    {
       
        var position = Microphone.GetPosition(micDevice);
        Microphone.End(micDevice);
        audioSource.Stop();
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        
        SendRecording();
    }

    private void SendRecording() {
        inputText.color = Color.yellow;
        inputText.text = "Sending...";
        stopButton.interactable = false;
        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response => {
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
