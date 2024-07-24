using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondLevelSceneController : MonoBehaviour
{
    [SerializeField] private Dialogue[] dialogues;
    
    private void OnEnable()
    {
        EventManager.DialogueWasEnded.AddListener(HandleDialogueEnding);
    }
    
    private void OnDisable()
    {
        EventManager.DialogueWasEnded.RemoveListener(HandleDialogueEnding);
    }

    private void Start()
    {
        GameManager.Instance.SetGameState(GameState.Dialogue);
        DialogueManager.Instance.HandleStartDialogue(GetDialogueFromID(DialogueID.SecondLevelIntroduction));
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
    
    private void HandleDialogueEnding(DialogueID id)
    {
        if (id == DialogueID.SecondLevelIntroduction)
        {
            MSceneManager.Instance.FadeToScene("01_MainMenu"); 
        }
    }
}
