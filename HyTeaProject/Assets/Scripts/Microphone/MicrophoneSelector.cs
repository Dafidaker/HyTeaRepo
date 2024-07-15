using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MicrophoneSelector : MonoBehaviour
{
    public TMP_Dropdown sourceDropdown;
    private int _chooseDeviceIndex;
    
    private void Awake()
    {
        _chooseDeviceIndex = 0;
    }

    private void Start()
    {
        PopulateSourceDownDrop();   
    }

    private void OnEnable()
    {
        EventManager.PopulateMicrophoneList.AddListener(PopulateSourceDownDrop);
    }

    private void OnDisable()
    {
        EventManager.PopulateMicrophoneList.RemoveListener(PopulateSourceDownDrop);
    }

    private void PopulateSourceDownDrop()
    {
        sourceDropdown.ClearOptions();
        
        var options = new List<TMP_Dropdown.OptionData>();

        if(Microphone.devices.Length == 0)
        {
            AudioSettings.Reset(AudioSettings.GetConfiguration());
        }
        
        foreach (var microphone in Microphone.devices)
        {
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(microphone, null);
            
            options.Add(optionData);
        }

        sourceDropdown.options = options;

        ChooseMicrophone(0);
    }


    public void ChooseMicrophone(int optionIndex)
    {
        _chooseDeviceIndex = optionIndex;
        EventManager.DifferentMicrophoneSelectedInUI.Invoke(_chooseDeviceIndex);
    }
}
