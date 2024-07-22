using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class VolumeAnalyzer : Object
{
    public VolumeAnalyzer(float threshold, float minAcceptable, float maxAcceptable)
    {
        _threshold = threshold;
        _minAcceptable = minAcceptable;
        _maxAcceptable = maxAcceptable;
    }

    public VolumeAnalyzer(float whisperingLoudness, float projectingLoudness, float min, float max)
    {
        _threshold = Mathf.Clamp(whisperingLoudness - 0.05f, min, max);
        _minAcceptable = Mathf.Clamp(projectingLoudness - 0.15f, min, max);
        _maxAcceptable = Mathf.Clamp(projectingLoudness + 0.15f, min, max);
        _minValue = min;
        _maxValue = max;
    }

    public List<GameObject> CreateDividors(Slider slider,GameObject dividorPrefab)
    {
        float initialValue = slider.value;
        
        List<GameObject> gameObjects = new List<GameObject>();

        gameObjects.Add(CreateADivider(slider,  dividorPrefab, _threshold));
        gameObjects.Add(CreateADivider(slider,  dividorPrefab, _minAcceptable));
        gameObjects.Add(CreateADivider(slider,  dividorPrefab, _maxAcceptable));
        
        slider.value = initialValue;
        return gameObjects;
    }

    private GameObject CreateADivider(Slider slider, GameObject dividorPrefab, float value)
    {
        slider.value = value;
        GameObject tempGo = Instantiate(dividorPrefab);
        if (slider.handleRect.parent != null) { tempGo.transform.SetParent(slider.handleRect.parent); }
        CopyRectTransform(slider.handleRect.GetComponent<RectTransform>(), tempGo.GetComponent<RectTransform>());
        return tempGo;
    }
    
    void CopyRectTransform(RectTransform source, RectTransform target)
    {
        // Copy all the RectTransform properties
        target.sizeDelta = source.sizeDelta;
        target.anchoredPosition = source.anchoredPosition;
        target.anchorMin = source.anchorMin;
        target.anchorMax = source.anchorMax;
        target.pivot = source.pivot;
        target.localRotation = source.localRotation;
        target.localScale = source.localScale;
        target.anchoredPosition3D = source.anchoredPosition3D;
        target.localPosition = source.localPosition;
        target.localEulerAngles = source.localEulerAngles;

        // Ensure the target RectTransform's parent is set correctly
        if (source.parent != null)
        {
            target.SetParent(source.parent, true);
        }
        
        // Set anchors and pivot
        target.anchorMin = source.anchorMin;
        target.anchorMax = source.anchorMax;
        target.pivot = source.pivot;
    }
    
    
    private readonly float _threshold;
    private readonly float _minAcceptable;
    private readonly float _maxAcceptable;
    private readonly float _minValue;
    private readonly float _maxValue;

    public SpeakingVolume GetSpeakingVolume(float loudness)
    {
        //smaller than the threshold
        if (loudness < _threshold)
        {
            return SpeakingVolume.Mute;
        }

        //bigger than the _threshold && smaller than the _minAcceptable
        if (loudness >= _threshold && loudness <= _minAcceptable)
        {
            return SpeakingVolume.Whisper;
        }
    
        //bigger than the _minAcceptable && smaller than the _maxAcceptable
        if (loudness > _minAcceptable && loudness <= _maxAcceptable)
        {
            return SpeakingVolume.Normal;
        }
    
        //loud if bigger than the _maxAcceptable otherwise is Mute
        return loudness > _maxAcceptable ? SpeakingVolume.Loud : SpeakingVolume.Mute;
    }
}

public class SliderVoiceLoudness : MonoBehaviour
{   
    private AudioLoudnessDetection _detection;

    public Slider slider;
    public Slider everytimeSlider;
    private RectTransform handleRectTransform;

    private List<GameObject> _dividingBars;

    [field:SerializeField] private TextMeshProUGUI instructionText;
    [field:SerializeField] private TextMeshProUGUI loudnessText;
    [field:SerializeField] private GameObject divisionBarPrefab;
    
    public float loudnessSensitivity = 100;
    public float threshold = 0.1f;

    private bool _isDetectionNull;
    private bool _isInstructionTextNull;

    private List<float> _loudnessHistory;
    private List<float> _loudnessTestHistory;
    private float _averageLoudness;
    private float _averageLoudnessRecurring;
    public float _whisperingLoudness;
    public float _projectingLoudness;

    private const float Min = 0f;
    private const float Max = 1f;

    private VolumeAnalyzer _volumeAnalyzer;
    private bool _isSliderNotNull;
    private bool _isEverytimeSliderNotNull;

    private void OnEnable()
    {
        EventManager.PressedVolumeAnalysisButton.AddListener(CheckMicrophone);
        EventManager.StartedVolumeAnalysis.AddListener(DoMicrophoneVolumeTest);
    }
    
    private void OnDisable()
    {
        EventManager.PressedVolumeAnalysisButton.RemoveListener(CheckMicrophone);
        EventManager.StartedVolumeAnalysis.AddListener(DoMicrophoneVolumeTest);
    }

    private void Awake()
    {
        slider.minValue = Min;
        slider.maxValue = Max;
        
        //_volumeAnalyzer = new VolumeAnalyzer(0.1f,0.5f,Min,Max);
        //_dividingBars = _volumeAnalyzer.CreateDividors(slider, divisionBarPrefab);

        if (_detection == null)
        {
            _detection = MicrophoneManager.Instance.GetAudioDetection();
        }
        
        _isInstructionTextNull = instructionText == null;
        _isDetectionNull = _detection == null;
        
        _loudnessHistory = new List<float>();
        _loudnessTestHistory = new List<float>();
        
        handleRectTransform = slider.handleRect;
    }

    private void Start()
    {
        _isEverytimeSliderNotNull = everytimeSlider != null;
        _isSliderNotNull = slider != null;
        StartCoroutine(GetLoudnessRecurring());
    }

    private void Update()
    {
        if (_isDetectionNull) return;

        float loudness = _detection.currentLoudness;
    
        _loudnessHistory.Add(loudness);
        _loudnessTestHistory.Add(loudness);
    }
    
    /*private static float RoundUpToOneDecimalPlace(float value)
    {
        return Mathf.Ceil(value * 100f) / 100f;
    }*/

    public void UpdateSensitivity(float sensitivity)
    {
        loudnessSensitivity = sensitivity;
    }

    private void CheckMicrophone()
    {
        instructionText.text = "Please check the source and calibration of the microphone and then\n press the button to continue";
    }
    
    private void DoMicrophoneVolumeTest()
    {
        StartCoroutine(GetLoudnessLimits(5));
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
    
    private IEnumerator GetLoudnessRecurring()
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
    }

    private float GetLoudness()
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

        _loudnessTestHistory.Clear();
    
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

        _whisperingLoudness = GetLoudness();
        instructionText.text = "Whispering loudness is " + _whisperingLoudness;

    }

    private IEnumerator GetProjectingLoudness(string instruction, float duration)
    {
        if (_isInstructionTextNull) yield break;
    
        instructionText.text = instruction;
    
        yield return new WaitForSeconds(1.5f);

        _loudnessTestHistory.Clear();
    
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

        _projectingLoudness = GetLoudness();
        instructionText.text = "Projecting loudness is " + _projectingLoudness;

    }
    
    public void ButtonCallExample()
    {
        CreateVolumeAnalyzer();
    }
}
