using TMPro;
using UnityEngine;

public class DialogueUIController : MonoBehaviour
{
    [field: SerializeField] private TextMeshProUGUI nameText;
    [field: SerializeField] private TextMeshProUGUI dialogueText;
    [field: SerializeField] private TextMeshProUGUI continueText;
    
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
        continueText.enabled = value;
    }
}
