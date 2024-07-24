using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class GestureDone
{
    public float Time;
    public string Gesture;
}


public class CheckForPositions : MonoBehaviour
{
    [SerializeField] private float _handToNoseMaxDistance;

    private Dictionary<string, List<float>> gesturesTracker;
    
    private void OnEnable()
    {
        MovementAnalysis.OnGestureDetected += HandleNewGesture;
    }

    private void OnDisable()
    {
        MovementAnalysis.OnGestureDetected -= HandleNewGesture;
    }

    private void Update()
    {
        if (PoseAnalysis.AnalyzeForCrossedArms(LandMarkProvider.Instance.VectorLandmarkList, true))
        {
            Debug.Log("Crossed Arms");
        }
            
        (float leftHandNoseDistance, float rightHandNoseDistance) = PoseAnalysis.GetHandsToNoseDistance( LandMarkProvider.Instance.VectorLandmarkList);
        
        bool tooCloseTooFace = leftHandNoseDistance <= _handToNoseMaxDistance || rightHandNoseDistance <= _handToNoseMaxDistance;
        if (tooCloseTooFace)
        {
            Debug.Log("touching face");
        }
    }
    
    private void HandleNewGesture(GestureHolder gestureHolder)
    {
        AddGestureValue(gestureHolder.Name);
    }

    private void AddGestureValue(string key)
    {
        if (gesturesTracker.ContainsKey(key))
        {
            gesturesTracker[key].Add(Time.time);
        }
        else
        {
            gesturesTracker[key] = new List<float> { Time.time };
        }
    }
    
}
