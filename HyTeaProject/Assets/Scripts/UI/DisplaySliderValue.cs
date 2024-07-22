using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DisplaySliderValue : MonoBehaviour
{
    [field: SerializeField] private Slider _slider;
    private TextMeshProUGUI _textMeshProUGUI;

    private void Awake()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();

        _textMeshProUGUI.text = _slider.value.ToString(CultureInfo.InvariantCulture);
    }

    private void Start()
    {
        _slider.onValueChanged.Invoke(_slider.value);
    }

    private void OnEnable()
    {
        _slider.onValueChanged.AddListener(UpdateValue);
    }

    private void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(UpdateValue);
    }

    private void UpdateValue(float value)
    {
        _textMeshProUGUI.text = value.ToString("F1");
    }
}
