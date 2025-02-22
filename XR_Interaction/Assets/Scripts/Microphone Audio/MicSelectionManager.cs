using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class MicSelectionManager : MonoBehaviour
{
    string selectedMic = "";
    [SerializeField] AudioSource audioSource;
    [SerializeField] DisplayAudioData displayAudioData;

    public void StartMic(string micName)
    {
        audioSource.clip = Microphone.Start(micName, true, 10, 44100);
        audioSource.loop = true;
        selectedMic = micName;

        displayAudioData.micDevice = micName;

        displayAudioData.micClip = audioSource.clip;
        while (!(Microphone.GetPosition(micName) > 0)) { } // Wait for mic to start
        audioSource.Play();
    }

    private void OnDestroy()
    {
        if (Microphone.IsRecording(selectedMic))
        {
            Microphone.End(selectedMic);
        }
    }
}
