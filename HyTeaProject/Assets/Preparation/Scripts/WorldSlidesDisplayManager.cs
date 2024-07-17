using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSlidesDisplayManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> CompletedSlides;
    [SerializeField] private Transform SlideParent;

    private List<GameObject> _slideGameObjects;
    private int _indexOfCurrentSlide;

    private void SetCompletedSlides(List<GameObject> slides)
    {
        CompletedSlides = slides;
        DisplaySlidesInWorld();
    }

    private void DisplaySlidesInWorld()
    {
        for (int i = 0; i < CompletedSlides.Count; i++)
        {
            var go = Instantiate(CompletedSlides[i], SlideParent);
            Destroy(go.transform.Find("OptionImageSlot").GetComponent<Button>());
            Destroy(go.transform.Find("OptionTextSlot").GetComponent<Button>());

            if (!go.GetComponent<FullSlideManager>().HasImageSelected)
            {
                go.transform.Find("OptionImageSlot").gameObject.SetActive(false);
            }
            
            _slideGameObjects.Add(go);
        }

        _indexOfCurrentSlide = 0;
    }

    private void Update()
    {
        
        //THIS IS TO BE REMOVED IF WE DO A ACTUAL INPUT SYSTEM
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _slideGameObjects[_indexOfCurrentSlide].SetActive(false);
            _indexOfCurrentSlide += 1;
            if (_indexOfCurrentSlide < 0) _indexOfCurrentSlide = _slideGameObjects.Count - 1;
            if (_indexOfCurrentSlide >= _slideGameObjects.Count) _indexOfCurrentSlide = 0;
            _slideGameObjects[_indexOfCurrentSlide].SetActive(true);
            
        }
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _slideGameObjects[_indexOfCurrentSlide].SetActive(false);
            _indexOfCurrentSlide -= 1;
            if (_indexOfCurrentSlide < 0) _indexOfCurrentSlide = _slideGameObjects.Count - 1;
            if (_indexOfCurrentSlide >= _slideGameObjects.Count) _indexOfCurrentSlide = 0;
            _slideGameObjects[_indexOfCurrentSlide].SetActive(true);
            
        }
    }

    private void OnEnable()
    {
        EventManager.GetCompletedSlides.AddListener(SetCompletedSlides);
        _slideGameObjects = new List<GameObject>();
    }

    private void OnDisable()
    {
        EventManager.GetCompletedSlides.RemoveListener(SetCompletedSlides);
    }
}
