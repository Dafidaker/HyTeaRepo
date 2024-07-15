using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentationManager : Singleton<PresentationManager>
{
    [field: SerializeField] private PresentationData presentationData;
    [field: SerializeField] public List<Feedback> _feedbacks;
    [field: SerializeField] private GameObject presentationUIPrefab;
    private PresentationEvaluationUIController _presentationEvaluationUIController;
    private PresentationEvaluation _presentationEvaluation;

    public PresentationData GetPresentationData()
    {
        return presentationData;
    }
    
    private void CreateEvalutationUI()
    {
        var go = Instantiate(presentationUIPrefab);
        _presentationEvaluationUIController = go.GetComponent<PresentationEvaluationUIController>();
    }

    public void StartPresentation()
    {
        presentationData =  gameObject.AddComponent<PresentationData>();
        // create a presentation data 

        //start record the mic
        MicrophoneManager.Instance.RecordMicrophone();
        
        MicrophoneManager.Instance.GetAudioDetection().StartDetectingLoudness();
        
        presentationData.StartPresentation();
        
        //todo
        //record loudness
        
        //start getting the gestures
    }

    public void HandleEndPresentation()
    {
        StartCoroutine(EndPresentation());
    }

    public IEnumerator EndPresentation()
    {
        _presentationEvaluation = gameObject.AddComponent<PresentationEvaluation>();
        
        MicrophoneManager.Instance.StopRecording();
        
        yield return StartCoroutine(MicrophoneManager.Instance.GetAudioDetection().StopDetectingLoudness());
        
        presentationData.EndPresentation();

        yield return StartCoroutine(presentationData.SanitizeAudioClips());
        
        _presentationEvaluation.EvaluatePresentation();
    }
    
    

    public void FeedbackTriggerTriggered()
    {
        AIManager.Instance.InitiateFeedback();
    }
}
