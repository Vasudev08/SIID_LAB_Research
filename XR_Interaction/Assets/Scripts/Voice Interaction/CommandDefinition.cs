using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

/// <summary>
/// Holds data about a possible command (e.g. "rotate", "scale", "move"),
/// any synonyms, and the associated Action that performs it.
/// </summary>
[Serializable]
public class CommandDefinition
{
    public string referencePhrase;
    public List<string> synonyms;
    public Viewpoint targetViewpoint;
    public Action<Viewpoint> invokeFunction;
}
