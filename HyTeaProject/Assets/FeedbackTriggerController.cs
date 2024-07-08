using UnityEngine;

public class FeedbackTriggerController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PresentationManager.Instance.FeedbackTriggerTriggered();
            gameObject.SetActive(false);
        }
    }
}
