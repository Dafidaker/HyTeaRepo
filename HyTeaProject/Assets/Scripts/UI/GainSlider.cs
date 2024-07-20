using System;
using UnityEngine;
using UnityEngine.UI;
using static EventManager;

public class GainSlider : MonoBehaviour
{
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.onValueChanged.AddListener((value) => ChangedGain.Invoke(value));
    }

    private void OnDisable()
    {
        _slider.onValueChanged.RemoveListener((value) => ChangedGain.Invoke(value));
    }
}
