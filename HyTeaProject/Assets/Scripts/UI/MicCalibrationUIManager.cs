using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MicCalibrationUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown microphoneDropdown;
    [SerializeField] private Slider microphoneCalibration;
    [SerializeField] private Slider microphoneGain;
    [SerializeField] private GameObject OptionsSection;
    
    [field:SerializeField] private TextMeshProUGUI instructionText;
    [field:SerializeField] private LoudnessController loudnessController;
    
    private AudioLoudnessDetection _detection;
    private float _whisperingLoudness;
    private float _projectingLoudness;
    
    private bool _isDetectionNull;
    private bool _isInstructionTextNull;
    
    private void OnDestroy()
    {
        EventManager.ClickedPlaybackButton.Invoke(false);
        MicrophoneManager.Instance.StopRecording();
    }

    private void OnEnable()
    {
        EventManager.StartedVolumeAnalysis.AddListener(DisableElements);
        EventManager.FinishedVolumeAnalysis.AddListener(EnableElements);
        
        EventManager.StartedVolumeAnalysis.AddListener(DoMicrophoneVolumeTest);
    }

    private void OnDisable()
    {
        EventManager.StartedVolumeAnalysis.RemoveListener(DisableElements);
        EventManager.FinishedVolumeAnalysis.RemoveListener(EnableElements);
        
        EventManager.StartedVolumeAnalysis.AddListener(DoMicrophoneVolumeTest);
    }
    
    private void Awake()
    {
        if (microphoneDropdown == null) { GetComponentInChildren<TMP_Dropdown>(); }
        if (microphoneCalibration == null) { GetComponentInChildren<Slider>(); }
        
        _detection = MicrophoneManager.Instance.GetAudioDetection();
        
        _isInstructionTextNull = instructionText == null;
        _isDetectionNull = _detection == null;
    }

    private void Start()
    {
        MicrophoneManager.Instance.RecordMicrophone();
    }
    
    private void Update()
    {
    }
    
    private void DisableElements()
    {
        microphoneDropdown.interactable = false;
        microphoneCalibration.interactable = false;
        microphoneGain.interactable = false;
    }
    
    private void HideElements()
    {
        OptionsSection.SetActive(false);
    }
    
    private void EnableElements()
    {
        microphoneDropdown.interactable = true;
        microphoneCalibration.interactable = true;
        microphoneGain.interactable = true;
    }
    
    private void ShowElements()
    {
        OptionsSection.SetActive(true);
    }

    private void HandleStartVolumeAnalysis()
    {
        DisableElements();
        HideElements();

        //prep for whipser
        
        EnableElements();
        ShowElements();
    }

    #region MicrophoneCalibration

    private void DoMicrophoneVolumeTest()
    {
        StartCoroutine(GetLoudnessLimits(5));
    }
    
    private IEnumerator GetLoudnessLimits(float duration)
    {
        PresentationManager.Instance.ListenToCalibration();
        EventManager.CalibrationHasStarted.Invoke();
        
        yield return StartCoroutine(GetWhisperingLoudness("Whisper for the next few seconds", duration));
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(GetProjectingLoudness("Project you voice for the next few seconds", duration));
        
        StartCoroutine(ResultsOfLoudnessTest());
    }
    
    private IEnumerator ResultsOfLoudnessTest()
    {
        var isProjectingLouderThanWhispering = _projectingLoudness > _whisperingLoudness;
        //var whisperingAndProjectingDifferenceIsNoticeable = Mathf.Abs(_projectingLoudness - _whisperingLoudness) >= 0.15f;
        
        
        var whisperingAndProjectingDifferenceIsNoticeable = _projectingLoudness / _whisperingLoudness >= 2f;
        
        instructionText.text = "Calculating the Data ...";
        yield return new WaitForSeconds(2f);
        
        if (!isProjectingLouderThanWhispering)
        {
            instructionText.text = "It seems like the audio got mixed up, your whispering seems a louder than the projecting";
            yield return new WaitForSeconds(3f);
        }

        if (!whisperingAndProjectingDifferenceIsNoticeable)
        {
            instructionText.text = "Seems like your whispering is too high or you are not projecting enough";
            yield return new WaitForSeconds(3f);
        }

        if (!isProjectingLouderThanWhispering || !whisperingAndProjectingDifferenceIsNoticeable)
        {
            instructionText.text = "You can change up the source or calibration of the microphone, and try again";
            yield return new WaitForSeconds(3f);
            instructionText.text = "If the problem precises ou might not have the equipment to play the game";
            yield return new WaitForSeconds(3f);
            instructionText.text = "";
        }
        else
        {
            CreateVolumeAnalyzer();
            instructionText.text = "Microphone is Calibrated :)";
            yield return new WaitForSeconds(3f);
            instructionText.text = "";
        }
        
        EventManager.FinishedVolumeAnalysis.Invoke();
        EventManager.CalibrationHasFinished.Invoke();
        PresentationManager.Instance.StopListeningToCalibration();
    }

    private IEnumerator GetWhisperingLoudness(string instruction, float duration)
    {
        if (_isInstructionTextNull) yield break;
    
        instructionText.text = instruction;
    
        yield return new WaitForSeconds(1.5f);

        loudnessController.loudnessTestHistory.Clear();
    
        while (duration >= 1)
        {
            instructionText.text = duration.ToString(CultureInfo.CurrentCulture);
            yield return new WaitForSeconds(1f);
            duration -= 1;
        }

        if (duration > 0)
        {
            instructionText.text = duration.ToString(CultureInfo.CurrentCulture);
            yield return new WaitForSeconds(duration);
        }

        _whisperingLoudness = loudnessController.GetLoudness();
        instructionText.text = "Whispering loudness is " + _whisperingLoudness;

    }

    private IEnumerator GetProjectingLoudness(string instruction, float duration)
    {
        if (_isInstructionTextNull) yield break;
    
        instructionText.text = instruction;
    
        yield return new WaitForSeconds(1.5f);

        loudnessController.loudnessTestHistory.Clear();
    
        while (duration >= 1)
        {
            instructionText.text = duration.ToString(CultureInfo.CurrentCulture);
            yield return new WaitForSeconds(1f);
            duration -= 1;
        }

        if (duration > 0)
        {
            instructionText.text = duration.ToString(CultureInfo.CurrentCulture);
            yield return new WaitForSeconds(duration);
        }

        _projectingLoudness = loudnessController.GetLoudness();
        instructionText.text = "Projecting loudness is " + _projectingLoudness;

    }
    
    private void CreateVolumeAnalyzer()
    {
        foreach (var dividingBar in _dividingBars)
        {
            Destroy(dividingBar);
        }
        _dividingBars.Clear();
        _volumeAnalyzer = new VolumeAnalyzer(_whisperingLoudness,_projectingLoudness,Min,Max);
        _dividingBars = _volumeAnalyzer.CreateDividors(slider,divisionBarPrefab);
    }

    #endregion
    

    
    public Slider slider;
    private RectTransform handleRectTransform;

    private List<GameObject> _dividingBars;

    [field:SerializeField] private TextMeshProUGUI loudnessText;
    [field:SerializeField] private GameObject divisionBarPrefab;
    
    private float _averageLoudness;
    private float _averageLoudnessRecurring;

    private const float Min = 0f;
    private const float Max = 1f;

    private VolumeAnalyzer _volumeAnalyzer;
    private bool _isSliderNotNull;
    
    
    /*private IEnumerator GetLoudnessRecurring()
    {
        yield return new WaitForSeconds(0.3f);
            
        _averageLoudnessRecurring = 0;
        var loudnessInstances = 0;
    
        foreach (var loudness in _loudnessHistory.Where(loudness => loudness > 0))
        {
            _averageLoudnessRecurring += loudness;
            loudnessInstances++;
        }

        if (loudnessInstances > 0)
        {
            _averageLoudnessRecurring /= loudnessInstances;
        }
    
        _loudnessHistory.Clear();

        if (_isSliderNotNull)
        {
            slider.value = _averageLoudnessRecurring;
        }

        string loudnessInText = _volumeAnalyzer.GetSpeakingVolume(_averageLoudnessRecurring).ToString();
        
        if (loudnessInText != null)
        {
            loudnessText.text = loudnessInText;
        }
        else
        {
            loudnessText.text = _averageLoudnessRecurring.ToString(CultureInfo.InvariantCulture);
        }

        
        StartCoroutine(GetLoudnessRecurring());
    }*/

    /*private float GetLoudness()
    {
        _averageLoudness = 0;
        var loudnessInstances = 0;
    
        foreach (var loudness in _loudnessTestHistory.Where(loudness => loudness > 0.001))
        {
            _averageLoudness += loudness;
            loudnessInstances++;
        }

        if (loudnessInstances > 0 )
        {
            _averageLoudness /= loudnessInstances;
        }
    
        _loudnessTestHistory.Clear();

        return _averageLoudness;
    }*/
    
    public void CreateVolumeAnalyzerInInspector()
    {
        CreateVolumeAnalyzer();
    }
    
}
