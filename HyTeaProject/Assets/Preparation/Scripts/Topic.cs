using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Topic", menuName = "Presentation/Topic", order = 51)]
public class Topic : ScriptableObject
{
    public string Title;
    [SerializeField] private List<Slide> AvailableSlideObjects;
    private List<GameObject> _previewSlides;
    private List<GameObject> _fullSlides;

    public List<Slide> GetAllSlides()
    {
        return AvailableSlideObjects;
    }
    
    public List<GameObject> GetAllPreviewSlides()
    {
        _previewSlides = new List<GameObject>();
        for (int i = 0; i < AvailableSlideObjects.Count; i++)
        {
            _previewSlides.Add(AvailableSlideObjects[i].GetSlidePreview());
        }
        return _previewSlides;
    }
    
    public List<GameObject> GetAllFullSlides()
    {
        _fullSlides = new List<GameObject>();
        for (int i = 0; i < AvailableSlideObjects.Count; i++)
        {
            _fullSlides.Add(AvailableSlideObjects[i].GetFullSlide());
        }
        return _fullSlides;
    }
}
