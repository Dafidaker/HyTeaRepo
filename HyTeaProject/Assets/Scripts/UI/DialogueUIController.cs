using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogueUIController : MonoBehaviour
{
    [field: SerializeField] private TextMeshProUGUI nameText;
    [field: SerializeField] private TextMeshProUGUI dialogueText;
    [FormerlySerializedAs("continueText")] [field: SerializeField] private Image continueImg;
    
    public void ChangeNameText(string str)
    {
        if (nameText == null) return;
        nameText.text = str;
    }
    
    public void ChangeDialogueText(string str)
    {
        if (dialogueText == null) return;
        dialogueText.text = str;
    }

    public void EnableContinueText(bool value)
    {
        if (continueImg == null)
        {
            return;
        }
        continueImg.enabled = value;
    }
}
