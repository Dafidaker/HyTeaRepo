using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SlideBuildingManager : MonoBehaviour
{
    [Header("Topic Selection Section")]
    [SerializeField] private Topic SelectedTopic;
    [SerializeField] private List<Slide> AvailableSlides;
    [SerializeField] private TextMeshProUGUI CurrentTopicText;
    
    [Header("Slide Order Section")]
    [SerializeField] private Slide IntroSlideObj;
    [SerializeField] private Slide OutroSlideObj;
    [SerializeField] private GameObject SlidesParent;
    [SerializeField] private Transform StartingPoint;
    
    [Header("Slide Customization Section")] 
    [SerializeField] private Transform SlideDisplayPoint;
    [SerializeField] private Transform OptionStartPoint;
    
    private List<Slide> _slideObjInOrder;
    
    private int _indexOfHeldSlide;
    private int _indexOfTargetSlide;

    private int _indexOfCurrentFullSlide;
    private List<GameObject> _displayedSlides;
    
    [Header("Sections")]
    [SerializeField] private GameObject TopicSection;
    [SerializeField] private GameObject SlideOrderSection;
    [SerializeField] private GameObject OptionsSection;
    
    public void UpdateSelectedTopic(Topic topic)
    {
        SelectedTopic = topic;
        AvailableSlides = SelectedTopic.GetAllSlides();

        CurrentTopicText.text = SelectedTopic.Title;
        
    }

    public void MoveToSlideOrderSection()
    {
        if (SelectedTopic != null)
        {
            TopicSection.SetActive(false);
            SlideOrderSection.SetActive(true);
            DisplayPreviewSlides();
        }
        else
        {
            Debug.Log("NO TOPIC SELECTED");
        }
    }
    
    public void MoveToSlideCustomizationSection()
    {
        SlideOrderSection.SetActive(false);
        OptionsSection.SetActive(true);
        
        DisplayFullSlides();
    }

    public void ReturnToTopicSection()
    {
        TopicSection.SetActive(true);
        SlideOrderSection.SetActive(false);
        
        foreach (Transform child in SlidesParent.transform)
        {
            Destroy(child.gameObject);
        }
        
        SlidesParent.transform.DetachChildren();
    }

    public void ReturnToSlideOrderSection()
    {
        foreach (Transform child in SlideDisplayPoint.transform)
        {
            Destroy(child.gameObject);
        }
        
        SlideDisplayPoint.transform.DetachChildren();
        
        SlideOrderSection.SetActive(true);
        OptionsSection.SetActive(false);
        
        _displayedSlides.Clear();
    }

    public void CycleThroughDisplayedSlides(int increment)
    {
        _displayedSlides[_indexOfCurrentFullSlide].SetActive(false);
        _indexOfCurrentFullSlide += increment;
        if (_indexOfCurrentFullSlide < 0) _indexOfCurrentFullSlide = _displayedSlides.Count - 1;
        if (_indexOfCurrentFullSlide >= _displayedSlides.Count) _indexOfCurrentFullSlide = 0;
        _displayedSlides[_indexOfCurrentFullSlide].SetActive(true);
    }

    private void DisplayPreviewSlides() 
    {
        _slideObjInOrder = AvailableSlides;
        _slideObjInOrder = AddToBeginning(_slideObjInOrder, IntroSlideObj);
        _slideObjInOrder = AddToEnd(_slideObjInOrder, OutroSlideObj);

        float horizontalSpacing = IntroSlideObj.GetSlidePreview().GetComponent<RectTransform>().rect.width + 5 + Screen.width / 100;
        float verticalSpacing = IntroSlideObj.GetSlidePreview().GetComponent<RectTransform>().rect.height + 10 + Screen.height / 100;
        
        for (int i = 0; i < _slideObjInOrder.Count; i++)
        {
            int row = i / 4;
            int column = i % 4;
            var go = Instantiate(_slideObjInOrder[i].GetSlidePreview(), StartingPoint.position + new Vector3(column * horizontalSpacing, row * -verticalSpacing , 0), quaternion.identity, SlidesParent.transform);
            if (!go.GetComponent<DragAndDropSlideManager>()) continue;
            go.GetComponent<DragAndDropSlideManager>().SetIndex(i);
        }
        
        EventManager.GetNumberOfSiblings.Invoke(SlidesParent.transform.childCount);
    }

    private void DisplayFullSlides()
    {
        float horizontalSpacing = _slideObjInOrder[1].GetAvailableOptions()[0].GetComponent<RectTransform>().rect.width + 5 + Screen.width / 100;
        
        _displayedSlides.Clear();
        for (int i = 0; i < _slideObjInOrder.Count; i++)
        {
            if (_slideObjInOrder[i].IsNotMovable) continue;
            var go = Instantiate(_slideObjInOrder[i].GetFullSlide(), SlideDisplayPoint.position, quaternion.identity, SlideDisplayPoint.transform);
            _displayedSlides.Add(go);
            
            
            int optionCount = _slideObjInOrder[i].GetAvailableOptions().Count;

    
            RectTransform parentRectTransform = go.transform.Find("OptionDisplayStart").GetComponent<RectTransform>();
            float parentWidth = parentRectTransform.rect.width;


            float totalWidth = (optionCount - 1) * horizontalSpacing;


            Vector3 startPosition = parentRectTransform.position - new Vector3(totalWidth / 2, 0, 0);

            for (int j = 0; j < optionCount; j++)
            {
                
                Vector3 position = startPosition + new Vector3(j * horizontalSpacing, 0, 0);

                
                var opt = Instantiate(_slideObjInOrder[i].GetAvailableOptions()[j], position, quaternion.identity, parentRectTransform);
                opt.GetComponent<DragAndDropOptionManager>().Index = j;
            }
        }

        for (int k = 1; k < _displayedSlides.Count; k++)
        {
            _displayedSlides[k].SetActive(false);
        }
        _indexOfCurrentFullSlide = 0;
    }

    private void UpdateChosenOptionInSlide(int index)
    {
        int temp = _indexOfCurrentFullSlide;

        if (temp == 0) temp = 1;
        if (temp == _slideObjInOrder.Count - 1) temp = _slideObjInOrder.Count - 2;
        
        _slideObjInOrder[temp].SetChosenOption(_slideObjInOrder[temp].GetAvailableOptions()[index]);
    }
    
    private List<Slide> AddToBeginning(List<Slide> originalList, Slide newElement)
    {
        List<Slide> newList = new List<Slide>(originalList.Count + 1);
        newList.Add(newElement);
        newList.AddRange(originalList);
        return newList;
    }
    
    private List<Slide> AddToEnd(List<Slide> originalList, Slide newElement)
    {
        List<Slide> newList = new List<Slide>(originalList);
        newList.Add(newElement);
        return newList;
    }
    
    private void UpdateIndexOfHeldSlide(int index)
    {
        _indexOfHeldSlide = index;
        Debug.Log("Event triggered. Index of the held slide: " + index);
    }

    private void SwapSlidesOnDrop(int index1, int index2)
    {
        (_slideObjInOrder[index1], _slideObjInOrder[index2]) = (_slideObjInOrder[index2], _slideObjInOrder[index1]);
        
        foreach (Transform child in SlidesParent.transform)
        {
            Destroy(child.gameObject);
        }
        
        SlidesParent.transform.DetachChildren();
        
        float horizontalSpacing = IntroSlideObj.GetSlidePreview().GetComponent<RectTransform>().rect.width + 5 + Screen.width / 100;
        float verticalSpacing = IntroSlideObj.GetSlidePreview().GetComponent<RectTransform>().rect.height + 10 + Screen.height / 100;
        
        for (int i = 0; i < _slideObjInOrder.Count; i++)
        {
            int row = i / 4;
            int column = i % 4;
            var go = Instantiate(_slideObjInOrder[i].GetSlidePreview(), StartingPoint.position + new Vector3(column * horizontalSpacing, row * -verticalSpacing , 0), quaternion.identity, SlidesParent.transform);            
            if (!go.GetComponent<DragAndDropSlideManager>()) continue;
            go.GetComponent<DragAndDropSlideManager>().SetIndex(i);
        }
        
        EventManager.GetNumberOfSiblings.Invoke(SlidesParent.transform.childCount);
    }
    
    private void OnEnable()
    {
        EventManager.TopicSelectedEvent.AddListener(UpdateSelectedTopic);
        EventManager.GetIndexOfHeldSlideEvent.AddListener(UpdateIndexOfHeldSlide);
        EventManager.SwapSlidesEvent.AddListener(SwapSlidesOnDrop);
        EventManager.SetChosenOptionEvent.AddListener(UpdateChosenOptionInSlide);
        if (SelectedTopic == null) CurrentTopicText.text = "None";
        
        _slideObjInOrder = new List<Slide>();
        _displayedSlides = new List<GameObject>();
    }

    private void OnDisable()
    {
        EventManager.TopicSelectedEvent.RemoveListener(UpdateSelectedTopic);
        EventManager.GetIndexOfHeldSlideEvent.RemoveListener(UpdateIndexOfHeldSlide);
        EventManager.SwapSlidesEvent.RemoveListener(SwapSlidesOnDrop);
        EventManager.SetChosenOptionEvent.RemoveListener(UpdateChosenOptionInSlide);
    }

    public void PrintSlideOrder()
    {
        Debug.Log("Printing slide order\n");
        for (int i = 0; i < _slideObjInOrder.Count; i++)
        {
            Debug.Log(_slideObjInOrder[i].GetSlidePreview().name);
        }
    }
    
}
