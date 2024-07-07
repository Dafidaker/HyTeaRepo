using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class PresentationBuildingManager : MonoBehaviour
{
    [Header("Topic Selection Section")]
    [SerializeField] private Topic SelectedTopic;
    [SerializeField] private List<Section> ListOfSections;
    [SerializeField] private TextMeshProUGUI CurrentTopicText; 
    
    [Header("Section Order Section")]
    [SerializeField] private GameObject SectionParent;
    [SerializeField] private GameObject PossiblePositionsParent;
    [SerializeField] private GameObject SectionPreviewPrefab;
    private List<Transform> _possiblePos;

    private List<Section> _sectionsInOrder;
    
    [Header("Sections")]
    [SerializeField] private GameObject TopicSection;
    [SerializeField] private GameObject SectionOrderSection;
    [SerializeField] private GameObject OptionsSection;
    
    public void UpdateSelectedTopic(Topic topic)
    {
        SelectedTopic = topic;
        ListOfSections = topic.GetAllSections();

        CurrentTopicText.text = SelectedTopic.Title;

        /*for (int i = 0; i < PossiblePositionsParent.transform.childCount; i++)
        {
            _possiblePos[i] = PossiblePositionsParent.transform.GetChild(i);
        }*/
    }
    
    public void MoveToSectionOrderSection()
    {
        if (SelectedTopic != null)
        {
            TopicSection.SetActive(false);
            SectionOrderSection.SetActive(true);

            _possiblePos = new List<Transform>();
            
            for (int i = 0; i < PossiblePositionsParent.transform.childCount; i++)
            {
                _possiblePos.Add(PossiblePositionsParent.transform.GetChild(i));
                //Debug.Log(_possiblePos[i].name);
            }
            DisplaySectionPreviews();
        }
        else
        {
            Debug.Log("NO TOPIC SELECTED");
        }
    }
    
    public void ReturnToTopicSection()
    {
        TopicSection.SetActive(true);
        SectionOrderSection.SetActive(false);
        
        foreach (Transform child in SectionParent.transform)
        {
            Destroy(child.gameObject);
        }
        
        SectionParent.transform.DetachChildren();
    }

    private void DisplaySectionPreviews()
    {
        _sectionsInOrder = new List<Section>();

        var availablePos = _possiblePos;
        int posChosen;

        for (int i = 0; i < ListOfSections.Count; i++)
        {
            posChosen = Random.Range(0, availablePos.Count);
            var go = Instantiate(SectionPreviewPrefab, availablePos[posChosen].position, quaternion.identity, SectionParent.transform);
            SetTextInSectionPreview(go, ListOfSections[i]);
            availablePos.RemoveAt(posChosen);
        }
    }

    private void SetTextInSectionPreview(GameObject preview, Section section)
    {
        preview.transform.Find("SectionTitle").GetComponent<TextMeshProUGUI>().SetText(section.GetSectionTitle());
        preview.transform.Find("NumOfSlides").GetComponent<TextMeshProUGUI>().SetText(section.GetSlidesRequired().Count.ToString());
        preview.GetComponent<Image>().color = section.GetColor();
    }
}
