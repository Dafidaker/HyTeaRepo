using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideKeyWordManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> Slides;
    [SerializeField] private Transform SlideParent;

    private List<GameObject> _slideGameObjects;
    
    private int _currentSlideIndex;

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
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveSlides();
        }
    }
}
