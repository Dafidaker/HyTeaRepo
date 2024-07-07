using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Topic Section", menuName = "Presentation/Topic Section", order = 53)]
public class Section : ScriptableObject
{
    [SerializeField] private string SectionTitle;
    [SerializeField] private List<GameObject> SlidesRequired;
    [SerializeField] private Color ImageColor;

    public List<GameObject> GetSlidesRequired()
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
