using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideBuildingManager : MonoBehaviour
{
    [SerializeField] private Topic SelectedTopic;
    [SerializeField] private GameObject[] AvailableSlides;
    [SerializeField] private GameObject[] AvailableOptionsForSlides;
    [SerializeField] private Transform SlideStartPoint;
    [SerializeField] private GameObject IntroSlidePrefab;
    [SerializeField] private GameObject OutroSlidePrefab;
    [SerializeField] private GameObject SlidesParent;

    public void UpdateSelectedTopic(Topic topic)
    {
        SelectedTopic = topic;
        AvailableSlides = SelectedTopic.GetAvailableSlides();
        AvailableOptionsForSlides = SelectedTopic.GetAvailableOptionsForSlides();
        
        Debug.Log("Topic Title: " + SelectedTopic.Title);
        Debug.Log("Available Slides: " + AvailableSlides.Length);
        Debug.Log("Available Options: " + AvailableOptionsForSlides.Length);
    }

    public Topic GetSelectedTopic()
    {
        return SelectedTopic;
    }
    
    private void OnEnable()
    {
        EventManager.TopicSelectedEvent.AddListener(UpdateSelectedTopic);
    }

    private void OnDisable()
    {
        EventManager.TopicSelectedEvent.RemoveListener(UpdateSelectedTopic);    
    }
}
