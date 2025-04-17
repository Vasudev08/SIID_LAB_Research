using UnityEngine;
using UnityEngine.UI;
using System;

public class VoiceRecognition : MonoBehaviour
{   
    [Header("UI")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    public TMPro.TextMeshProUGUI inputText;
    public TMPro.TextMeshProUGUI debugText;
    [SerializeField] private AudioSource audioSource;

    [Header("Voice Interaction")]
    public RunWhisper runWhisper;
    public SentenceSimilarity sentenceSimilarity;
    public VoiceAction voiceAction;
    public CommandManager commandManager;
    
    [Header("Voice Detection")]
    public float energyThreshold = 0.01f;  // Energy required to detect speech
    public float silenceDuration = 1f; // Seconds of silence to consider the end of  speech.

    [Header("Viewpoint Transition")]
    public ViewpointTransitionController viewpointTransitionController;
    public string zoomOutCommandKeyPhrase;


    [NonSerialized] public bool inTransition = false;

    #region Microphone Input Variables
    private string micDevice;
    private int sampleRate = 16000;
    private AudioClip clip;
    private byte[] wavData;
    private bool isListening = false; // check if user has started voice interaction
    #endregion
    
    
    #region Voice Detection Variables
    private float silenceTimer = 0f; // Amount of time elapsed after last audio sample above threshold.
    private int lastSamplePosition = 0; // Last sample position in the audio clip.
    private int currentPosition; // Current sample position in the audio clip.
    private int sampleDiff; // Difference between current and last sample position.
    private bool isRecording; // Flag to check if we have detected a loud enough noise and can start "recording" for ASR.
    private float[] sampleBuffer; // Preallocated buffer for reading from the AudioClip.
    private CircularBuffer circularBuffer;
    private string recognizedSpeech;
    #endregion

    
    void Awake()
    {
        // Preallocate a reusable sample buffer.
        // assume a maximum chunk size (for example, 1024 samples per channel (8 = max number of channels)).
        sampleBuffer = new float[1024 * 8];

        // allocate a circular buffer for speech data.
        // allocate enough space for 10 seconds of audio.
        int maxSpeechSamples = sampleRate * 8 * 10;
        circularBuffer = new CircularBuffer(maxSpeechSamples);
    }
    private void Start()
    {
        recognizedSpeech = "";
        inputText.text = "Press the Start button to start voice interaction.";
        isRecording = false;
        
        // Debug block checking if there are enough microphone devices
        if (Microphone.devices.Length > 0)
        {
            micDevice = Microphone.devices[0];
            debugText.text = micDevice;
            Debug.Log("Using microphone: " + micDevice);
        }
        else
        {
            Debug.LogError("No microphone detected!");
            return;
        }
        
        audioSource.loop = true;

        
    }
   

    private void Update() 
    {   
        // If the microphone is not recording or we are in a viewpoint transition, return
        if (clip == null || !Microphone.IsRecording(micDevice) || !isListening || inTransition) 
        {
            return;
        }

        // Get the current recording position of the microphone
        currentPosition = Microphone.GetPosition(micDevice);
        sampleDiff = currentPosition - lastSamplePosition;

        if (sampleDiff < 0)
        {
            // Wrap around to the beginning of the clip if looping
            // means that the clip sample has looped making the current position less than
            // just add the size of the clip samples to get the linear sequenec
            sampleDiff += clip.samples;
        }

        if (sampleDiff > 0)
        {
            int sampleCount = clip.channels * sampleDiff;
            if (sampleCount > sampleBuffer.Length)
            {
                sampleCount = sampleBuffer.Length;
            }
            clip.GetData(sampleBuffer, lastSamplePosition);
            lastSamplePosition = currentPosition;

            ProcessSamples(sampleBuffer, sampleCount);
        }
        
    }

    private void ProcessSamples(float[] samples, int sampleCount)
    {   
        if (DetectedSpeech(samples))
        {
            // Start recording if first valid sample detected
            if (!isRecording)
            {
                // Debug.Log("Speech Started");
                isRecording = true;
                silenceTimer = 0f;
            }
            circularBuffer.Write(samples, sampleCount);
        }
        else if (isRecording)
        {
            silenceTimer += Time.deltaTime;

            if (silenceTimer >= silenceDuration)
            {
                // Debug.Log("Speech Ended, processing segment.");
                isRecording = false;
                silenceTimer = 0f;
                float[] speech_data = circularBuffer.GetData();
                circularBuffer.Clear();
                runWhisper.TranscribeAudioLocally(speech_data);
                
                // Required to send the data using the Whisper Tiny API in Huggingface
                // Data is required to be in WAV format
                // wavData = EncodeAsWAV(speech_data, clip.frequency, clip.channels);
                // SendRecording();
            }
        }
    }

    public bool DetectedSpeech(float[] samples)
    {
        float sum = 0f;
        foreach (float s in samples)
            sum += s * s;

        float rms = Mathf.Sqrt(sum / samples.Length);
        return rms > energyThreshold;
    }

    
    public void StartRecording()
    {
        // **** Starting Mic Input *****
        clip = Microphone.Start(micDevice, true, 10, sampleRate);
        while (Microphone.GetPosition(micDevice) <= 0) {  } // Controls latency. 0 means no latency.
        audioSource.clip = clip;
        
        // audioSource.Play();
        inputText.text = "Listening...";
        isListening = true;
       
    }
    
    public void StopRecording()
    {
        inputText.text = "Press the Start button to start voice interaction.";
        Microphone.End(micDevice);
        // audioSource.Stop();
        isListening = false;
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

    public void SetRecognizedSpeech(string transcribed_speech)
    {
        recognizedSpeech = transcribed_speech;
    }

    public void OnTranscriptionSuccess()
    {
        debugText.text = "Successful Transcription";
        commandManager.UnderstoodCommand(recognizedSpeech, (success, matched_command) =>{
            if (!success || matched_command == null)
            {
                // No match found
                return;
            }

            // We have a matched command. Let's invoke it:
            if (matched_command.invokeFunction != null)
            {
                if (matched_command.targetViewpoint)
                    Debug.Log(matched_command.targetViewpoint.name);
                if (matched_command.referencePhrase == zoomOutCommandKeyPhrase)
                {
                    Viewpoint current_viewpoint = viewpointTransitionController.userViewpoint;
                    
                    if (current_viewpoint.parent)
                        matched_command.targetViewpoint = current_viewpoint.parent;
                    else
                        matched_command.targetViewpoint = current_viewpoint;

                }
                matched_command.invokeFunction.Invoke(matched_command.targetViewpoint);
            }
            else
            {
                Debug.LogWarning($"Matched command {matched_command.referencePhrase} has no action assigned!");
            }
        });
    }

}
