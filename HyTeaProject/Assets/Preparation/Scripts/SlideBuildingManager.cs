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
    [SerializeField] private GameObject[] AvailableSlides;
    [SerializeField] private GameObject[] AvailableOptionsForSlides;
    [SerializeField] private TextMeshProUGUI CurrentTopicText;
    
    [Header("Slide Order Section")]
    [SerializeField] private GameObject IntroSlidePrefab;
    [SerializeField] private GameObject OutroSlidePrefab;
    [SerializeField] private GameObject SlidesParent;
    [SerializeField] private Transform StartingPoint;
    //[SerializeField] private GridLayoutGroup Grid;

    private GameObject[] _slidesInOrder;
    
    [Header("Sections")]
    [SerializeField] private GameObject TopicSection;
    [SerializeField] private GameObject SlideOrderSection;
    [SerializeField] private GameObject OptionsSection;
    
    public void UpdateSelectedTopic(Topic topic)
    {
        SelectedTopic = topic;
        AvailableSlides = SelectedTopic.GetAvailableSlides();
        AvailableOptionsForSlides = SelectedTopic.GetAvailableOptionsForSlides();

        CurrentTopicText.text = SelectedTopic.Title;
        
        /*Debug.Log("Topic Title: " + SelectedTopic.Title);
        Debug.Log("Available Slides: " + AvailableSlides.Length);
        Debug.Log("Available Options: " + AvailableOptionsForSlides.Length);*/
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
        for (int i = 0; i < _slidesInOrder.Length; i++)
        {
            int row = i / 4;
            int column = i % 4;
            Instantiate(_slidesInOrder[i], StartingPoint.position + new Vector3(column * 250, row * -145 , 0), quaternion.identity, SlidesParent.transform);
        }
    }
    
    private GameObject[] AddToBeginning(GameObject[] originalArray, GameObject newElement)
    {
        GameObject[] newArray = new GameObject[originalArray.Length + 1];
        newArray[0] = newElement;
        Array.Copy(originalArray, 0, newArray, 1, originalArray.Length);
        return newArray;
    }
    
    private GameObject[] AddToEnd(GameObject[] originalArray, GameObject newElement)
    {
        GameObject[] newArray = new GameObject[originalArray.Length + 1];
        Array.Copy(originalArray, newArray, originalArray.Length);
        newArray[newArray.Length - 1] = newElement;
        return newArray;
    }
    
    /*private void RefreshGrid()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Grid.transform);
    }*/
    private void OnEnable()
    {
        EventManager.TopicSelectedEvent.AddListener(UpdateSelectedTopic);
        //EventManager.RefreshGridEvent.AddListener(RefreshGrid);
        if (SelectedTopic == null) CurrentTopicText.text = "None";
    }

    private void OnDisable()
    {
        EventManager.TopicSelectedEvent.RemoveListener(UpdateSelectedTopic);    
    }

    public void test()
    {
        for (int i = 1; i < _slidesInOrder.Length - 2; i++)
        {
            _slidesInOrder[i] = _slidesInOrder[i + 1];
        }
        foreach (Transform child in SlidesParent.transform)
        {
            Destroy(child.gameObject);
        }
        
        SlidesParent.transform.DetachChildren();
        for (int i = 0; i < _slidesInOrder.Length; i++)
        {
            Instantiate(_slidesInOrder[i], SlidesParent.transform);
        }
    }

    private void Start()
    {
        TopicSection.SetActive(true);
        SlideOrderSection.SetActive(false);
        OptionsSection.SetActive(false);
    }
}
