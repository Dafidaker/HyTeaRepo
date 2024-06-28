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
    
    private List<Slide> _slideObjInOrder;

    private int _indexOfHeldSlide;
    private int _indexOfTargetSlide;
    
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
            DisplayAvailableOptions();
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

    private void DisplayAvailableOptions()
    {
        _slideObjInOrder = AvailableSlides;
        _slideObjInOrder = AddToBeginning(_slideObjInOrder, IntroSlideObj);
        _slideObjInOrder = AddToEnd(_slideObjInOrder, OutroSlideObj);
        
        for (int i = 0; i < _slideObjInOrder.Count; i++)
        {
            int row = i / 4;
            int column = i % 4;
            var go = Instantiate(_slideObjInOrder[i].GetSlidePreview(), StartingPoint.position + new Vector3(column * 250, row * -145 , 0), quaternion.identity, SlidesParent.transform);
            if (!go.GetComponent<DragAndDropSlideManager>()) continue;
            go.GetComponent<DragAndDropSlideManager>().SetIndex(i);
        }
        
        EventManager.GetNumberOfSiblings.Invoke(SlidesParent.transform.childCount);
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
        
        for (int i = 0; i < _slideObjInOrder.Count; i++)
        {
            int row = i / 4;
            int column = i % 4;
            var go = Instantiate(_slideObjInOrder[i].GetSlidePreview(), StartingPoint.position + new Vector3(column * 250, row * -145 , 0), quaternion.identity, SlidesParent.transform);
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
        if (SelectedTopic == null) CurrentTopicText.text = "None";
        
        _slideObjInOrder = new List<Slide>();
    }

    private void OnDisable()
    {
        EventManager.TopicSelectedEvent.RemoveListener(UpdateSelectedTopic);
        EventManager.GetIndexOfHeldSlideEvent.RemoveListener(UpdateIndexOfHeldSlide);
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
