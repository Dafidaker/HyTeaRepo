using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private Canvas _canvas;

    [Header("Slide Customization Section")] 
    [SerializeField] private List<FullSlide> ListOfFullSlides;
    [SerializeField] private GameObject FullSlideParent;

    [SerializeField] private List<GameObject> _fullSlidesGameObjects;
    private int _indexOfCurrentSlide;
    private bool _SlidesReady;
    
    
    [Header("Sections")]
    [SerializeField] private GameObject TopicSection;
    [SerializeField] private GameObject SectionOrderSection;
    [SerializeField] private GameObject SlideCustomizationSection;
    
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

    public void MoveToSlideCustomizationSection()
    {
        if (_sectionsInOrder.Count != ListOfSections.Count)
        {
            Debug.Log("There are still sections to select!");
        }
        else
        {
            SectionOrderSection.SetActive(false);
            SlideCustomizationSection.SetActive(true);

            ListOfFullSlides = new List<FullSlide>();

            for (int i = 0; i < _sectionsInOrder.Count; i++)
            {
                for (int j = 0; j < _sectionsInOrder[i].GetSlidesRequired().Count; j++)
                {
                    ListOfFullSlides.Add(_sectionsInOrder[i].GetSlidesRequired()[j]);
                }
            }
            
            DisplayFullSlides();

            for (int k = 1; k < _fullSlidesGameObjects.Count; k++)
            {
                _fullSlidesGameObjects[k].SetActive(false);
            }

            _indexOfCurrentSlide = 0;
        }
    }

    public void ReturnToSectionOrder()
    {
        SlideCustomizationSection.SetActive(false);
        SectionOrderSection.SetActive(true);

        foreach (Transform child in FullSlideParent.transform)
        {
            Destroy(child.gameObject);
        }
        
        FullSlideParent.transform.DetachChildren();
    }

    public void CycleThroughSlides(int i)
    {
        _fullSlidesGameObjects[_indexOfCurrentSlide].SetActive(false);
        _indexOfCurrentSlide += i;
        if (_indexOfCurrentSlide < 0) _indexOfCurrentSlide = _fullSlidesGameObjects.Count - 1;
        if (_indexOfCurrentSlide >= _fullSlidesGameObjects.Count) _indexOfCurrentSlide = 0;
        _fullSlidesGameObjects[_indexOfCurrentSlide].SetActive(true);
    }

    public void FinishCustomization()
    {
        int completedSlides = 0;
        for (int i = 0; i < _fullSlidesGameObjects.Count; i++)
        {
            if (_fullSlidesGameObjects[i].GetComponent<FullSlideManager>().HasTextOptionSelected)
            {
                completedSlides++;
            }
        }

        if (completedSlides == _fullSlidesGameObjects.Count)
        {
            Debug.Log("FINISH!");
            for (int i = 0; i < _fullSlidesGameObjects.Count; i++)
            {
                _fullSlidesGameObjects[i].SetActive(false);
            }
            _fullSlidesGameObjects[0].SetActive(true);
        }
        else
        {
            Debug.Log("THERE ARE STILL INCOMPLETED SLIDES");
        }
        
        EventManager.GetCompletedSlides.Invoke(_fullSlidesGameObjects);
        
        SlideCustomizationSection.SetActive(false);
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

    private void DisplayFullSlides()
    {
        _fullSlidesGameObjects = new List<GameObject>();
        for (int i = 0; i < ListOfFullSlides.Count; i++)
        {
            var go = Instantiate(ListOfFullSlides[i].FullSlidePrefab, FullSlideParent.transform);
            _fullSlidesGameObjects.Add(go);
        }

        for (int i = 0; i < _fullSlidesGameObjects.Count; i++)
        {
            SetVariablesInFullSlides(_fullSlidesGameObjects[i], ListOfFullSlides[i].CorrespondingSection, i);
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

    private void SetVariablesInFullSlides(GameObject slide, Section section, int num)
    {
        slide.GetComponent<FullSlideManager>().SetTitle(section.GetSectionTitle() + " " + ListOfFullSlides[num].Title);
        slide.GetComponent<FullSlideManager>().SetOptions(ListOfFullSlides[num].TextOptions);
        slide.GetComponent<FullSlideManager>().SetMiscOption(ListOfFullSlides[num].MiscOption);
        slide.GetComponent<FullSlideManager>().SetImages(ListOfFullSlides[num].AvailableImages);
        slide.GetComponent<Image>().color = section.GetColor();
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
            Vector2 movePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                SectionOrderSection.GetComponent<Canvas>().transform as RectTransform,
                Input.mousePosition, SectionOrderSection.GetComponent<Canvas>().worldCamera,
                out movePos);
            _line.points.Add(movePos);
        }
        else if(_drawingLine && _sectionsInOrder.Count < ListOfSections.Count)
        {
            if (_sectionsInOrder.Contains(section))
            {
                Debug.Log("Section already in list");
                return;
            }
            
            _sectionsInOrder.Add(section);
            _line.points[_currentLinePoint] = linePoint;
            _line.points.Add(linePoint);
            _currentLinePoint++;

            if (_sectionsInOrder.Count == ListOfSections.Count)
            {
                Debug.Log("All Sections have been selected");
                _drawingLine = false;
            }
            
            _line.SetAllDirty();
        }
        else if(_sectionsInOrder.Count == ListOfSections.Count)
        {
            Debug.Log("All Sections have been selected");
            _drawingLine = false;
        }
    }

    public void ResetSectionOrder()
    {
        _sectionsInOrder = new List<Section>();
        if(_line) Destroy(_line);
        LineParent.transform.DetachChildren();
        _currentLinePoint = 0;
        _drawingLine = false;
    }

    private void Start()
    {
        _canvas = SectionOrderSection.GetComponent<Canvas>();
    }

    private void Update()
    {
        if (_drawingLine && _sectionsInOrder.Count > 0)
        {
            Vector2 movePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                Input.mousePosition, _canvas.worldCamera,
                out movePos);
            _line.points[_currentLinePoint] = movePos;
        }
        
        if(_line) _line.SetAllDirty();
    }

    private void OnEnable()
    {
        EventManager.AddSectionToOrderEvent.AddListener(AddSectionToOrder);
        _drawingLine = false;
        _SlidesReady = false;
    }

    private void OnDisable()
    {
        EventManager.AddSectionToOrderEvent.RemoveListener(AddSectionToOrder);
    }
}
