using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MovementAnalysis : MonoBehaviour
{
    [SerializeField] private List<GestureHolder> _gesturesToAnalyse;
    [SerializeField] private Dictionary<GestureHolder, float> _gestureCooldowns = new Dictionary<GestureHolder, float>();
    
    private bool _analysisStarted;
    private bool _analysisCanBeStarted;
    public static event Action<GestureHolder> OnGestureDetected;

    private async void Start()
    { 
        await LandMarkProvider.Instance.WaitForLandMarkData();
        _analysisCanBeStarted = true;
        TryStartAnalysis();
        InitializeGestureCooldowns();
    }

    private async void TryStartAnalysis()
    {
        if (_analysisCanBeStarted && !_analysisStarted)
        {
            _analysisStarted = true;
            await StartTrackers();
        }
    }

    private async Task StartTrackers()
    {
        foreach (var gestureHolder in _gesturesToAnalyse)
        {
            foreach (var landMark in gestureHolder.GetNeededTrackers())
            {
                Debug.Log(landMark);
                await TrackingProvider.Instance.StartLandMarkTracker(landMark);
            }
        }
    }
    
    private void InitializeGestureCooldowns()
    {
        foreach (var gesture in _gesturesToAnalyse)
        {
            _gestureCooldowns.Add(gesture, 0);
        }
    }

    private void Update()
    {
        if (IsAnalysisRunning())
        {
            Analysis();
            HandleGestureCooldowns();
        }
    }

    private bool IsAnalysisRunning()
    {
        return _analysisStarted;
    }

    private void Analysis()
    {
        foreach (var gesture in _gesturesToAnalyse)
        {
            RunGestureAnalysis(gesture);
        }
    }

    private void RunGestureAnalysis(GestureHolder gestureHolder)
    {
        foreach (var gesture in gestureHolder.Gestures)
        {
            Tracker tracker = TrackingProvider.Instance.GetLandMarkTracker(gesture.LandMarkToTrack);
            List<Tracker.TimeStep> track = tracker.GetTimeStepsFromLastSeconds(gestureHolder.Duration);
            List<MotionDirection> foundDirections = TrackAnalysis.GetFoundDirectionsFromTrack(track, gesture.StepAnalysisParameters);
        
            if (!AnalyseForDirectionPercentages(gesture, foundDirections, track)) return;
            if (!AnalyseForPositionConditions(gesture, track, gestureHolder.Duration)) return;
            
        }
        if (!IsGestureOnCooldown(gestureHolder))
        {
            GestureDetected(gestureHolder);
        }
    }

    private bool AnalyseForDirectionPercentages(Gesture gesture,  List<MotionDirection> foundDirections, List<Tracker.TimeStep> track)
    {
        foreach (var directionPercentage in gesture.DirectionPercentages)
        {
            int directionCount = foundDirections.FindAll(e => e == directionPercentage.Direction).Count;
                
            // If one of the needed direction percentages is not met cancel the analysis
            if (! directionPercentage.CheckPercentageCondition(directionCount, track.Count))
            {
                return false;
            }
        }

        return true;
    }
    
    private bool AnalyseForPositionConditions(Gesture gesture, List<Tracker.TimeStep> objectTrack, float gestureHolderDuration)
    {
        foreach (var positionCondition in gesture.PositionConditions)
        {
            List<Tracker.TimeStep> conditionTrack =
                TrackingProvider.Instance.GetLandMarkTracker(positionCondition.Landmark).GetTimeStepsFromLastSeconds(gestureHolderDuration);
            if (!positionCondition.CheckPositionConditionOnTrack(objectTrack, conditionTrack))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsGestureOnCooldown(GestureHolder gesture)
    {
        return _gestureCooldowns[gesture] > 0;
    }
    
    private void GestureDetected(GestureHolder gesture)
    {
        OnGestureDetected?.Invoke(gesture);
        _gestureCooldowns[gesture] = gesture.Cooldown;
    }
    
    private void HandleGestureCooldowns()
    {
        foreach (var gesture in _gestureCooldowns.Keys.ToList())
        {
            _gestureCooldowns[gesture] -= Time.deltaTime;
        }
    }
    
    [Serializable]
    public enum MotionDirection
    {
        Left,
        Right,
        Up,
        Down,
        Unclear
    }

}
