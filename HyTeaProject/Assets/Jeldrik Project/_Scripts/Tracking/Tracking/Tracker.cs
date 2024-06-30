using System;
using System.Collections.Generic;
using Mediapipe;
using UnityEngine;

public class Tracker
{
    [Serializable]
    public class TimeStep
    {
        public Vector3 Position { get; private set; }
        public float Time { get; private set; }

        public float Visibility;

        public TimeStep(Vector3 position, float time, float visibility)
        {
            Position = position;
            Time = time;
            Visibility = visibility;
        }
    }

    private readonly List<TimeStep> _timeSteps = new List<TimeStep>();

    protected bool _trackingStarted;
    
    // How many timesteps should be kept in the track
    protected int _trackedStepAmount; 
    // How long to wait between timesteps
    protected float _trackingInterval; 
    
    public virtual void StartTracking()
    {
        Debug.LogError("Base Class shouldn't be called");
    }

    public void StopTracking()
    {
        _trackingStarted = false;
    }
    
    public virtual void RunTracker()
    {
        Debug.LogError("Base Class shouldn't be called");
    }
    
    protected void AddTimeStep(Vector3 position, float time, float visibility)
    {
        LimitTimeStepsListSize();
        _timeSteps.Add( new TimeStep(position, time,visibility ));
    }
    
    private void LimitTimeStepsListSize()
    {
        if (_timeSteps.Count > _trackedStepAmount)
        {
            _timeSteps.RemoveAt(0);
        }
    }

    protected bool NextTrackingTimeReached()
    {
        return (_timeSteps[^1].Time + _trackingInterval < Time.time);
    }

    protected bool IsTrackingAllowed()
    {
        return _trackingStarted;
    }
    
    public List<TimeStep> GetTimeSteps()
    {
        return _timeSteps;
    }

    public List<TimeStep> GetTimeStepsFromLastSeconds(float seconds)
    {
        List<TimeStep> timeSteps = new List<TimeStep>();
        foreach (var timeStep in _timeSteps)
        {
            if (timeStep.Time > Time.time - seconds)
            {
                timeSteps.Add(timeStep);
            }
        }

        return timeSteps;
    }
    
    public float GetTrackDuration()
    {
        return _timeSteps[^1].Time - _timeSteps[0].Time;
    }
}

public class GameObjectTracker : Tracker 
{
    private readonly GameObject _objectToTrack;
    
    public GameObjectTracker(GameObject objectToTrack, int trackedStepAmount, float trackingInterval)
    {
        _objectToTrack = objectToTrack;
        _trackedStepAmount = trackedStepAmount;
        _trackingInterval = trackingInterval;
    }

    public override void StartTracking()
    {
        _trackingStarted = true;
        AddTimeStep(_objectToTrack.transform.position, Time.time, 0);
    }

    public override void RunTracker()
    {
        if (!IsTrackingAllowed()) return;
            
        if (NextTrackingTimeReached())
        {
            float visibility = _objectToTrack.activeInHierarchy ? 1 : 0;
            AddTimeStep(_objectToTrack.transform.position, Time.time, visibility);
        }
    }

    public GameObject GetTrackedObject()
    {
        return _objectToTrack;
    }
}

public class LandMarkTracker : Tracker
{
    private readonly PoseTrackingInfo.LandmarkNames _landmarkToTrack;
    
    public LandMarkTracker( PoseTrackingInfo.LandmarkNames landmarkToTrack, int trackedStepAmount, float trackingInterval)
    {
        _landmarkToTrack = landmarkToTrack;
        _trackedStepAmount = trackedStepAmount;
        _trackingInterval = trackingInterval;
    }
    
    public override void StartTracking()
    {
        _trackingStarted = true;
        
        AddTimeStep(GetVectorFromTrackedLandMark(), Time.time, 0);
    }

    private Vector3 GetVectorFromTrackedLandMark()
    {
        return LandMarkProvider.Instance.VectorLandmarkList[PoseTrackingInfo.LandmarkIndexes[_landmarkToTrack]];
    }

    private float GetVisibilityFromTrackedLandMark()
    {
        return LandMarkProvider.Instance.LandmarkList.Landmark[PoseTrackingInfo.LandmarkIndexes[_landmarkToTrack]].Visibility;
    }
    
    public override void RunTracker()
    {
        if (!IsTrackingAllowed()) return;
        
        if (NextTrackingTimeReached())
        {
            AddTimeStep(GetVectorFromTrackedLandMark(), Time.time, GetVisibilityFromTrackedLandMark());
        }
    }
    
    public PoseTrackingInfo.LandmarkNames GetTrackedLandMark()
    {
        return _landmarkToTrack;
    }
}
