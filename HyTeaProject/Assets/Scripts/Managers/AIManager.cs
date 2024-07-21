using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIManager : Singleton<AIManager>
{
    [field: SerializeField] private List<RobotController> aiAgents;
    private RobotController feedbackRobot;

    protected override void Awake()
    {
        base.Awake();
        aiAgents = new List<RobotController>();
    }

    public void InitiateFeedback(RobotController robot)
    {
        GameObject player = GameManager.Instance.Player;
        if (player == null) return;
        
        EventManager.FeedbackWasGiven.AddListener(FeedbackHasBeenGiven);
        
        SetFeedbackRobot(robot);
        
        GameManager.Instance.SetGameState(GameState.Dialogue);
        GameManager.Instance.LockPlayerCameraOnTarget(robot.dialogueCameraLookAt);

        Canvas temp = UIManager.Instance.CreateDialogueCanvas();
        
        feedbackRobot.GiveFeedback(PresentationManager.Instance._feedbacks, UIManager.Instance.GetDialogueUIController(temp));
    }

    public void RobotTalkToPlayer(RobotController robot)
    {
        GameManager.Instance.SetGameState(GameState.Dialogue);
        GameManager.Instance.LockPlayerCameraOnTarget(robot.dialogueCameraLookAt);
        
        Canvas temp = UIManager.Instance.CreateDialogueCanvas();
        
        robot.StartDialogue(robot.DialogueStrings,UIManager.Instance.GetDialogueUIController(temp));
    }
    
    private void RobotArrivedThePlayer(RobotController robot, Transform playerTransform)
    {
        GameObject Player = GameManager.Instance.Player;
        if (Player == null) return;
        
        if (robot == feedbackRobot && playerTransform == Player.transform)
        {
            GameManager.Instance.LockPlayerCameraOnTarget(robot.dialogueCameraLookAt);
            
            Canvas temp = UIManager.Instance.CreateDialogueCanvas();
            
            robot.GiveFeedback(PresentationManager.Instance._feedbacks, UIManager.Instance.GetDialogueUIController(temp));
            EventManager.ArrivedAtTarget.RemoveListener(RobotArrivedThePlayer);
        }
        
    }

    private void FeedbackHasBeenGiven(RobotController robot)
    {
        if (feedbackRobot == null && robot != feedbackRobot) return;
        
        EventManager.FeedbackWasGiven.RemoveListener(FeedbackHasBeenGiven);
        
        GameManager.Instance.UnlockPlayerCameraOnTarget(feedbackRobot.dialogueCameraLookAt);
    }

   /* public void SendRandomRobotToPosition(Vector3 position)
    {
        if (aiAgents.Count <= 0)
        {
            if (FindObjectsOfType(typeof(RobotController)) is RobotController[] { Length: > 0 } agentControllers) aiAgents.AddRange(agentControllers);
            else  return;
        }

        aiAgents[Random.Range(0,aiAgents.Count)].GetComponent<RobotController>().MoveTo(position);
    }

    private RobotController SendRobotToFollowTarget(Transform targetTransform, RobotController robot = null)
    {
        RobotController selectedController = robot;

        if (selectedController == null)
        {
            selectedController = GetRandomRobot();
        }

        if (selectedController == null)
        {
            return selectedController;
        }
        
        selectedController.GetComponent<RobotController>().FollowTarget(targetTransform);

        return selectedController;
    }*/

    private RobotController SetFeedbackRobot(RobotController robot = null)
    {
        feedbackRobot = robot;

        if (feedbackRobot == null)
        {
            feedbackRobot = GetRandomRobot();
        }

        return feedbackRobot;
    }

    private RobotController GetRandomRobot()
    {
        if (aiAgents.Count > 0) return aiAgents[Random.Range(0, aiAgents.Count)];
        
        if (FindObjectsOfType(typeof(RobotController)) is RobotController[] { Length: > 0 } agentControllers) aiAgents.AddRange(agentControllers);
        else  return null;

        return aiAgents[Random.Range(0, aiAgents.Count)];
    }
    
}

