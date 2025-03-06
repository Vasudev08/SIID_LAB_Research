using System;
using System.Collections.Generic;
using HuggingFace.API;
using Unity.Mathematics;
using UnityEngine;

public class SentenceSimilarity : MonoBehaviour
{
    [Header("Voice Input Commands")]
    public List<string> inputs = new List<string>();
    [Range(0, 1)]
    public float similarityThreshold = 0.6f;
    public VoiceAction voiceAction;

    private string errorOutput = "";

    public void CompareInput(string source) {

        HuggingFaceAPI.SentenceSimilarity(source, 
        success_values =>{
            Debug.Log("Success!");
            string vals = "";
            foreach (var val in success_values)
            {
                vals += val + " ";
            }
            Debug.Log(vals);
            FindStrongestMatch(success_values);
        }, 
        error => {
            errorOutput = error;
            Debug.Log("Error! " + errorOutput);

        },
        inputs.ToArray());
    }

    public void FindStrongestMatch(float[] similarity_scores)
    {
        // Check just in case
        if (similarity_scores.Length !=  inputs.Count)
        {
            Debug.LogError("Number of input commands and similarity rankings do not match!!");
            return;
        }

        float current_max = float.NegativeInfinity;
        string most_similar_command = "";
        for (int i = 0; i < similarity_scores.Length; i++)
        {
            if(similarity_scores[i] > current_max)
            {
                current_max = similarity_scores[i];
                most_similar_command = inputs[i];
            }
        }

        if (current_max > similarityThreshold)
        {
            // Do the command
            voiceAction.ManipulateObject(most_similar_command);
            Debug.Log("Successfully performed action.");
        }
        else
        {
            Debug.LogWarning("Unrecognizable command. Not similar enough to any known commands.");
        }
    }
}
