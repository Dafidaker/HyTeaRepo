using System;
using System.Collections;
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
    private float Duration;

    public void SetDuration(float duration)
    {
        Duration = duration;
    }
}

[Serializable]
public class TimedGestures
{
    public TimedGestures(float startTime,float duration, string gesturesName)
    {
        StartTime = startTime;
        Duration = duration;
        GesturesName = gesturesName;
    }

    [SerializeField] public float StartTime;
    [SerializeField] public float Duration;
    //[SerializeField] public GesturesEnum GesturesEnum; 
    [SerializeField] public string GesturesName; 
}

[Serializable]
public class AudioClipInfo
{
    public AudioClipInfo(AudioClip _clip, float _startTime)
    {
        clip = _clip;
        startTime = _startTime;
        endTime = startTime + clip.length;
        transcription = "";
    }

    [SerializeField] public AudioClip clip;
    [SerializeField] public float startTime;
    [SerializeField] public float endTime;
    [SerializeField] public string transcription; 
}


[Serializable]
public class PresentationData : MonoBehaviour
{
    public float StartTime;
    public float EndTime;
    public float Duration;
    public float DesiredDuration;

    private bool _presentationOccuring;

    public string presentationScript;
    private Coroutine _trackingCoroutine;
    
    [SerializeField] public List<TimedLoudness>  _timedLoudness = new();
    [SerializeField] public List<TimedGestures>  _timedGestures = new();
    [SerializeField] public List<Pauses> _timedPauses = new();
    [SerializeField] public List<AudioClipInfo> _AudioClips = new();
    [SerializeField] public List<AudioClipInfo> _sanatizedAudioClips = new();
    
    private float SumOfAllPausesDuration;

    private void Awake()
    {
        //StartTime = Time.time;
    }

    ~PresentationData()
    {
        EndPresentation();
    }
    
    public void StartPresentation()
    {
        StartTime = Time.time;
        _trackingCoroutine ??= StartCoroutine(TrackPresentationTime());
        _presentationOccuring = true;
        EventManager.LatestLoudnessCaptured.AddListener(StoreLoudness);
        EventManager.OnPauseDone.AddListener(AddPause);
        MovementAnalysis.OnGestureDetected += MovementAnalysisOnOnGestureDetected;
        EventManager.GestureCaptured.AddListener(StoreGesture);
        EventManager.NewAudioClipFinished.AddListener(SaveAudioClip);
    }

    public void PausePresentation()
    {
        _presentationOccuring = false;
        EventManager.LatestLoudnessCaptured.RemoveListener(StoreLoudness);
        MovementAnalysis.OnGestureDetected += MovementAnalysisOnOnGestureDetected;
        EventManager.OnPauseDone.RemoveListener(AddPause);
        EventManager.GestureCaptured.RemoveListener(StoreGesture);
        EventManager.NewAudioClipFinished.RemoveListener(SaveAudioClip);
    }
    
    public void EndPresentation()
    {
        _presentationOccuring = false;
        if (_trackingCoroutine != null)  StopCoroutine(_trackingCoroutine); 
        EndTime = Time.time;
        
        EventManager.LatestLoudnessCaptured.RemoveListener(StoreLoudness);
        MovementAnalysis.OnGestureDetected -= MovementAnalysisOnOnGestureDetected;
        EventManager.OnPauseDone.RemoveListener(AddPause);
        EventManager.GestureCaptured.RemoveListener(StoreGesture);
        EventManager.NewAudioClipFinished.RemoveListener(SaveAudioClip);
    }

    private void MovementAnalysisOnOnGestureDetected(GestureHolder obj)
    {
        _timedGestures.Add(new TimedGestures(Time.time,  obj.Duration, obj.Name));
    }

    private IEnumerator TrackPresentationTime()
    {
        while (true)
        {
            if (_presentationOccuring)
            {
                Duration += Time.deltaTime;
            }
            yield return null;
        }
    }

    private void StoreLoudness(float loudness)
    {
        _timedLoudness.Add(new TimedLoudness(Time.time, loudness));
    }

    private void StoreGesture(float startTime, float duration, GesturesEnum gesture)
    {
        //_timedGestures.Add(new TimedGestures(startTime,  duration, gesture));
    }

    private void AddPause(float duration)
    {
        _timedPauses.Add(new Pauses(Time.time - duration, duration));
        
        /*if (pauseState)
        {
            _timedPauses.Add(new Pauses(Time.time - 0.5f, 0));
        }
        else if (_timedPauses.Count > 0 && _timedPauses[^1].Duration == 0)
        {
            _timedPauses[^1].Duration = Time.time - StartTime;
        }*/
    }
    
    private void GetSumOfPausesDuration()
    {
        SumOfAllPausesDuration = 0.0f;
        
        foreach (var pause in _timedPauses)
        {
            SumOfAllPausesDuration += pause.Duration;
        }
      
    }

    
    private void SaveAudioClip(AudioClip clip)
    {
        _AudioClips.Add(new AudioClipInfo(clip, Time.time) );
    }

    public IEnumerator SanitizeAudioClips()
    {
        foreach (var audioClipInfo in _AudioClips)
        {
            yield return StartCoroutine(SpeechRecognitionUtill.Instance.SendPreRecordedRecording(audioClipInfo, _sanatizedAudioClips));
        }
        
    }
}
