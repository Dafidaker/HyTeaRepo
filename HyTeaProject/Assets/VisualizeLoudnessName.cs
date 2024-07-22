using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualizeLoudnessName : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        EventManager.LatestLoudnessCaptured.AddListener(HandleNextLoudness);
    }

    private void OnDisable()
    {
        EventManager.LatestLoudnessCaptured.RemoveListener(HandleNextLoudness);
    }

    private void HandleNextLoudness(float loudness)
    {
        _text.text = MicrophoneManager.Instance.volumeAnalyzer.GetSpeakingVolume(loudness).ToString();
    }
}
