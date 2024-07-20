using UnityEngine;

public class FeedbackTriggerController : MonoBehaviour
{
    [SerializeField] private AgentController robotController;
    
    private void OnTriggerEnter(Collider other)
    {
        if (robotController == null)
        {
            Debug.LogWarning("Feedback trigger doesn't have the robot controller");
            return;
        }
        
        if (other.CompareTag("Player"))
        {
            AIManager.Instance.InitiateFeedback(robotController);
            gameObject.SetActive(false);
        }
    }
}
