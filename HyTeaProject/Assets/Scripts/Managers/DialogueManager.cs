using System;
using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class DialogueInformation
{
    [SerializeField] public Camera DialogueCamera;
}

[Serializable]
public class RobotInformation
{
    [SerializeField] public string robotID;
    [SerializeField] public RobotController RobotController;
    [HideInInspector] public DialogueUIController RobotUIController;
    
    [SerializeField] public Canvas RobotUIPrefab;
    
    [FormerlySerializedAs("RobotPosition")] [SerializeField] public Transform RobotGoTo;
    
}

[Serializable]
public class DialogueLine
{
    [FormerlySerializedAs("WhileSpeaking")] [SerializeField] public RobotFace whileSpeaking = RobotFace.Talking; 
    [SerializeField, TextArea] public string dialogueStrings;
    [FormerlySerializedAs("AfterSpeaking")] [SerializeField] public RobotFace afterSpeaking = RobotFace.Blink; 
}

[Serializable]
public class DialogueSection
{
    [SerializeField] public string robotID;
    [SerializeField] public DialogueLine[] dialogueLines; 
}

[Serializable]
public class Dialogue
{
    [SerializeField] public DialogueID dialogueID;
    [SerializeField] public DialogueInformation dialogueInfo;
    [SerializeField] public RobotInformation[] robotsInfo;
    [SerializeField] public DialogueSection[] dialogueStructure;
}

public enum DialogueID
{
    InitialDialogue,
    AfterCalibration,
    test,
    BeginningCalibration,
    
}

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private Dialogue[] dialogues;
    
    private GameObject DialogueUIPrefab;
    private AudioClip _talkingClip;

    private Transform[] RobotsPositions;


    [CanBeNull]
    private RobotController GetAgentControllerFromSpeakrsIndex(string id, RobotInformation[] speakersInfo)
    {
        foreach (RobotInformation speakerInfo in speakersInfo)
        {
            if (speakerInfo.robotID == id)
            {
                return speakerInfo.RobotController;
            }
        }

        return null;
    }

    private RobotInformation GetRobotInformationFromId(string id, RobotInformation[] robotsInfo)
    {
        foreach (var robot in robotsInfo)
        {
            if (robot.robotID == id)
            {
                return robot;
            }
        }

        return null;
    }

    public void HandleStartDialogue(DialogueID dialogueID)
    {
        Dialogue dialogue = GetDialogueFromID(dialogueID);
        
        if (dialogue == null)
        {
            EventManager.DialogueWasEnded.Invoke(dialogueID);
        }
        
        StartCoroutine(StartDialogue(dialogue));
    }
    
    public void HandleStartDialogue(Dialogue dialogue)
    {
        if (dialogue == null)
        {
            EventManager.DialogueWasEnded.Invoke(dialogue.dialogueID);
        }
        
        StartCoroutine(StartDialogue(dialogue));
    }
    
    public IEnumerator StartDialogue(Dialogue dialogue)
    {
        GameManager.Instance.SetGameState(GameState.Dialogue);
        
        GameManager.Instance.SetUpNewCamera(dialogue.dialogueInfo.DialogueCamera);

        foreach (var robot in dialogue.robotsInfo)
        {
            robot.RobotUIPrefab = UIManager.Instance.CreateDialogueCanvas(robot.RobotUIPrefab);
            robot.RobotUIController = UIManager.Instance.GetDialogueUIController(robot.RobotUIPrefab);
            robot.RobotController.HideRobot();
            if (robot.RobotGoTo != null) robot.RobotController.TeleportRobot(robot.RobotGoTo);
            else Debug.LogWarning(robot.robotID + " does not have a to go transform in the dialogue");
        }
        
        
        foreach (var dialogueSection in dialogue.dialogueStructure)
        {
            RobotInformation currentRobot = GetRobotInformationFromId(dialogueSection.robotID, dialogue.robotsInfo);
            
            if (currentRobot == null) continue;
            
            RobotController curRobot = currentRobot.RobotController;
            
            UIManager.Instance.ActivateCanvas(currentRobot.RobotUIPrefab);
            curRobot.ShowRobot();

            if (curRobot == null) continue;
            
            yield return StartCoroutine(curRobot.StartDialogueCoroutine1(dialogueSection,
                currentRobot.RobotUIController));
            
            //it uses the robot to get the transform for the player to lock into 
            //start coroutine that is responsible for showing the dialogue 
            UIManager.Instance.DeactivateCanvas(currentRobot.RobotUIPrefab);
        }
        
        
        foreach (var robot in dialogue.robotsInfo)
        {
            robot.RobotController.HideRobot();
        }
        
        EventManager.DialogueWasEnded.Invoke(dialogue.dialogueID);
        GameManager.Instance.SetGameState(GameState.Walking);
    }
    
    

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
    
    private void StopDialogue()
    {
        //unlocks player
    }

}
