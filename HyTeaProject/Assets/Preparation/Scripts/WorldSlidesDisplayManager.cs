using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorldSlidesDisplayManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> CompletedSlides;
    [SerializeField] private Transform SlideParent;
    [SerializeField] private GameObject SlideTransfer;

    private List<GameObject> _slideGameObjects;
    private int _indexOfCurrentSlide;
    
    
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

        for (int i = 0; i < _slideGameObjects.Count; i++)
        {
            _slideGameObjects[i].transform.localPosition = new Vector2(0, 0);
        }

        _indexOfCurrentSlide = 0;
    }

    void Start()
    {
        if (SlideTransferManager.Instance != null)
        {
            foreach (GameObject persistentObject in SlideTransferManager.Instance.gameObjectsToPersist)
            {
                MoveToActiveScene(persistentObject);
            }
        }
        else
        {
            Debug.LogError("PersistentManager instance not found!");
        }

        SlideTransfer = GameObject.Find("FullSlideTransfer");

        for (int i = 0; i < SlideTransfer.transform.childCount; i++)
        {
            CompletedSlides.Add(SlideTransfer.transform.GetChild(i).gameObject);
        }
        //SlideTransfer.SetActive(false);
        
        DisplaySlidesInWorld();
        
        Destroy(SlideTransfer);

    }

    void MoveToActiveScene(GameObject obj)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.MoveGameObjectToScene(obj, currentScene);
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
        CompletedSlides = new List<GameObject>();
        _slideGameObjects = new List<GameObject>();
    }

    private void OnDisable()
    {
        
    }
}
