using UnityEngine;

public class IntroductionSceneController : MonoBehaviour
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
    
    [SerializeField] private string nextSceneName;
    
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
        DialogueManager.Instance.HandleStartDialogue(GetDialogueFromID(DialogueID.InitialDialogue));
    }

    private void HandleDialogueEnding(DialogueID _dialogueID)
    {
        if (_dialogueID != DialogueID.InitialDialogue ) return;
        
        MSceneManager.Instance.FadeToScene(nextSceneName);
        
    }
    
}
