using System.Collections.Generic;
using UnityEngine;

public enum GesturesEnum
{
    
}

class Pauses
{
    public Pauses(float startTime, float duration)
    {
        StartTime = startTime;
        Duration = duration;
    }
    
    public float StartTime { get; private set ; }
    public float Duration { get;  set; }
}

class TimedLoudness
{
    public TimedLoudness(float time, float loudness)
    {
        Time = time;
        Loudness = loudness;
    }
    
    public float Time { get; private set; }
    public float Loudness { get; private set; }
}

class TimedGestures
{
    public TimedGestures(float startTime,float duration, GesturesEnum gesturesEnum)
    {
        StartTime = startTime;
        Duration = duration;
        GesturesEnum = gesturesEnum;
    }
    
    public float StartTime { get; private set; }
    public float Duration { get; private set; }
    public GesturesEnum GesturesEnum { get; private set; }
}

public class PresentationData : Object
{
    private float StartTime;
    private float EndTime;
    private float Duration;
    
    
    private List<TimedLoudness>  _timedLoudness;
    private List<TimedGestures>  _timedGestures;
    private List<Pauses> _timedPauses;
    
    PresentationData()
    {
        StartTime = Time.time;
        
        
        _timedLoudness = new List<TimedLoudness>();
        _timedGestures = new List<TimedGestures>();
        _timedPauses = new List<Pauses>();
        EventManager.LatestLoudnessCaptured.AddListener(StoreLoudness);
        EventManager.OnPauseStateChanged.AddListener(CreateUpdatePause);
        EventManager.GestureCaptured.AddListener(StoreGesture);
    }


    private void StoreLoudness(float loudness)
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
    }
}
