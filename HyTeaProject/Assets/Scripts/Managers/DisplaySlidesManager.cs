using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySlidesManager : MonoBehaviour
{
    [SerializeField] private List<Sprite> LevelOneSlides;
    [SerializeField, TextArea] private List<string> Notes;
    [SerializeField] private Image TabletScreen;
    [SerializeField] private TextMeshProUGUI NoteDisplay;
    [SerializeField] private Scrollbar VerticalScrollbar;
    
    private int _currentSlideIndex;

    private void Start()
    {
        _currentSlideIndex = 0;
        TabletScreen.sprite = LevelOneSlides[_currentSlideIndex];
        NoteDisplay.text = Notes[_currentSlideIndex];
    }   

    public void CycleSlides(int num)
    {
        _currentSlideIndex += num;
        if (_currentSlideIndex >= LevelOneSlides.Count) _currentSlideIndex = 0;
        if (_currentSlideIndex < 0) _currentSlideIndex = LevelOneSlides.Count - 1;
        TabletScreen.sprite = LevelOneSlides[_currentSlideIndex];
        NoteDisplay.text = Notes[_currentSlideIndex];

        VerticalScrollbar.value = 1;
    }
}
