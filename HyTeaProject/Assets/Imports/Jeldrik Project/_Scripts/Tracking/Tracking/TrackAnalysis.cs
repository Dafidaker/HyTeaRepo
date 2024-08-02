using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public static class TrackAnalysis
{
    
    [Serializable]
    public class StepAnalysisParameters
    {
        // Temporary solution - TODO: make step parameters depend on camera / setup
        [HideInInspector]
        public Vector2 XStepLimits = new Vector2(-0.005f, 0.005f);
        [HideInInspector]
        public Vector2 YStepLimits = new Vector2(-0.005f, 0.005f);
        [HideInInspector]
        public float VisibilityThreshold = 0.7f;
    }

    [Serializable]
    public class TrackStepInformation
    {
        public Tracker.TimeStep Step1;
        public Tracker.TimeStep Step2;
        public Vector3 Distance;
        public StepAnalysisParameters StepParameters;
        public List<MovementAnalysis.MotionDirection> Directions;

        public TrackStepInformation(Tracker.TimeStep step1, Tracker.TimeStep step2, StepAnalysisParameters stepParameters)
        {
            Step1 = step1;
            Step2 = step2;
            Distance = Step2.Position - Step1.Position;
            StepParameters = stepParameters;
            Directions = GetDirections();
        }

        private List<MovementAnalysis.MotionDirection> GetDirections()
        {
            List<MovementAnalysis.MotionDirection> foundDirections = new List<MovementAnalysis.MotionDirection>();
            
            // If not both steps are visible no motions should be detected
            if (! BothStepVisible())
            {
                foundDirections.Add(MovementAnalysis.MotionDirection.Unclear);
                return foundDirections;
            }
            
            // Vertical Motions
            if (Mathf.Abs(Distance.y) > StepParameters.YStepLimits.x && Mathf.Abs(Distance.y) > StepParameters.YStepLimits.y)
            {
                //Debug.Log(Distance.y > 0 ? "Greater" : "Lesser");

                foundDirections.Add(Distance.y > 0 ? MovementAnalysis.MotionDirection.Up : MovementAnalysis.MotionDirection.Down);
            }
            
            // Horizontal Motions
            if (Mathf.Abs(Distance.x) > StepParameters.XStepLimits.x && Mathf.Abs(Distance.x) > StepParameters.XStepLimits.y)
            {
                foundDirections.Add(Distance.x > 0 ? MovementAnalysis.MotionDirection.Right : MovementAnalysis.MotionDirection.Left);
            }

            if (foundDirections.Count == 0)
            {
                foundDirections.Add(MovementAnalysis.MotionDirection.Unclear);
            }
            
            return foundDirections;
        }
        
        private bool BothStepVisible()
        {
            return Step1.Visibility > StepParameters.VisibilityThreshold &&
                   Step1.Visibility > StepParameters.VisibilityThreshold;
        }
    }

    public static List<TrackStepInformation> GetStepInformationFromTrack(List<Tracker.TimeStep> track ,StepAnalysisParameters parameters)
    {
        List<TrackStepInformation> trackStepInformation = new List<TrackStepInformation>();
        
        for (int i = 0; i < track.Count - 1; i++)
        {
            trackStepInformation.Add(new TrackStepInformation(track[i], track[i+1], parameters));
        }

        return trackStepInformation;
    }
    
    private static List<MovementAnalysis.MotionDirection> GetFoundDirections(List<TrackStepInformation> trackStepInformation)
    {
        List<MovementAnalysis.MotionDirection> foundDirections = new List<MovementAnalysis.MotionDirection>();
        
        foreach (var step in trackStepInformation)
        {
            foundDirections.AddRange(step.Directions);
        }

        return foundDirections;
    }

    public static List<MovementAnalysis.MotionDirection> GetFoundDirectionsFromTrack(List<Tracker.TimeStep> track ,StepAnalysisParameters parameters)
    {
        List<TrackStepInformation> trackStepInformation = GetStepInformationFromTrack(track, parameters);
        return GetFoundDirections(trackStepInformation);
    }
}
