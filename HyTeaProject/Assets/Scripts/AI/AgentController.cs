using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentController : MonoBehaviour
{
    public Transform dialogueCameraLookAt;
    private NavMeshAgent _navMeshAgent;
    private Coroutine _followCoroutine;
    
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

            if (Vector3.Distance(transform.position,target.position) < 3f)
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

    public void StopFollowing()
    {
        if (_followCoroutine != null)
        {
            StopCoroutine(_followCoroutine);
            _followCoroutine = null;
        }
    }

    // Method to move to a specific position
    public void MoveTo(Vector3 position)
    {
        if (_followCoroutine != null)
        {
            StopCoroutine(_followCoroutine);
            _followCoroutine = null;
        }
        _navMeshAgent.SetDestination(position);
    }

    public void GiveFeedback(List<Feedback> feedbacks)
    {
        StartCoroutine(GiveFeedbackCourotine(feedbacks));
    }

    private IEnumerator GiveFeedbackCourotine(List<Feedback> feedbacks)
    {
        if (feedbacks == null)
        {
            EventManager.FeedbackWasGiven.Invoke(this);
            yield break;
        }
        
        foreach (var feedback in feedbacks)
        {
            Debug.Log(feedback.TextFeedback);
            yield return new WaitForSeconds(1f);
        }
        EventManager.FeedbackWasGiven.Invoke(this);
    }
}
