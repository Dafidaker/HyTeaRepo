using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MicCalibrationUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown microphoneDropdown;
    [SerializeField] private Slider microphoneCalibration;
    [SerializeField] private Slider microphoneGain;
    [SerializeField] private LoudnessController loudnessController;
    
    [SerializeField,Header("Sections")] private GameObject BeforeCalibrationSection;
    [SerializeField] private GameObject CalibrationSection;
    [SerializeField] private GameObject AfterCalibrationSection;
    
    [SerializeField, Header("During Calibration Text")] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI instructionCountdownText;
    
    private AudioLoudnessDetection _detection;
    private float _whisperingLoudness;
    private float _projectingLoudness;
    
    private bool _isInstructionTextNull;
    private bool _calibrationComplete;
    
    public Slider loudnessSlider;

    private List<GameObject> _dividingBars;
    [field:SerializeField] private GameObject divisionBarPrefab;
    
    private float _averageLoudness;
    private float _averageLoudnessRecurring;

    private const float Min = 0f;
    private const float Max = 1f;

    private VolumeAnalyzer _volumeAnalyzer;
    private bool _isSliderNotNull;
    
    private void OnDestroy()
    {
        EventManager.ClickedPlaybackButton.Invoke(false);
        MicrophoneManager.Instance.StopRecording();
    }

    private void OnEnable()
    {
        EventManager.StartedVolumeAnalysis.AddListener(UpdateUIForCalibration);
        
        EventManager.StartedVolumeAnalysis.AddListener(DoMicrophoneVolumeTest);
    }

    private void OnDisable()
    {
        EventManager.StartedVolumeAnalysis.RemoveListener(UpdateUIForCalibration);
        
        EventManager.StartedVolumeAnalysis.AddListener(DoMicrophoneVolumeTest);
    }
    
    private void Awake()
    {
        if (microphoneDropdown == null) { GetComponentInChildren<TMP_Dropdown>(); }
        if (microphoneCalibration == null) { GetComponentInChildren<Slider>(); }
        
        _detection = MicrophoneManager.Instance.GetAudioDetection();
        
        _isInstructionTextNull = instructionText == null;
    }

    private void Start()
    {
        MicrophoneManager.Instance.RecordMicrophone();
    }


    #region ButtonOnClick

    public void RedoCalibration()
    {
        DestroyDividingBars();
        _calibrationComplete = false;
        BeforeCalibrationSection.SetActive(true);
        
        AfterCalibrationSection.SetActive(false);
        CalibrationSection.SetActive(false);
    }

    public void ContinueCalibration()
    {
        MSceneManager.Instance.FadeToNextScene();
    }

    #endregion

    #region UIEnabling&Disabling

    
    
    private void UpdateUIForCalibration()
    {
        BeforeCalibrationSection.SetActive(false);
        AfterCalibrationSection.SetActive(false);
        
        CalibrationSection.SetActive(true);
    }
    
    private void UpdateUIAfterCalibration()
    {
        CalibrationSection.SetActive(false);
        
        if (_calibrationComplete)
        {
            AfterCalibrationSection.SetActive(true);
        }
        else
        {
            BeforeCalibrationSection.SetActive(false);
        }
        
    }
        

    #endregion
    
    public void HandleStartVolumeAnalysis()
    {
        StartCoroutine(StartVolumeAnalysis());
    }

    private IEnumerator StartVolumeAnalysis()
    {
        UpdateUIForCalibration();
        
        yield return StartCoroutine(GetLoudnessLimits(5));

        UpdateUIAfterCalibration();
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
        
        instructionText.text = "Well done";
        instructionCountdownText.text = " ";
        yield return new WaitForSeconds(1.5f);
        instructionText.text = "And now";
        yield return new WaitForSeconds(1.5f);
        
        yield return StartCoroutine(GetProjectingLoudness("Project you voice for the next few seconds", duration));
        
        instructionCountdownText.text = " ";
        yield return StartCoroutine(ResultsOfLoudnessTest());
    }
    
    private IEnumerator ResultsOfLoudnessTest()
    {
        _projectingLoudness = 0.7f;
        _whisperingLoudness = 0.3f;
        
        _calibrationComplete = false;
        
        var isProjectingLouderThanWhispering = _projectingLoudness > _whisperingLoudness;
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
            _calibrationComplete = true;
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
        instructionCountdownText.text = duration.ToString("0");
    
        yield return new WaitForSeconds(1.5f);

        loudnessController.loudnessTestHistory.Clear();
    
        while (duration >= 1)
        {
            instructionCountdownText.text = duration.ToString("0");
            //instructionText.text = duration.ToString(CultureInfo.CurrentCulture);
            yield return new WaitForSeconds(1f);
            duration -= 1;
        }

        if (duration > 0)
        {
            instructionCountdownText.text = duration.ToString("0");
            //instructionText.text = duration.ToString(CultureInfo.CurrentCulture);
            yield return new WaitForSeconds(duration);
        }

        _whisperingLoudness = loudnessController.GetLoudness();
        //instructionText.text = "Whispering loudness is " + _whisperingLoudness;

    }

    private IEnumerator GetProjectingLoudness(string instruction, float duration)
    {
        if (_isInstructionTextNull) yield break;
    
        instructionText.text = instruction;
        instructionCountdownText.text = duration.ToString("0");
        
        yield return new WaitForSeconds(1.5f);

        loudnessController.loudnessTestHistory.Clear();
    
        while (duration >= 1)
        {
            instructionCountdownText.text = duration.ToString("0");
            //instructionText.text = duration.ToString(CultureInfo.CurrentCulture);
            yield return new WaitForSeconds(1f);
            duration -= 1;
        }

        if (duration > 0)
        {
            instructionCountdownText.text = duration.ToString("0");
            //instructionText.text = duration.ToString(CultureInfo.CurrentCulture);
            yield return new WaitForSeconds(duration);
        }

        _projectingLoudness = loudnessController.GetLoudness();
        //instructionText.text = "Projecting loudness is " + _projectingLoudness;

    }
    
    #endregion
    
    
    private void CreateVolumeAnalyzer()
    {
        DestroyDividingBars();
        
        _volumeAnalyzer = new VolumeAnalyzer(_whisperingLoudness,_projectingLoudness,Min,Max);
        _dividingBars = _volumeAnalyzer.CreateDividors(loudnessSlider,divisionBarPrefab);
    }

    private void DestroyDividingBars()
    {
        _dividingBars ??= new List<GameObject>();
        
        foreach (var dividingBar in _dividingBars)
        {
            Destroy(dividingBar);
        }
        _dividingBars.Clear();
    }
    
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
            loudnessSlider.value = _averageLoudnessRecurring;
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
