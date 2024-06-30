using TMPro;
using UnityEngine;

public class TextIndicator : MonoBehaviour
{
    [SerializeField] private TMP_Text _textField;
    [SerializeField] private string _staticText;
    
    public void SetDistanceText(string newText)
    {
        _textField.text = _staticText + " " + newText;
    }
}
