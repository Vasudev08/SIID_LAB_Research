using HuggingFace.API;
using UnityEngine;

public class SentenceSimilarity : MonoBehaviour
{
    public void CompareInput(string source, string[] actions) {

        HuggingFaceAPI.SentenceSimilarity(source, 
        success_value =>{

        }, 
        error => {

        },
        actions);
    }
}
