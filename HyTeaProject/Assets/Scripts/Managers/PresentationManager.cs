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
    private PresentationStartSettings _presentationStartSettings;

    protected override void Awake()
    {
        base.Awake();
        _presentationEvaluation = GetComponent<PresentationEvaluation>();
        if (_presentationEvaluation == null)
        {
            _presentationEvaluation = gameObject.AddComponent<PresentationEvaluation>();
        }
    }


    public PresentationData GetPresentationData()
    {
        return presentationData;
    }
    
    public PresentationStartSettings GetPresentationStartSettings()
    {
        return _presentationStartSettings;
    }
    
    private void CreateEvalutationUI()
    {
        var go = Instantiate(presentationUIPrefab);
        _presentationEvaluationUIController = go.GetComponent<PresentationEvaluationUIController>();
    }

    public void StartPresentation(PresentationStartSettings presentationStartSettings)
    {
        if (presentationStartSettings != null) _presentationStartSettings = presentationStartSettings; 
        
        presentationData =  gameObject.AddComponent<PresentationData>();

        MicrophoneManager.Instance.RecordMicrophone();
        
        MicrophoneManager.Instance.GetAudioDetection().StartDetectingLoudness();
        
        presentationData.StartPresentation();
        
        //start getting the gestures
    }

    public void HandleEndPresentation()
    {
        StartCoroutine(EndPresentation());
    }

    public IEnumerator EndPresentation()
    {
        MicrophoneManager.Instance.StopRecording();
        
        yield return StartCoroutine(MicrophoneManager.Instance.GetAudioDetection().StopDetectingLoudness());
        
        presentationData.EndPresentation();

        yield return StartCoroutine(presentationData.SanitizeAudioClips());
        
        _presentationEvaluation.EvaluatePresentation();
        
        EventManager.ChangeToNextSlide.RemoveListener(GameManager.Instance.EndPresentation);
    }
    
    
    public void FeedbackTriggerTriggered()
    {
        AIManager.Instance.InitiateFeedback();
    }
}
