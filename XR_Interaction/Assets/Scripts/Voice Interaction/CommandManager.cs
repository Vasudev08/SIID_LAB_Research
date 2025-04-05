using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    [Description("Known Commands")]
    public List<CommandDefinition> commandDefinitions;
    public SentenceSimilarity sentenceSimilarity;
    public ViewpointTransitionController vtc;

    private List<string> knownCommands = new List<string>(); // All known command phrases including synonyms written in the inspector.
    private Dictionary<string, CommandDefinition> stringToCommand = new Dictionary<string, CommandDefinition>(); // Mapping command phrases and synonyms to a command in case that is the best match.

    void Awake()
    {
        foreach (var command in commandDefinitions)
        {
            command.invokeFunction = vtc.TransitionToViewpoint;
            knownCommands.Add(command.referencePhrase);
            if (!stringToCommand.ContainsKey(command.referencePhrase))
            {
                stringToCommand.Add(command.referencePhrase, command);
                foreach (var command_synonym in command.synonyms)
                {
                    knownCommands.Add(command_synonym);
                    if(!stringToCommand.ContainsKey(command_synonym))
                    {
                        stringToCommand.Add(command_synonym, command);
                    }
                    else
                    {
                        Debug.LogWarning(String.Format("You have duplicates of the same synonym: {0}! Please fix, each command must be unique.", command_synonym));
                    }
                    
                }
            }
            else
            {
                Debug.LogWarning(String.Format("You have duplicates of the same command: {0}! Please fix, each command must be unique.", command.referencePhrase));
            }
        }

    }

    /// <summary>
    /// Attempt to match the voiceInput to a known command. Because
    /// we call an async method (CompareInput), we provide a callback
    /// that is invoked once a match is found.
    /// </summary>
    public void UnderstoodCommand(string voiceInput, Action<bool, CommandDefinition> onComplete)
    {
        
        sentenceSimilarity.CompareInput(voiceInput, knownCommands.ToArray(), matched_string =>{
            Debug.Log(voiceInput);
            if (string.IsNullOrEmpty(matched_string))
            {
                Debug.Log(String.Format("Could not find a matching command from input: {0}.", voiceInput));
                onComplete.Invoke(false, null);
                return;
            }
            // Successfully found a matching command
            CommandDefinition matched_command = stringToCommand[matched_string];
            
            // Return success + matchedCommand
            onComplete.Invoke(true, matched_command);
        });
        
    }
    
}
