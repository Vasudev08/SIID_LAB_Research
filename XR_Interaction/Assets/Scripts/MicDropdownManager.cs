using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MicDropdownManager : MonoBehaviour
{
    
    [SerializeField] TMPro.TMP_Dropdown micDropdown;
    [SerializeField] private MicSelectionManager micSelector;

    // Start is called before the first frame update
    void Start()
    {
        UpdateMicDropdown();
    }

    private void UpdateMicDropdown()
    {
        micDropdown.ClearOptions();
        micDropdown.AddOptions(Microphone.devices.ToList());

        micDropdown.onValueChanged.AddListener(delegate {
            micSelector.StartMic(micDropdown.options[micDropdown.value].text);
        });
    }
}
