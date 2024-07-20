using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIManager : Singleton<AIManager>
{
    [field: SerializeField] private List<AgentController> aiAgents;
    private AgentController feedbackRobot;

    protected override void Awake()
    {
        base.Awake();
        aiAgents = new List<AgentController>();
    }

    public void InitiateFeedback(AgentController robot)
    {
        GameObject player = GameManager.Instance.Player;
        if (player == null) return;
        
        //EventManager.ArrivedAtTarget.AddListener(RobotArrivedThePlayer);
        EventManager.FeedbackWasGiven.AddListener(FeedbackHasBeenGiven);
        
        SetFeedbackRobot(robot);
        
        GameManager.Instance.LockPlayerCameraOnTarget(robot.dialogueCameraLookAt);
        /*SendRobotToFollowTarget(player.transform, feedbackRobot);*/
        feedbackRobot.GiveFeedback(PresentationManager.Instance._feedbacks, UIManager.Instance.CreateDialogueCanvas());
    }

    public void RobotTalkToPlayer(AgentController robot)
    {
        GameManager.Instance.LockPlayerCameraOnTarget(robot.dialogueCameraLookAt);
        robot.StartDialogue(robot.DialogueStrings,UIManager.Instance.CreateDialogueCanvas());
    }

    private void RobotArrivedThePlayer(AgentController robot, Transform playerTransform)
    {
        GameObject Player = GameManager.Instance.Player;
        if (Player == null) return;
        
        if (robot == feedbackRobot && playerTransform == Player.transform)
        {
            GameManager.Instance.LockPlayerCameraOnTarget(robot.dialogueCameraLookAt);
            robot.GiveFeedback(PresentationManager.Instance._feedbacks, UIManager.Instance.CreateDialogueCanvas());
            EventManager.ArrivedAtTarget.RemoveListener(RobotArrivedThePlayer);
        }
        
    }

    private void FeedbackHasBeenGiven(AgentController robot)
    {
        if (feedbackRobot == null && robot != feedbackRobot) return;
        
        EventManager.FeedbackWasGiven.RemoveListener(FeedbackHasBeenGiven);
        
        GameManager.Instance.UnlockPlayerCameraOnTarget(feedbackRobot.dialogueCameraLookAt);
    }

   /* public void SendRandomRobotToPosition(Vector3 position)
    {
        if (aiAgents.Count <= 0)
        {
            if (FindObjectsOfType(typeof(AgentController)) is AgentController[] { Length: > 0 } agentControllers) aiAgents.AddRange(agentControllers);
            else  return;
        }

        aiAgents[Random.Range(0,aiAgents.Count)].GetComponent<AgentController>().MoveTo(position);
    }

    private AgentController SendRobotToFollowTarget(Transform targetTransform, AgentController robot = null)
    {
        AgentController selectedController = robot;

        if (selectedController == null)
        {
            selectedController = GetRandomRobot();
        }

        if (selectedController == null)
        {
            return selectedController;
        }
        
        selectedController.GetComponent<AgentController>().FollowTarget(targetTransform);

        return selectedController;
    }*/

    private AgentController SetFeedbackRobot(AgentController robot = null)
    {
        feedbackRobot = robot;

        if (feedbackRobot == null)
        {
            feedbackRobot = GetRandomRobot();
        }

        return feedbackRobot;
    }

    private AgentController GetRandomRobot()
    {
        if (aiAgents.Count > 0) return aiAgents[Random.Range(0, aiAgents.Count)];
        
        if (FindObjectsOfType(typeof(AgentController)) is AgentController[] { Length: > 0 } agentControllers) aiAgents.AddRange(agentControllers);
        else  return null;

        return aiAgents[Random.Range(0, aiAgents.Count)];
    }
    
}

