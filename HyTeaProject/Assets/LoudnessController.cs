using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class LoudnessController : MonoBehaviour
{
    [HideInInspector] public List<float> loudnessHistory;
    [HideInInspector] public List<float> loudnessTestHistory;
    
    private AudioLoudnessDetection _detection;
    
    private float _averageLoudness;
    private float _averageLoudnessRecurring;
    
    private bool _isDetectionNull;

    private void Awake()
    {
        loudnessHistory = new List<float>();
        loudnessTestHistory = new List<float>();
    }

    private void Start()
    {
        _detection = MicrophoneManager.Instance.GetAudioDetection();
        _isDetectionNull = _detection == null;
        
        StartCoroutine(GetLoudnessRecurring());
    }

    private void Update()
    {
        if (_isDetectionNull) return;

        float loudness = _detection.currentLoudness;
    
        loudnessHistory.Add(loudness);
        loudnessTestHistory.Add(loudness);
    }
    
    private IEnumerator GetLoudnessRecurring()
    {
        yield return new WaitForSeconds(0.3f);
            
        _averageLoudnessRecurring = 0;
        var loudnessInstances = 0;
    
        foreach (var loudness in loudnessHistory.Where(loudness => loudness > 0))
        {
            _averageLoudnessRecurring += loudness;
            loudnessInstances++;
        }

        if (loudnessInstances > 0)
        {
            _averageLoudnessRecurring /= loudnessInstances;
        }
    
        loudnessHistory.Clear();
        
        //todo send event with recurring loudness

        /*if (_isSliderNotNull)
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
        }*/

        
        StartCoroutine(GetLoudnessRecurring());
    }
    
    public float GetLoudness()
    {
        _averageLoudness = 0;
        var loudnessInstances = 0;
    
        foreach (var loudness in loudnessTestHistory.Where(loudness => loudness > 0.001))
        {
            _averageLoudness += loudness;
            loudnessInstances++;
        }

        if (loudnessInstances > 0 )
        {
            _averageLoudness /= loudnessInstances;
        }
    
        loudnessTestHistory.Clear();

        return _averageLoudness;
    }
}
