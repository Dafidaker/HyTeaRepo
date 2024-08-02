using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PresentationSceneController : MonoBehaviour
{
    [SerializeField] private Dialogue[] dialogues;
    [FormerlySerializedAs("_settings")] [SerializeField] private PresentationStartSettings settings;
    [SerializeField] private Camera playerCam;
    [SerializeField] private Transform Notes;
    [SerializeField] private GameObject PresentationLight;
    [SerializeField] private GameObject PresentationButtons;
    [SerializeField] private GameObject PrePresentationButtonsUI;
    void Start()
    {
        DialogueManager.Instance.HandleStartDialogue(GetDialogueFromID(DialogueID.PresentingPlayer));
    }

    private void OnEnable()
    {
        EventManager.DialogueWasEnded.AddListener(HandleDialogueEnding);
        EventManager.FeedbackHasBeenCreated.AddListener(HandleFeedbackWasCreated);
        EventManager.PresentationHasEnded.AddListener(HandlePresentationEnded);
        EventManager.PresentationHasStarted.AddListener(HandlePresentationStarted);
    }

    private void HandlePresentationStarted()
    {
        if (PresentationLight != null)  PresentationLight.SetActive(true);
        else Debug.LogWarning("the PresentationLight is not set in " + gameObject.name);
        
        if (PrePresentationButtonsUI != null)  PrePresentationButtonsUI.SetActive(false);
        else Debug.LogWarning("the PrePresentationButtonsUI is not set in " + gameObject.name);
        
        if (PresentationButtons != null)  PresentationButtons.SetActive(true);
        else Debug.LogWarning("the PresentationButtons is not set in " + gameObject.name);
    }

    private void OnDisable()
    {
        EventManager.DialogueWasEnded.RemoveListener(HandleDialogueEnding);
        EventManager.FeedbackHasBeenCreated.RemoveListener(HandleFeedbackWasCreated);
        EventManager.PresentationHasStarted.RemoveListener(HandlePresentationStarted);
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
        
        if (PresentationLight != null)  PresentationLight.SetActive(false);
        else Debug.LogWarning("the PresentationLight is not set in " + gameObject.name);
        
        if (PresentationButtons != null)  PresentationButtons.SetActive(false);
        else Debug.LogWarning("the PresentationButtons is not set in " + gameObject.name);
        
    }
    
    private void HandleDialogueEnding(DialogueID id)
    {
        if (id == DialogueID.PresentingPlayer)
        {
            DialogueManager.Instance.HandleStartDialogue(GetDialogueFromID(DialogueID.BeforePresentation));
        }
        else if (id == DialogueID.BeforePresentation)
        {
            if (PrePresentationButtonsUI != null)  PrePresentationButtonsUI.SetActive(true);
            else Debug.LogWarning("the PrePresentationButtonsUI is not set in " + gameObject.name);
            
            if (playerCam != null) GameManager.Instance.SetUpNewCamera(playerCam);
            else Debug.LogWarning("player cam is not assigned in the " + gameObject.name);
            
            if (Notes != null)  Notes.gameObject.SetActive(true);
            else Debug.LogWarning("the notes transform is not set in " + gameObject.name);
            
            GameManager.Instance.PrepareforPresentation(settings);
        }
        else if  (id == DialogueID.Feedback)
        {
            MSceneManager.Instance.FadeToNextScene();
        }
    }

}
