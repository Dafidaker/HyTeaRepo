using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MicrophoneSelector : MonoBehaviour
{
    public TMP_Dropdown sourceDropdown;
    public int chooseDeviceIndex;

    public static UnityAction<int> OnMicrophoneChanged;
    
    private void Start()
    {
        PopulateSourceDownDrop();   
    }

    private void PopulateSourceDownDrop()
    {
        var options = new List<TMP_Dropdown.OptionData>();

        foreach (var microphone in Microphone.devices)
        {
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(microphone, null);
            
            options.Add(optionData);
        }

        sourceDropdown.options = options;
    }


    public void ChooseMicrophone(int optionIndex)
    {
        chooseDeviceIndex = optionIndex;
        OnMicrophoneChanged.Invoke(chooseDeviceIndex);
    }
}
