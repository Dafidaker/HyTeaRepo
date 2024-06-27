using UnityEngine;
using UnityEngine.UI;

public class DeselectButtonOnClick : MonoBehaviour
{
    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();

        _button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        _button.interactable = false; 
        _button.interactable = true; 
    }
}