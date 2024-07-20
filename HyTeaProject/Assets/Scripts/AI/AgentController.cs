using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]
public class AgentController : MonoBehaviour
{
    public Transform dialogueCameraLookAt;
    [FormerlySerializedAs("name")] [field: SerializeField]private string agentName;
    [SerializeField] private AudioClip[] voiceClip;

    [TextArea]public List<string> DialogueStrings = new();
    
    private AudioSource audioSource;
    private Coroutine _followCoroutine;
    
    private bool _canContinue = false;
    private int curTextIndex = 0;
    private string curFullText;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    /*public void FollowTarget(Transform target)
    {
        if (_followCoroutine != null)
        {
            StopCoroutine(_followCoroutine);
        }
        _followCoroutine = StartCoroutine(FollowTargetCoroutine(target));
    }

    private IEnumerator FollowTargetCoroutine(Transform target)
    {
        while (true)
        {
            _navMeshAgent.SetDestination(target.position);

            if (Vector3.Distance(transform.position,target.position) <= _navMeshAgent.stoppingDistance)
            {
                EventManager.ArrivedAtTarget.Invoke(this,target);
                StopFollowing();
                yield break;
            }
            
            Vector3 direction = (_navMeshAgent.steeringTarget - transform.position).normalized;
            Vector3 rotationVector = new Vector3(direction.x, 0, direction.z);
            Quaternion lookRotation = default;
            if (rotationVector != Vector3.zero)
            {
                lookRotation = Quaternion.LookRotation(rotationVector);
            }
            
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _navMeshAgent.angularSpeed / 100);

            yield return null; 
        }
    }

    private void StopFollowing()
    {
        if (_followCoroutine != null)
        {
            StopCoroutine(_followCoroutine);
            _followCoroutine = null;
        }
    }

    public void MoveTo(Vector3 position)
    {
        if (_followCoroutine != null)
        {
            StopCoroutine(_followCoroutine);
            _followCoroutine = null;
        }
        _navMeshAgent.SetDestination(position);
    }*/

    public void GiveFeedback(List<Feedback> feedbacks,DialogueUIController dialController)
    {
        var DialogueLines = feedbacks.Select(feedback => feedback.textFeedback).ToList();
        StartCoroutine(StartDialogueCoroutine(DialogueLines,dialController,true));
    }
    
    public void StartDialogue(List<string> dialogueLines, DialogueUIController dialController)
    {
        StartCoroutine(StartDialogueCoroutine(dialogueLines,dialController,true));
    }

    private IEnumerator StartDialogueCoroutine(List<string> dialogue, DialogueUIController dialController, bool isItFeedback = false )
    {
        var coroutine = StartCoroutine(UIManager.Instance.CheckForMousePress());
        EventManager.MouseWasPressed.AddListener(() => _canContinue = true);
        
        if (dialogue == null || dialController == null)
        {
            if(isItFeedback) EventManager.FeedbackWasGiven.Invoke(this);
            yield break;
        }

        UIManager.Instance.ShowActiveCanvas(true);
        dialController.EnableContinueText(false);
        dialController.ChangeNameText(agentName);
        
        
        foreach (var str in dialogue)
        {
            /*dialController.ChangeDialogueText(str);
            yield return new WaitForSeconds(1f);
            dialController.EnableContinueText(true);
            
            yield return new WaitUntil(() => _canContinue);
            _canContinue = false;*/

            yield return StartCoroutine(ShowDialogue(str, dialController));
            _canContinue = false;
            dialController.EnableContinueText(true);
            yield return new WaitUntil(() => _canContinue);
            _canContinue = false;
        }
        
        /*dialController.ChangeDialogueText("Thank you for the presentation");
        yield return new WaitForSeconds(1f);
        dialController.EnableContinueText(true);
            
        yield return new WaitUntil(() => _canContinue);
        _canContinue = false;*/

        if (isItFeedback) EventManager.FeedbackWasGiven.Invoke(this);
        EventManager.MouseWasPressed.RemoveListener(() => _canContinue = true);
        
        if (coroutine != null) StopCoroutine(coroutine);
        GameManager.Instance.UnlockPlayerCameraOnTarget(dialogueCameraLookAt);
        UIManager.Instance.CloseUpperCanvas();
    }

    private IEnumerator ShowDialogue(string fullText, DialogueUIController dialController)
    {
        EventManager.MouseWasPressed.AddListener((() => _canContinue = true));
        
        dialController.EnableContinueText(false);
        
        curFullText = fullText;

        if (voiceClip.Length > 0)
        {
            audioSource.clip = voiceClip[Random.Range(0,voiceClip.Length)];
            audioSource.Play();
        }
        
        for (curTextIndex = 0; curTextIndex <= curFullText.Length; curTextIndex++)
        {
            if (_canContinue & curTextIndex <= curFullText.Length * 0.2)
            {
                curTextIndex = curFullText.Length;
            }
            
            var currentText = curFullText.Substring(0, curTextIndex);
            dialController.ChangeDialogueText(currentText);
            
            if (currentText.Length > 0)
            {
                char lastChar = currentText[^1];

                switch (lastChar)
                {
                    case '.':
                        audioSource.Pause();
                        yield return new WaitForSeconds(1f);
                        audioSource.Play();
                        break;
                    case ',':
                        audioSource.Pause();
                        yield return new WaitForSeconds(0.3f);
                        audioSource.Play();
                        break;
                    default:
                        yield return new WaitForSeconds(0.005f);
                        break;
                }
            }
            else
            {
                yield return new WaitForSeconds(0.005f);
            }
        }
        audioSource.Stop();
        
        EventManager.MouseWasPressed.RemoveListener((() => _canContinue = true));
        yield return null;
    }
    
    
    
        
        
        
    



}
