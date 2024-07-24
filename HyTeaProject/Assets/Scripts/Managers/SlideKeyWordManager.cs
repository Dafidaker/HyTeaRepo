using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlideKeyWordManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> Slides;
    [SerializeField, TextArea] private List<string> Notes;
    [SerializeField] private Transform SlideParent;
    
    [SerializeField] private TextMeshProUGUI NoteDisplay;
    
    private List<GameObject> _slideGameObjects;
    
    private int _currentSlideIndex;

    private void OnEnable()
    {
        EventManager.PresentationHasEnded.AddListener(HandlePresentationEnd);
    }

    private void HandlePresentationEnd()
    {
        _slideGameObjects[^1].GetComponent<SlideKeyWordList>().KeyWords.endTime = Time.time;

        List<TimedWordList> timedWordLists = new List<TimedWordList>();

        foreach (var gameObject in _slideGameObjects)
        {
            timedWordLists.Add(gameObject.GetComponent<SlideKeyWordList>().KeyWords);
        }

        PresentationManager.Instance.GetPresentationEvaluation().timedWordLists = timedWordLists;
    }

    private void OnDisable()
    {
        EventManager.PresentationHasEnded.AddListener(HandlePresentationEnd);
    }

    private void Start()
    {
        _currentSlideIndex = 0;
        _slideGameObjects = new List<GameObject>();
        
        for (int i = 0; i < Slides.Count; i++)
        {
            var go = Instantiate(Slides[i], SlideParent);
            _slideGameObjects.Add(go);
        }

        for (int i = 1; i < _slideGameObjects.Count ; i++)
        {
            _slideGameObjects[i].SetActive(false);
        }

        _slideGameObjects[_currentSlideIndex].GetComponent<SlideKeyWordList>().KeyWords.startTime = Time.time;
        
        
        NoteDisplay.text = Notes[_currentSlideIndex];
    }

    private void MoveSlides()
    {
        if (_currentSlideIndex < _slideGameObjects.Count - 1)
        {
            _slideGameObjects[_currentSlideIndex].GetComponent<SlideKeyWordList>().KeyWords.endTime = Time.time;
            _slideGameObjects[_currentSlideIndex].SetActive(false);
            _currentSlideIndex++;
            _slideGameObjects[_currentSlideIndex].SetActive(true);
            _slideGameObjects[_currentSlideIndex].GetComponent<SlideKeyWordList>().KeyWords.startTime = Time.time;
            
            NoteDisplay.text = Notes[_currentSlideIndex];
        }
        else
        {
            //_currentSlideIndex = _slideGameObjects.Count - 1;
            Debug.Log("END OF SLIDES");
        }
    }

    public List<GameObject> GetSlideKeyWords()
    {
        return _slideGameObjects;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) && GameManager.Instance.GameState == GameState.Presentation)
        {
            MoveSlides();
        }
    }
}
