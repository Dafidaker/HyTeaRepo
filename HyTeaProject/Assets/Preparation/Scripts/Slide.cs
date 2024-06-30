using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Slide", menuName = "Presentation/Slide", order = 52)]
public class Slide : ScriptableObject
{
    [SerializeField] private GameObject SlidePreviewPrefab;
    [SerializeField] private GameObject SlideFullPrefab;

    [SerializeField] private List<GameObject> AvailableOptions;

    public bool IsNotMovable;

    private GameObject _chosenOption;

    public GameObject GetSlidePreview()
    {
        return SlidePreviewPrefab;
    }

    public GameObject GetFullSlide()
    {
        return SlideFullPrefab;
    }

    public List<GameObject> GetAvailableOptions()
    {
        return AvailableOptions;
    }

    public GameObject GetChosenOption()
    {
        return _chosenOption;
    }

    public void SetChosenOption(GameObject chosen)
    {
        _chosenOption = chosen;
    }

    private void OnEnable()
    {
        _chosenOption = null;
    }
}
