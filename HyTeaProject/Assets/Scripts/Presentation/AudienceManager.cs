using System.Collections.Generic;
using UnityEngine;


public enum RobotsEmotion
{
    
}

public class AudienceManager : MonoBehaviour
{
    private List<GameObject> _robots;

    
    /// <summary>
    /// Makes the part of the audience display a emotion
    /// </summary>
    /// <param name="emotion">The emotions the robots will present</param>
    /// <param name="percentage">The percentage of the audience you want to should that emotion(from 0 to 100)</param>
    public void ReactionFromRobots(RobotsEmotion emotion, int percentage)
    {
        percentage = Mathf.Clamp(percentage, 0, 100);

        var numberOfRobots = _robots.Count * (percentage / 100);

        if (numberOfRobots <= 0) return;

        var selectedRobots = new List<GameObject>(_robots);

        var numberOfRobotsToRemove = _robots.Count - numberOfRobots;
        
        for (var i = 0; i < numberOfRobotsToRemove; i++)
        {
            selectedRobots.Remove(selectedRobots[Random.Range(0,selectedRobots.Count)]);
        }
        
        //todo 
        //robot.playAnimation(of emotion x)
    }
}
