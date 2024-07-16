using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Topic Section", menuName = "Presentation/Full Slide", order = 54)]
public class FullSlide : ScriptableObject
{
    public Section CorrespondingSection;
    public GameObject FullSlidePrefab;
    public string Title;
    [TextArea] public List<string> TextOptions;
    public List<GameObject> AvailableImages;
    public Sprite MiscOption;
}
