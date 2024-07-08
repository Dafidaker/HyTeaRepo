using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIManager : Singleton<AIManager>
{
    [field: SerializeField] private List<AgentController> aiAgents;
    private AgentController feedbackRobot;

    private void Awake()
    {
        aiAgents = new List<AgentController>();
    }

    public void InitiateFeedback()
    {
        GameObject Player = GameManager.Instance.Player;
        if (Player == null) return;
        
        EventManager.ArrivedAtTarget.AddListener(RobotArrivedThePlayer);
        EventManager.FeedbackWasGiven.AddListener(FeedbackHasBeenGiven);
        
        SetFeedbackRobot();
        SendRobotToFollowTarget(Player.transform, feedbackRobot);
    }

    public void RobotArrivedThePlayer(AgentController robot, Transform playerTransform)
    {
        GameObject Player = GameManager.Instance.Player;
        if (Player == null) return;
        
        if (robot == feedbackRobot && playerTransform == Player.transform)
        {
            GameManager.Instance.LockPlayerCameraOnTarget(robot.dialogueCameraLookAt);
            robot.GiveFeedback(PresentationManager.Instance._feedbacks);
            EventManager.ArrivedAtTarget.RemoveListener(RobotArrivedThePlayer);
        }
        
    }

    public void FeedbackHasBeenGiven(AgentController robot)
    {
        if (feedbackRobot == null && robot != feedbackRobot) return;
        
        EventManager.FeedbackWasGiven.RemoveListener(FeedbackHasBeenGiven);
        
        GameManager.Instance.UnlockPlayerCameraOnTarget(feedbackRobot.dialogueCameraLookAt);
    }

    public void SendRandomRobotToPosition(Vector3 position)
    {
        if (aiAgents.Count <= 0)
        {
            if (FindObjectsOfType(typeof(AgentController)) is AgentController[] { Length: > 0 } agentControllers) aiAgents.AddRange(agentControllers);
            else  return;
        }

        aiAgents[Random.Range(0,aiAgents.Count)].GetComponent<AgentController>().MoveTo(position);
    }
    
    public AgentController SendRobotToFollowTarget(Transform targetTransform, AgentController robot = null)
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
    }

    public AgentController SetFeedbackRobot(AgentController robot = null)
    {
        feedbackRobot = robot;

        if (feedbackRobot == null)
        {
            feedbackRobot = GetRandomRobot();
        }

        return feedbackRobot;
    }

    public AgentController GetRandomRobot()
    {
        if (aiAgents.Count > 0) return aiAgents[Random.Range(0, aiAgents.Count)];
        
        if (FindObjectsOfType(typeof(AgentController)) is AgentController[] { Length: > 0 } agentControllers) aiAgents.AddRange(agentControllers);
        else  return null;

        return aiAgents[Random.Range(0, aiAgents.Count)];
    }
    
}

