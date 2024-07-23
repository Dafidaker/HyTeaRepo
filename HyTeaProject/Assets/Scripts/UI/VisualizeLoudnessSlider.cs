using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualizeLoudnessSlider : MonoBehaviour
{
    private Slider _slider;
    private void Awake()
    {
        _slider = GetComponent<Slider>();
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
        _slider.value = loudness;
    }
}
