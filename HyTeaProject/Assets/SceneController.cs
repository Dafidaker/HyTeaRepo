using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField]private DialogueID dialogueID;
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
        DialogueManager.Instance.HandleStartDialogue(dialogueID);
    }

    private void HandleDialogueEnding(DialogueID _dialogueID)
    {
        if (_dialogueID != dialogueID ) return;
        
        MSceneManager.Instance.FadeToScene(nextSceneName);
        
    }
    
}
