
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

public class SliderVoiceLoudness : MonoBehaviour
{   
    public AudioLoudnessDetection detection;

    public Slider Slider;

    public float loudnessSentitivity = 100;
    public float threshold = 0.1f;
    private bool _isdetectionNull;

    private void Start()
    {
        _isdetectionNull = detection == null;
    }

    private void Update()
    {
        if (_isdetectionNull) return;
        
        var loudness = detection.GetLoudnessFromMicrophone() * loudnessSentitivity;

        if (loudness < threshold) { loudness = 0; }

        Slider.value = RoundUpToOneDecimalPlace(loudness);
    }
    
    float RoundUpToOneDecimalPlace(float value)
    {
        return Mathf.Ceil(value * 100f) / 100f;
    }

    public void UpdateSentitivity(float sensitivity)
    {
        loudnessSentitivity = sensitivity;
    }
}
