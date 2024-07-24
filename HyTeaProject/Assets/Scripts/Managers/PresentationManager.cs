using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PresentationManager : Singleton<PresentationManager>
{
    [SerializeField] private PresentationData presentationData;
    [SerializeField] public List<Feedback> _feedbacks;
    private PresentationEvaluation _presentationEvaluation;
    [SerializeField] private PresentationStartSettings _presentationStartSettings;
    [HideInInspector] public bool isCalibrationDone = false;

    protected override void Awake()
    {
        base.Awake();
        _presentationEvaluation = GetComponent<PresentationEvaluation>();
        if (_presentationEvaluation == null)
        {
            _presentationEvaluation = gameObject.AddComponent<PresentationEvaluation>();
        }
    }
    
    private void OnDisable()
    {
        EventManager.CalibrationHasStarted.AddListener(() => isCalibrationDone = false);
        EventManager.CalibrationHasFinished.AddListener(() => isCalibrationDone = true);
    }

    public void ListenToCalibration()
    {
        EventManager.CalibrationHasStarted.AddListener(() => isCalibrationDone = false);
        EventManager.CalibrationHasFinished.AddListener(() => isCalibrationDone = true);
    }
    
    public void StopListeningToCalibration()
    {
        EventManager.CalibrationHasStarted.AddListener(() => isCalibrationDone = false);
        EventManager.CalibrationHasFinished.AddListener(() => isCalibrationDone = true);
    }


    public PresentationData GetPresentationData()
    {
        return presentationData;
    }
    
    public PresentationStartSettings GetPresentationStartSettings()
    {
        return _presentationStartSettings;
    }
    
    public PresentationEvaluation GetPresentationEvaluation()
    {
        if (_presentationEvaluation == null)
        {
            _presentationEvaluation = GetComponent<PresentationEvaluation>();
        }
        if (_presentationEvaluation == null)
        {
            _presentationEvaluation = gameObject.AddComponent<PresentationEvaluation>();
        }
        return _presentationEvaluation;
    }
    
    /*private void CreateEvalutationUI()
    {
        var go = Instantiate(presentationUIPrefab);
        _presentationEvaluationUIController = go.GetComponent<PresentationEvaluationUIController>();
    }*/

    public void StartPresentation(PresentationStartSettings presentationStartSettings)
    {
        if (presentationStartSettings != null) _presentationStartSettings = presentationStartSettings; 
        
        presentationData =  gameObject.AddComponent<PresentationData>();

        MicrophoneManager.Instance.RecordMicrophone();
        
        MicrophoneManager.Instance.GetAudioDetection().StartDetectingLoudness();
        
        presentationData.StartPresentation();
        
        //start getting the gestures
    }
    
    public void StartPresentation()
    {
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

    private IEnumerator EndPresentation()
    {
        EventManager.PresentationHasEnded.Invoke();
        MicrophoneManager.Instance.StopRecording();
        
        yield return StartCoroutine(MicrophoneManager.Instance.GetAudioDetection().StopDetectingLoudness());
        
        presentationData.EndPresentation();

        yield return StartCoroutine(presentationData.SanitizeAudioClips());
        
        _presentationEvaluation.EvaluatePresentation();
        
        EventManager.ChangeToNextSlide.RemoveListener(GameManager.Instance.EndPresentation);
        
    }
    
    
}
