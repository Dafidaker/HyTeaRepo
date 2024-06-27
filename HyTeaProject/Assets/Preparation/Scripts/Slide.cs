using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Slide", menuName = "Presentation/Slide", order = 52)]
public class Slide : ScriptableObject
{
    [SerializeField] private GameObject SlidePreviewPrefab;
    [SerializeField] private GameObject SlideFullPrefab;

    public GameObject GetSlidePreview()
    {
        return SlidePreviewPrefab;
    }

    public GameObject GetFullSlide()
    {
        return SlideFullPrefab;
    }
}
