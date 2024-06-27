using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;


public class MicrophoneCalibrationButton : VirtualButton
{
    enum ButtonState
    {
        Start,
        Continue
    }
    
    [field: SerializeField] private TextMeshProUGUI buttonText;
    private string _initialButtonName;
    
    private ButtonState _buttonState;
    private Button _button;
    
    private void OnEnable()
    {
        EventManager.FinishedVolumeAnalysis.AddListener(ResetButtonName);
    }
    
    private void OnDisable()
    {
        EventManager.FinishedVolumeAnalysis.RemoveListener(ResetButtonName);
    }

    private void Start()
    {
        _button = GetComponent<Button>();
        buttonText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        _buttonState = ButtonState.Start;
        _initialButtonName = buttonText.text;
    }


    public override void OnClick()
    {
        switch (_buttonState)
        {
            case ButtonState.Start:
                EventManager.PressedVolumeAnalysisButton.Invoke();
                buttonText.text = "Continue";
                _buttonState = ButtonState.Continue;
                break;
            case ButtonState.Continue:
                _button.enabled = false;
                EventManager.StartedVolumeAnalysis.Invoke();
                break;
        }
    }

    private void ResetButtonName()
    {
        _buttonState = ButtonState.Start;
        buttonText.text = _initialButtonName;
        _button.enabled = true;
    }
    
    
}
