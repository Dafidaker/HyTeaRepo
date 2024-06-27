using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class SlideBuildingManager : MonoBehaviour
{
    [Header("Topic Selection Section")]
    [SerializeField] private Topic SelectedTopic;
    [SerializeField] private List<GameObject> AvailableSlides;
    [SerializeField] private List<GameObject> AvailableOptionsForSlides;
    [SerializeField] private TextMeshProUGUI CurrentTopicText;
    
    [Header("Slide Order Section")]
    [SerializeField] private GameObject IntroSlidePrefab;
    [SerializeField] private GameObject OutroSlidePrefab;
    [SerializeField] private GameObject SlidesParent;
    [SerializeField] private Transform StartingPoint;

    private List<GameObject> _slidesInOrder;

    private int _indexOfHeldSlide;
    
    [Header("Sections")]
    [SerializeField] private GameObject TopicSection;
    [SerializeField] private GameObject SlideOrderSection;
    [SerializeField] private GameObject OptionsSection;
    
    public void UpdateSelectedTopic(Topic topic)
    {
        SelectedTopic = topic;
        //AvailableSlides = SelectedTopic.GetAvailableSlides();
        AvailableSlides = SelectedTopic.GetAllPreviewSlides();
        AvailableOptionsForSlides = SelectedTopic.GetAvailableOptionsForSlides();

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
        _slidesInOrder = AvailableSlides;
        _slidesInOrder = AddToBeginning(_slidesInOrder, IntroSlidePrefab);
        _slidesInOrder = AddToEnd(_slidesInOrder, OutroSlidePrefab);
        for (int i = 0; i < _slidesInOrder.Count; i++)
        {
            int row = i / 4;
            int column = i % 4;
            var go = Instantiate(_slidesInOrder[i], StartingPoint.position + new Vector3(column * 250, row * -145 , 0), quaternion.identity, SlidesParent.transform);
            if (!go.GetComponent<DragAndDropSlideManager>()) continue;
            go.GetComponent<DragAndDropSlideManager>().SetIndex(i);
        }
        
        EventManager.GetNumberOfSiblings.Invoke(SlidesParent.transform.childCount);
    }
    
    private List<GameObject> AddToBeginning(List<GameObject> originalList, GameObject newElement)
    {
        List<GameObject> newList = new List<GameObject>(originalList.Count + 1);
        newList.Add(newElement);
        newList.AddRange(originalList);
        return newList;
    }
    
    private List<GameObject> AddToEnd(List<GameObject> originalList, GameObject newElement)
    {
        List<GameObject> newList = new List<GameObject>(originalList);
        newList.Add(newElement);
        return newList;
    }
    
    private void UpdateIndexOfHeldSlide(int index)
    {
        _indexOfHeldSlide = index;
        Debug.Log("Event triggered. Index of the held slide: " + index);
    }
    private void OnEnable()
    {
        EventManager.TopicSelectedEvent.AddListener(UpdateSelectedTopic);
        EventManager.GetIndexOfHeldSlideEvent.AddListener(UpdateIndexOfHeldSlide);
        if (SelectedTopic == null) CurrentTopicText.text = "None";

        _slidesInOrder = new List<GameObject>();
    }

    private void OnDisable()
    {
        EventManager.TopicSelectedEvent.RemoveListener(UpdateSelectedTopic);    
    }
}
