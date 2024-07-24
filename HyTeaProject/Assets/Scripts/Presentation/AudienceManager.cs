using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudienceManager : Singleton<AudienceManager>
{
    [SerializeField]private List<RobotController> _robots;

    private Coroutine handlingEmotion;

    /// <summary>
    /// Makes the part of the audience display a emotion
    /// </summary>
    /// <param name="emotion">The emotions the robots will present</param>
    /// <param name="percentage">The percentage of the audience you want to should that emotion(from 0 to 100)</param>
    private void ReactionFromRobots(RobotFace emotion, int percentage)
    {
        percentage = Mathf.Clamp(percentage, 0, 100);

        int numberOfRobots = (int)(_robots.Count * (percentage / 100f));

        if (numberOfRobots <= 0) return;

        var selectedRobots = new List<RobotController>(_robots);

        var numberOfRobotsToRemove = _robots.Count - numberOfRobots;

        for (var i = 0; i < numberOfRobotsToRemove; i++)
        {
            selectedRobots.Remove(selectedRobots[Random.Range(0, selectedRobots.Count)]);
        }

        foreach (var robot in selectedRobots)
        {
            robot.DisplayEmotion(emotion);
        }
    }

    public void HandleMakingReactionFromRobots(RobotFace emotion, int percentage, float duration)
    {
        if (handlingEmotion != null) return;
        handlingEmotion = StartCoroutine(MakingReactionFromRobots(emotion, percentage, duration));
    }

    private IEnumerator MakingReactionFromRobots(RobotFace emotion, int percentage, float duration)
    {
        ReactionFromRobots(emotion, percentage);

        yield return new WaitForSeconds(duration);

        ReactionFromRobots(RobotFace.Blink, 100);

        handlingEmotion = null;

    }
    
}
