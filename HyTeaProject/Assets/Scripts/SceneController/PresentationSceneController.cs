using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentationSceneController : MonoBehaviour
{
    [SerializeField] private Dialogue[] dialogues;
    
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
    
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
