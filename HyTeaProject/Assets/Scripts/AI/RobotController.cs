using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum RobotFace
{
    Talking,
    Happy,
    Unhappy,
    Blink,
    none
}

[RequireComponent(typeof(AudioSource))]
public class RobotController : MonoBehaviour
{
    public Transform dialogueCameraLookAt;
    [field: SerializeField] private string agentName;
    [SerializeField] private Transform robotTransform;
    [SerializeField] private Renderer[] visualRepresentation;
    [SerializeField] private AudioClip[] voiceClip;
    [SerializeField] private Animator animator;
    
    [HideInInspector] public List<string> DialogueStrings = new();
    
    private AudioSource audioSource;
    private Coroutine _followCoroutine;
    
    private bool _canContinue = false;
    private int curTextIndex = 0;
    private string curFullText;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        HideRobot();                                                                           
    }

    public void ShowRobot()
    {
        if (visualRepresentation == null) return;
        foreach (var vRenderer in visualRepresentation)
        {
            vRenderer.enabled = true;
        }
    }
    
    public void HideRobot()
    {
        if (visualRepresentation == null) return;
        foreach (var vRenderer in visualRepresentation)
        {
            vRenderer.enabled = false;
        }
    }

    public void TeleportRobot(Transform transform)
    {
        if (robotTransform == null) return;

        robotTransform.position = transform.position;
        robotTransform.rotation = transform.rotation;
    }
    
    public void GiveFeedback(List<Feedback> feedbacks,DialogueUIController dialController)
    {
        var DialogueLines = feedbacks.Select(feedback => feedback.textFeedback).ToList();
        StartCoroutine(StartDialogueCoroutine(DialogueLines,dialController,true));
    }
    
    public void StartDialogue(List<string> dialogueLines, DialogueUIController dialController)
    {
        StartCoroutine(StartDialogueCoroutine(dialogueLines,dialController,true));
    }

    public IEnumerator StartDialogueCoroutine(List<string> dialogue, DialogueUIController dialController, bool isItFeedback = false )
    {
        var coroutine = StartCoroutine(UIManager.Instance.CheckForSpacePress());
        EventManager.SpaceWasPressed.AddListener(() => _canContinue = true);
        
        if (dialogue == null || dialController == null)
        {
            if(isItFeedback) EventManager.FeedbackWasGiven.Invoke(this);
            yield break;
        }

        dialController.EnableContinueText(false);
        dialController.ChangeNameText(agentName);
        
        
        foreach (var str in dialogue)
        {
            DisplayEmotion(RobotFace.Talking);
            yield return StartCoroutine(ShowDialogue(str, dialController));
            DisplayEmotion(RobotFace.Blink);
            _canContinue = false;
            dialController.EnableContinueText(true);
            yield return new WaitUntil(() => _canContinue);
            _canContinue = false;
        }

        if (isItFeedback) EventManager.FeedbackWasGiven.Invoke(this);
        EventManager.SpaceWasPressed.RemoveListener(() => _canContinue = true);
        
        
        if (coroutine != null) StopCoroutine(coroutine);
        GameManager.Instance.SetGameState(GameState.Walking);
        GameManager.Instance.UnlockPlayerCameraOnTarget(dialogueCameraLookAt);
    }

    public IEnumerator StartDialogueCoroutine1(DialogueSection dialogueSection, DialogueUIController dialController, bool isItFeedback = false )
    {
        var coroutine = StartCoroutine(UIManager.Instance.CheckForSpacePress());
        EventManager.SpaceWasPressed.AddListener(() => _canContinue = true);
        
        if (dialogueSection == null || dialController == null)
        {
            if(isItFeedback) EventManager.FeedbackWasGiven.Invoke(this);
            yield break;
        }

        dialController.EnableContinueText(false);
        dialController.ChangeNameText(agentName);
        
        foreach (var str in dialogueSection.dialogueLines)
        {
            DisplayEmotion(str.whileSpeaking);
            yield return StartCoroutine(ShowDialogue(str.dialogueStrings, dialController));
            DisplayEmotion(str.afterSpeaking);
            _canContinue = false;
            dialController.EnableContinueText(true);
            yield return new WaitUntil(() => _canContinue);
            _canContinue = false;
        }

        if (isItFeedback) EventManager.FeedbackWasGiven.Invoke(this);
        EventManager.SpaceWasPressed.RemoveListener(() => _canContinue = true);
        
        
        if (coroutine != null) StopCoroutine(coroutine);
        GameManager.Instance.SetGameState(GameState.Walking);
        GameManager.Instance.UnlockPlayerCameraOnTarget(dialogueCameraLookAt);
    }
    private IEnumerator ShowDialogue(string fullText, DialogueUIController dialController)
    {
        if (fullText == null || dialController == null || fullText == "") yield break;
        
        EventManager.MouseWasPressed.AddListener(() => _canContinue = true);

        var punctuation = true;
        
        dialController.EnableContinueText(false);
        
        curFullText = fullText;

        audioSource.loop = true;
        if (voiceClip.Length > 0)
        {
            audioSource.clip = voiceClip[Random.Range(0,voiceClip.Length)];
            audioSource.Play();
        }
        
        for (curTextIndex = 0; curTextIndex <= curFullText.Length; curTextIndex++)
        {
            var currentText = curFullText.Substring(0, curTextIndex);
            char lastChar = ' ';
            if (currentText.Length > 0) lastChar = currentText[^1];
            if (lastChar == '|')
            {
                punctuation = false;
                curFullText = curFullText.Remove(curTextIndex-1,1);
                currentText = curFullText.Substring(0, curTextIndex);
            }
            
            dialController.ChangeDialogueText(currentText);
            
            if (currentText.Length > 0 && punctuation)
            {
                lastChar = currentText[^1];

                switch (lastChar)
                {
                    case '.':
                        audioSource.Pause();
                        /*audioSource.loop = false;
                        yield return new WaitWhile(() => audioSource.isPlaying);*/
                        yield return new WaitForSeconds(0.5f);
                        //audioSource.loop = true;
                        audioSource.Play();
                        break;
                    case ',':
                        yield return new WaitForSeconds(0.2f);
                        break;
                    default:
                        yield return new WaitForSeconds(0.005f);
                        break;
                }

            }
            else
            {
                yield return new WaitForSeconds(0.005f);
                punctuation = true;
            }
        }
        
        audioSource.loop = false;
        yield return new WaitWhile(() => audioSource.isPlaying);
        audioSource.Stop();
        
        EventManager.MouseWasPressed.RemoveListener(() => _canContinue = true);
        _canContinue = false;
    }


    public void DisplayEmotion(RobotFace robotFace)
    {
        animator.SetTrigger(robotFace.ToString());
    }
        
        
        
    



}
