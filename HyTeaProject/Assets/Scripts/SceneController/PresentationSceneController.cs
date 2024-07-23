using System.Collections.Generic;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEngine;

public class PresentationSceneController : MonoBehaviour
{
    [SerializeField] private Dialogue[] dialogues;
    [SerializeField] private PresentationStartSettings _settings;
    [SerializeField] private Camera playerCam;
    
    void Start()
    {
        DialogueManager.Instance.HandleStartDialogue(GetDialogueFromID(DialogueID.PresentingPlayer));
    }

    private void OnEnable()
    {
        EventManager.DialogueWasEnded.AddListener(HandleDialogueEnding);
        EventManager.FeedbackHasBeenCreated.AddListener(HandleFeedbackWasCreated);
    }

    private void HandleFeedbackWasCreated()
    {
        var feedbackDialogue = GetDialogueFromID(DialogueID.Feedback);
        string robotId = feedbackDialogue.robotsInfo[0].robotID;

        if (PresentationManager.Instance._feedbacks == null || robotId == null) return;

        List<DialogueLine> dialogueLines = new List<DialogueLine>();      
        
        foreach (var feedback in PresentationManager.Instance._feedbacks)
        {
            var newDialogueLine = new DialogueLine
            {
                dialogueStrings = feedback.textFeedback
            };

            dialogueLines.Add(newDialogueLine);
        }
        
        DialogueSection[] newDialogueSections = new[]
        {
            new DialogueSection { robotID = robotId, dialogueLines = dialogueLines.ToArray() }
        };
        
        feedbackDialogue.dialogueStructure = newDialogueSections;
        
        DialogueManager.Instance.HandleStartDialogue(GetDialogueFromID(DialogueID.Feedback));
    }

    private void OnDisable()
    {
        EventManager.DialogueWasEnded.RemoveListener(HandleDialogueEnding);
        EventManager.FeedbackHasBeenCreated.RemoveListener(HandleFeedbackWasCreated);
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
        if (id == DialogueID.PresentingPlayer)
        {
            DialogueManager.Instance.HandleStartDialogue(GetDialogueFromID(DialogueID.BeforePresentation));
        }
        else if (id == DialogueID.BeforePresentation)
        {
            if (playerCam != null) GameManager.Instance.SetUpNewCamera(playerCam);
            else Debug.LogWarning("player cam is not assigned in the " + gameObject.name);
            //GameManager.Instance.SetGameState(GameState.Presentation);
            GameManager.Instance.StartPresentation(_settings);
        }
        else if  (id == DialogueID.Feedback)
        {
            MSceneManager.Instance.FadeToNextScene();
        }
    }

}
