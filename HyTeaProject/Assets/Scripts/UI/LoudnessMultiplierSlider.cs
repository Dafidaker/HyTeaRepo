using UnityEngine;
using UnityEngine.UI;
using static EventManager;

public class LoudnessMultiplierSlider : MonoBehaviour
{
    private Slider _slider;
    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.onValueChanged.AddListener((value) => ChangedLoudnessMultiplier.Invoke(value));
    }

    private void OnDisable()
    {
        _slider.onValueChanged.RemoveListener((value) => ChangedLoudnessMultiplier.Invoke(value));
    }
}
