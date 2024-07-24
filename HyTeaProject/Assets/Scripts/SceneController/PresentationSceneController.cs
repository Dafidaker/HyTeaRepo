using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PresentationSceneController : MonoBehaviour
{
    [SerializeField] private Dialogue[] dialogues;
    [FormerlySerializedAs("_settings")] [SerializeField] private PresentationStartSettings settings;
    [SerializeField] private Camera playerCam;
    [SerializeField] private Transform Notes;
    
    void Start()
    {
        DialogueManager.Instance.HandleStartDialogue(GetDialogueFromID(DialogueID.PresentingPlayer));
    }

    private void OnEnable()
    {
        EventManager.DialogueWasEnded.AddListener(HandleDialogueEnding);
        EventManager.FeedbackHasBeenCreated.AddListener(HandleFeedbackWasCreated);
        EventManager.PresentationHasEnded.AddListener(HandlePresentationEnded);
    }
    private void OnDisable()
    {
        EventManager.DialogueWasEnded.RemoveListener(HandleDialogueEnding);
        EventManager.FeedbackHasBeenCreated.RemoveListener(HandleFeedbackWasCreated);
        EventManager.PresentationHasEnded.RemoveListener(HandlePresentationEnded);
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
    
    private void HandlePresentationEnded()
    {
        if (Notes != null)  Notes.gameObject.SetActive(false);
        else Debug.LogWarning("the notes tranform is not set in " + gameObject.name);
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
            if (Notes != null)  Notes.gameObject.SetActive(true);
            else Debug.LogWarning("the notes tranform is not set in " + gameObject.name);
            GameManager.Instance.PrepareforPresentation(settings);
        }
        else if  (id == DialogueID.Feedback)
        {
            MSceneManager.Instance.FadeToNextScene();
        }
    }

}
