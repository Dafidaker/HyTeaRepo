using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Topic", menuName = "Presentation/Topic", order = 51)]
public class Topic : ScriptableObject
{
    public string Title;
    [SerializeField] private GameObject[] AvailableSlides;
    [SerializeField] private GameObject[] AvailableOptionsForSlides;

    public GameObject[] GetAvailableSlides()
    {
        return AvailableSlides;
    }

    public GameObject[] GetAvailableOptionsForSlides()
    {
        return AvailableOptionsForSlides;
    }
}
