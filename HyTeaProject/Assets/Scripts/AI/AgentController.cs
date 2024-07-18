using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentController : MonoBehaviour
{
    public Transform dialogueCameraLookAt;
    [FormerlySerializedAs("name")] [field: SerializeField]private string agentName;
    
    private NavMeshAgent _navMeshAgent;
    private Coroutine _followCoroutine;
    
    private bool _canContinue = false;
    
    void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    public void FollowTarget(Transform target)
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
    }

    public void GiveFeedback(List<Feedback> feedbacks,DialogueUIController dialController )
    {
        StartCoroutine(GiveFeedbackCourotine(feedbacks,dialController));
    }

    private IEnumerator GiveFeedbackCourotine(List<Feedback> feedbacks,DialogueUIController dialController )
    {
        var coroutine= StartCoroutine(UIManager.Instance.CheckForMousePress());
        EventManager.MouseWasPressed.AddListener((() => _canContinue = true));
        
        if (feedbacks == null || dialController == null)
        {
            EventManager.FeedbackWasGiven.Invoke(this);
            yield break;
        }

        UIManager.Instance.ShowActiveCanvas(true);
        dialController.EnableContinueText(false);
        dialController.ChangeNameText(agentName);
        
        
        foreach (var feedback in feedbacks)
        {
            dialController.ChangeDialogueText(feedback.textFeedback);
            yield return new WaitForSeconds(1f);
            dialController.EnableContinueText(true);
            
            yield return new WaitUntil(() => _canContinue);
            _canContinue = false;
        }
        
        dialController.ChangeDialogueText("Thank you for the presentation");
        yield return new WaitForSeconds(1f);
        dialController.EnableContinueText(true);
            
        yield return new WaitUntil(() => _canContinue);
        _canContinue = false;
        
        EventManager.FeedbackWasGiven.Invoke(this);
        EventManager.MouseWasPressed.RemoveListener((() => _canContinue = true));
        if (coroutine != null) StopCoroutine(coroutine);
        UIManager.Instance.CloseUpperCanvas();
    }
}
