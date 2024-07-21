using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlideEvaluationManager : MonoBehaviour
{
    [SerializeField] private GameObject SlideTransfer;
    [SerializeField] private List<GameObject> SlidesToEvaluate;

    private List<FullSlideManager> _slideManagers;

    private int _presentationTotalScore;


    private void Start()
    {
        SlidesToEvaluate = new List<GameObject>();
        _slideManagers = new List<FullSlideManager>();
           
        SlideTransfer = GameObject.Find("FullSlideTransfer");

        for (int i = 0; i < SlideTransfer.transform.childCount; i++)
        {
            SlidesToEvaluate.Add(SlideTransfer.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < SlidesToEvaluate.Count; i++)
        {
            _slideManagers.Add(SlidesToEvaluate[i].GetComponent<FullSlideManager>());
        }
    }

    private void EvaluateImageContent()
    {
        int slidesWithImages = 0;
        int imagesInARow = 0;
        int imagesInARowPenalties = 0;
        
        for (int i = 0; i < _slideManagers.Count; i++)
        {
            if (_slideManagers[i].HasImageSelected)
            {
                slidesWithImages++;
                imagesInARow++;
                if (imagesInARow >= 3)
                {
                    imagesInARowPenalties++;
                    Debug.Log("Too many images in a row");
                    imagesInARow = 0;
                }
            }
            else
            {
                imagesInARow = 0;
            }
        }
        
        Debug.Log("Slides with Images: " + slidesWithImages + "\n");
        Debug.Log("Times of too many slides in a row: " + imagesInARowPenalties + "\n");

        if (slidesWithImages == 0)
        {
            Debug.Log("No Images where used!\n");
        }
    }

    private void EvalauteTextContent()
    {
        for (int i = 0; i < _slideManagers.Count; i++)
        {
            _presentationTotalScore += _slideManagers[i].SlideScore;
        }

        _presentationTotalScore /= _slideManagers.Count;

        Debug.Log("Final Score of the presentation: " + _presentationTotalScore);
    }

    private void Evaluate()
    {
        //EvaluateImageContent();
        EvalauteTextContent();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Evaluate();
        }
    }
}

