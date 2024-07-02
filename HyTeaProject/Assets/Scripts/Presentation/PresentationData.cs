using System;
using System.Collections.Generic;
using UnityEngine;

public enum GesturesEnum
{
    HandsInFace,
    CrossedArms,
    ArmsBelowWaist,
}

[Serializable]
public class Pauses
{
    public Pauses(float startTime, float duration)
    {
        StartTime = startTime;
        Duration = duration;
    }

    public float StartTime; 
    public float Duration; 
}

[Serializable]
public class TimedLoudness
{
    public TimedLoudness(float time, float loudness)
    {
        Time = time;
        Loudness = loudness;
    }

    public float Time; 
    public float Loudness; 
}

[Serializable]
public class TimedGestures
{
    /*public TimedGestures(float startTime,float duration, GesturesEnum gesturesEnum)
    {
        StartTime = startTime;
        Duration = duration;
        GesturesEnum = gesturesEnum;
    }*/

    [SerializeField] public float StartTime;
    [SerializeField] public float Duration;
    [SerializeField] public GesturesEnum GesturesEnum; 
}

[CreateAssetMenu(fileName = "PresentationData", menuName = "Presentation/PresentationData")]
[Serializable]
public class PresentationData : ScriptableObject
{
    public float StartTime;
    public float EndTime;
    public float Duration;
    public float DesiredDuration;

    public string presentationScript;
    
    [SerializeField] public List<TimedLoudness>  _timedLoudness;
    [SerializeField] public List<TimedGestures>  _timedGestures;
    [SerializeField] public List<Pauses> _timedPauses;
    
    /*PresentationData()
    {
//        StartTime = Time.time;
        
        
        _timedLoudness = new List<TimedLoudness>();
        _timedGestures = new List<TimedGestures>();
        _timedPauses = new List<Pauses>();
        //EventManager.LatestLoudnessCaptured.AddListener(StoreLoudness);
        //EventManager.OnPauseStateChanged.AddListener(CreateUpdatePause);
        //EventManager.GestureCaptured.AddListener(StoreGesture);
    }*/


    /*private void StoreLoudness(float loudness)
    {
        _timedLoudness.Add(new TimedLoudness(Time.time, loudness));
    }

    private void StoreGesture(float startTime, float duration, GesturesEnum gesture)
    {
        _timedGestures.Add(new TimedGestures(startTime,  duration, gesture));
    }

    private void CreateUpdatePause(bool pauseState)
    {
        if (pauseState)
        {
            _timedPauses.Add(new Pauses(Time.time - 0.5f, 0));
        }
        else if (_timedPauses.Count > 0 && _timedPauses[^1].Duration == 0)
        {
            _timedPauses[^1].Duration = Time.time - StartTime;
        }
    }*/
}
