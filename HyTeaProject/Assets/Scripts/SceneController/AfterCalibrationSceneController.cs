using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterCalibrationSceneController : MonoBehaviour
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
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
