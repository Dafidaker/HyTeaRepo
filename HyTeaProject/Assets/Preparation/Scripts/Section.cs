using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Topic Section", menuName = "Presentation/Topic Section", order = 53)]
public class Section : ScriptableObject
{
    [SerializeField] private string SectionTitle;
    [SerializeField] private List<FullSlide> SlidesRequired;
    [SerializeField] private Color ImageColor;

    public List<FullSlide> GetSlidesRequired()
    {
        return SlidesRequired;
    }

    public string GetSectionTitle()
    {
        return SectionTitle;
    }

    public Color GetColor()
    {
        return ImageColor;
    }
    
}
