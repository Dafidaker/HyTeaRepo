using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TrackingProvider : MonoBehaviour
{
    public static TrackingProvider Instance { get; private set; }

    private readonly List<Tracker> _allTrackers = new List<Tracker>();
    private readonly List<LandMarkTracker> _landMarkTrackers = new List<LandMarkTracker>();

    [SerializeField] private int _defaultTrackSize;
    [SerializeField] private float _defaultTrackingInterval;
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        InitializeTrackerProvider();
    }

    private void Update()
    {
        RunTrackers();
    }

    private void RunTrackers()
    {
        foreach (var tracker in _allTrackers)
        {
            tracker.RunTracker();
        }
    }
    
    private void InitializeTrackerProvider()
    {
        foreach (PoseTrackingInfo.LandmarkNames landmarkName in Enum.GetValues(typeof(PoseTrackingInfo.LandmarkNames)))
        {
            _landMarkTrackers.Add(new LandMarkTracker(landmarkName, _defaultTrackSize, _defaultTrackingInterval));
        }
        _allTrackers.AddRange(_landMarkTrackers);
    }

    public void ConfigureLandMarkTracker()
    {
        
    }
    
    public LandMarkTracker GetLandMarkTracker(PoseTrackingInfo.LandmarkNames landmarkName)
    {
        return _landMarkTrackers.Find(e => e.GetTrackedLandMark() == landmarkName);
    }

    public async Task StartLandMarkTracker(PoseTrackingInfo.LandmarkNames landmarkName)
    {
        await LandMarkProvider.Instance.WaitForLandMarkData();
        
        StartTracker (_landMarkTrackers.Find(e => e.GetTrackedLandMark() == landmarkName));
    }

    public void StopLandMarkTracker(PoseTrackingInfo.LandmarkNames landmarkName)
    {
        StopTracker (_landMarkTrackers.Find(e => e.GetTrackedLandMark() == landmarkName));
    }

    private void StartTracker(Tracker tracker)
    {
        tracker.StartTracking();
    }

    private void StopTracker(Tracker tracker)
    {
        tracker.StopTracking();
    }
}
