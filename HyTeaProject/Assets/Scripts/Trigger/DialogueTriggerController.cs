using System.Linq;
using UnityEngine;

public class DialogueTriggerController : MonoBehaviour
{
    [SerializeField] private AgentController robotController;
    [SerializeField, TextArea] private string[] dialogueStrings;
    
    private void OnTriggerEnter(Collider other)
    {
        if (robotController == null)
        {
            Debug.LogWarning("Feedback trigger doesn't have the robot controller");
            return;
        }
        
        if (other.CompareTag("Player"))
        {
            robotController.DialogueStrings = dialogueStrings.ToList();
            AIManager.Instance.RobotTalkToPlayer(robotController);
            gameObject.SetActive(false);
        }
    }
}
