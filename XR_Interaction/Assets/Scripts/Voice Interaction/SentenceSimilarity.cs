using System;
using System.Collections.Generic;
using System.ComponentModel;
using HuggingFace.API;
using Unity.Mathematics;
using UnityEngine;

public class SentenceSimilarity : MonoBehaviour
{
    [Description("Similarity threshold that the most similar command must be above to be recognized as valid.")]
    [Range(0, 1)]
    public float similarityThreshold = 0.6f;

    private string errorOutput = "";


    /// <summary>
    /// Compare the transcribed voice input to a collection of known commands/speech.
    /// Use a callback or "completion handler" to return the matched command when the async call finishes.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="context"></param>
    /// <param name="onComplete"></param>
    public void CompareInput(string source, string[] context, Action<string> onComplete) {

        HuggingFaceAPI.SentenceSimilarity(source, 
        success_values =>{
            Debug.Log("Successfully called the Sentence Similarity API!");
            string vals = "";
            foreach (var val in success_values)
            {
                vals += val + " ";
            }
            
            string strongest_match = FindStrongestMatch(success_values, context);
            onComplete.Invoke(strongest_match);
        }, 
        error => {
            errorOutput = error;
            Debug.Log("Error calling the Sentence Similarity API! " + errorOutput);
            onComplete.Invoke(null);
        },
        context);
    }

    public string FindStrongestMatch(float[] similarity_scores, string[] context)
    {
        // Check just in case
        if (similarity_scores.Length != context.Length)
        {
            Debug.LogError("Number of input commands and similarity rankings do not match!!");
            return "";
        }

        float current_max = float.NegativeInfinity;
        string most_similar_command = "";
        for (int i = 0; i < similarity_scores.Length; i++)
        {
            if(similarity_scores[i] > current_max)
            {
                current_max = similarity_scores[i];
                most_similar_command = context[i];
            }
        }

        if (current_max > similarityThreshold)
        {
            // Do the command
            Debug.Log($"Recognized command {most_similar_command}.");
            return most_similar_command;
        }
        else
        {
            Debug.LogWarning("Unrecognizable command. Not similar enough to any known commands.");
            return null;
        }
    }
}
