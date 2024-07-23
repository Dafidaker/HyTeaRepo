using UnityEngine;

public class AfterCalibrationSceneController : MonoBehaviour
{
    [SerializeField] private Dialogue[] dialogues;
    void Start()
    {
        DialogueManager.Instance.HandleStartDialogue(GetDialogueFromID(DialogueID.AfterCalibration));
    }

    private void OnEnable()
    {
        EventManager.DialogueWasEnded.AddListener(HandleDialogueEnding);
    }
    
    private void OnDisable()
    {
        EventManager.DialogueWasEnded.RemoveListener(HandleDialogueEnding);
    }
    
    private void HandleDialogueEnding(DialogueID id)
    {
        if (id == DialogueID.AfterCalibration)
        {
            MSceneManager.Instance.FadeToNextScene();
        }
    }
    
    private Dialogue GetDialogueFromID(DialogueID dialogueID)
    {
        foreach (var dialogue in dialogues)
        {
            if (dialogue.dialogueID == dialogueID)
            {
                return dialogue;
            }

        }

        return null;
    }
}
