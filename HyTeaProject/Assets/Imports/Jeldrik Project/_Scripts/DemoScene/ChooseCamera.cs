using System.Collections.Generic;
using Mediapipe.Unity.Sample;
using TMPro;
using UnityEngine;

public class ChooseCamera : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _selectionDropDown;
    private Solution _solution;

    private void OnEnable()
    {
        _selectionDropDown.onValueChanged.AddListener(delegate { ChangeWebCam(); });
    }

    private void Start()
    {
        _solution = FindObjectOfType<Solution>();
        if (! _solution)
        {
            Debug.LogError("No solution found ! Can't fetch needed reference for Choose Camera DropDown");
        }
        
        // TODO: make async and wait for camera options to be available
        Invoke(nameof(AddAvailableCamOptions), 1f);
    }

    private void AddAvailableCamOptions()
    {
        var imageSource = ImageSourceProvider.ImageSource;
        
        _selectionDropDown.ClearOptions();
        
        var sourceNames = imageSource.sourceCandidateNames;

        if (sourceNames == null)
        {
            _selectionDropDown.enabled = false;
            return;
        }

        var options = new List<string>(sourceNames);
        _selectionDropDown.AddOptions(options);
    }

    private void ChangeWebCam()
    {
        var imageSource = ImageSourceProvider.ImageSource;
        imageSource.SelectSource(_selectionDropDown.value);
        _solution.Play();
    }
}
