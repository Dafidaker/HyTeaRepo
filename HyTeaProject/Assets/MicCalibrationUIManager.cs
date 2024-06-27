using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MicCalibrationUIManager : MonoBehaviour
{
    [field: SerializeField] private TMP_Dropdown microphoneDropdown;
    [field: SerializeField] private Slider microphoneCalibration;

    private void Awake()
    {
        if (microphoneDropdown == null) { GetComponentInChildren<TMP_Dropdown>(); }
        if (microphoneCalibration == null) { GetComponentInChildren<Slider>(); }
    }

    private void OnEnable()
    {
        EventManager.StartedVolumeAnalysis.AddListener(DisableElements);
        EventManager.FinishedVolumeAnalysis.AddListener(EnableElements);
    }

    private void OnDisable()
    {
        EventManager.StartedVolumeAnalysis.RemoveListener(DisableElements);
        EventManager.FinishedVolumeAnalysis.RemoveListener(EnableElements);
    }

    private void DisableElements()
    {
        microphoneDropdown.interactable = false;
        microphoneCalibration.interactable = false;
    }
    
    private void EnableElements()
    {
        microphoneDropdown.interactable = true;
        microphoneCalibration.interactable = true;
    }
}
