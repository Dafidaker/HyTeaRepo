using System;
using System.Linq;
using UnityEngine;


/*[Serializable]
public class DialogueInformation
{
    [SerializeField] public Camera DialogueCamera;
}

[Serializable]
public class RobotInformation
{
    [SerializeField] public int robotID;
    [SerializeField] public RobotController RobotController;
    [SerializeField] public Transform RobotPosition;
    [SerializeField] public Canvas RobotUIPrefab;
    [HideInInspector] public DialogueUIController RobotUIController;
}


[Serializable]
public class DialogueSection
{
    [SerializeField] public int robotID;
    [SerializeField, TextArea] public string[] dialogueLines;
}*/


public class DialogueTriggerController : MonoBehaviour
{
    //[SerializeField] private RobotController robotController;
    [SerializeField] private DialogueInformation dialogueInfo;
    [SerializeField] private RobotInformation[] speakersInfo;
    [SerializeField] private DialogueSection[] dialogueStructure;

    

    private void OnTriggerEnter(Collider other)
    {
        if (dialogueInfo == null || dialogueStructure == null)
        {
            Debug.LogWarning("Information need to be filled in in " + gameObject.name);
            return;
        }
        
        if (other.CompareTag("Player"))
        {
            //robotController.DialogueStrings = dialogueLines.ToList();
            //DialogueManager.Instance.HandleStartDialogue(dialogueInfo,speakersInfo,dialogueStructure);
            gameObject.SetActive(false);
        }
    }
}
