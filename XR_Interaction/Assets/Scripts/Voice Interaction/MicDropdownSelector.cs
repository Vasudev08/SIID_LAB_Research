using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MicDropdownSelector : MonoBehaviour
{
    public TMP_Dropdown micDropdown;
    public TextMeshProUGUI selectedMicLabel;

    private List<string> availableMics = new List<string>();
    private string selectedMic = null;
    void Start()
    {
        PopulateMicDropdown();
        micDropdown.onValueChanged.AddListener(OnMicSelected);
    }

    void PopulateMicDropdown()
    {
        micDropdown.ClearOptions();
        availableMics.Clear();

        foreach (var mic in Microphone.devices)
        {
            availableMics.Add(mic);

        }

        if (availableMics.Count > 0)
        {

            micDropdown.AddOptions(availableMics);
            micDropdown.value = 0;
            selectedMic = availableMics[0];
            UpdateLabel();
        }
        else
        {
            selectedMicLabel.text = "No mics found";
            Debug.LogWarning("No Microphones detected.");
        }
    }

    void OnMicSelected(int index)
    {
        selectedMic = availableMics[index];
        UpdateLabel();
        Debug.Log("Selected Mic: " + selectedMic);
    }


    void UpdateLabel()
        {
            if (selectedMicLabel != null)
                selectedMicLabel.text = "Selected Mic: " + selectedMic;
        }
    }
