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
    [SerializeField] private UILineRenderer LinePrefab;
    [SerializeField] private GameObject LineParent;
    
    private List<Transform> _possiblePos;
    [SerializeField] private List<Section> _sectionsInOrder;
    private bool _drawingLine;
    private int _currentLinePoint;
    private UILineRenderer _line;
    
    
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
            EventManager.SetNumOfSectionsAdded.Invoke(_sectionsInOrder.Count);
            _currentLinePoint = 0;
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
        
        if(_line) Destroy(_line);
        LineParent.transform.DetachChildren();
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
            SetVariablesInSectionPreview(go, ListOfSections[i], availablePos[posChosen]);
            availablePos.RemoveAt(posChosen);
        }
    }

    private void SetVariablesInSectionPreview(GameObject preview, Section section, Transform trans)
    {
        preview.transform.Find("SectionTitle").GetComponent<TextMeshProUGUI>().SetText(section.GetSectionTitle());
        preview.transform.Find("NumOfSlides").GetComponent<TextMeshProUGUI>().SetText(section.GetSlidesRequired().Count.ToString());
        preview.GetComponent<Image>().color = section.GetColor();
        preview.GetComponent<SectionManager>().SetSection(section);
        preview.GetComponent<SectionManager>().ButtonPos = new Vector2(trans.localPosition.x, trans.localPosition.y);
    }


    private void AddSectionToOrder(Section section, Vector2 linePoint)
    {
        if (_sectionsInOrder.Count == 0)
        {
            _sectionsInOrder.Add(section);
            _drawingLine = true;
            _line = Instantiate(LinePrefab, LineParent.transform);
            _line.points = new List<Vector2>();
            _line.points.Add(linePoint);
            _currentLinePoint++;
            //_line.points.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        else if(_drawingLine && _sectionsInOrder.Count < ListOfSections.Count)
        {
            if (_sectionsInOrder.Contains(section))
            {
                Debug.Log("Section already in list");
                return;
            }
            
            _sectionsInOrder.Add(section);
            _line.points.Add(linePoint);
            _line.SetAllDirty();
        }
        else if(_sectionsInOrder.Count == ListOfSections.Count)
        {
            Debug.Log("All Sections have been selected");
            _drawingLine = false;
        }
    }

    private void Update()
    {
        if (_drawingLine)
        {
            //_line.points[_currentLinePoint] = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        
        //_line.SetAllDirty();
    }

    private void OnEnable()
    {
        EventManager.AddSectionToOrderEvent.AddListener(AddSectionToOrder);
        _drawingLine = false;
    }

    private void OnDisable()
    {
        EventManager.AddSectionToOrderEvent.RemoveListener(AddSectionToOrder);
    }
}
