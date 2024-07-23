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
    }
    
    private void OnDisable()
    {
        EventManager.DialogueWasEnded.RemoveListener(HandleDialogueEnding);
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
    }

}
