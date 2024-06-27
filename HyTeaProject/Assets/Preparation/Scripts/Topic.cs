using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Topic", menuName = "Presentation/Topic", order = 51)]
public class Topic : ScriptableObject
{
    public string Title;
    [SerializeField] private List<GameObject> AvailableSlides;
    [SerializeField] private List<GameObject> AvailableOptionsForSlides;
    
    public List<GameObject> GetAvailableSlides()
    {
        return AvailableSlides;
    }

    public List<GameObject> GetAvailableOptionsForSlides()
    {
        return AvailableOptionsForSlides;
    }

    private void Awake()
    {
        AvailableSlides = new List<GameObject>();
        AvailableOptionsForSlides = new List<GameObject>();
    }
}
