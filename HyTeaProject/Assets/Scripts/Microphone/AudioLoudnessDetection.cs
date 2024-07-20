using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioLoudnessDetection : MonoBehaviour
{
    private int _sampleWindow;
    
    [field:SerializeField] private float gain;
    [field:SerializeField] private float threshold = 0.001f;
    [field:SerializeField] private TextMeshProUGUI loudnessText;
    [field:SerializeField] private float _loudnessSensitivity = 150f;

    public float currentLoudness;
    private bool _pauseHappening = false;

    private Coroutine _loudnessTractCoroutine;
    private Coroutine _pauseTractCoroutine;
    
    private void Awake()
    {
        _sampleWindow = 1024;

        currentLoudness = 0;
    }
    
    private void OnEnable()
    {
        gain = MicrophoneManager.Instance.GetGain();
        EventManager.ChangedGain.AddListener(SetGain);
        EventManager.StartedNewRecording.AddListener(ResetDetectingLoudness);
        EventManager.StoppedRecording.AddListener(HandleStopRecording);
        EventManager.ChangedLoudnessMultiplier.AddListener(SetSensitivity);
    }
    
    private void OnDisable()
    {
        EventManager.ChangedGain.RemoveListener(SetGain);
        EventManager.StartedNewRecording.RemoveListener(ResetDetectingLoudness);
        EventManager.StoppedRecording.RemoveListener(HandleStopRecording);
        EventManager.ChangedLoudnessMultiplier.RemoveListener(SetSensitivity);
    }
    
    private void SetSensitivity(float sensitivity)
    {
        _loudnessSensitivity = sensitivity;
    }
    
    private void SetGain(float newGain)
    {
        gain = newGain;
    }
    
    private void HandleStopRecording()
    {
        StartCoroutine(StopDetectingLoudness());
    }
    
    private void ResetDetectingLoudness()
    {
        StopDetectingLoudness();

        StartDetectingLoudness();
    }
    
    public void StartDetectingLoudness()
    {
        _loudnessTractCoroutine ??= StartCoroutine(UpdateLoudness());
    }

    public IEnumerator StopDetectingLoudness()
    {
        if (_loudnessTractCoroutine != null)
        {
            StopCoroutine(_loudnessTractCoroutine);
            _loudnessTractCoroutine = null;
        }
        _pauseHappening = false;

        while (_pauseTractCoroutine != null)
        {
            yield return null;
        }
    }
    
    private float GetLoudnessFromMicrophone()
    {
        return  GetLoudnessFromAudioClip(Microphone.GetPosition(MicrophoneManager.Instance.GetSelectedMicrophoneString()), MicrophoneManager.Instance.microphoneClip);
    }

    private IEnumerator UpdateLoudness()
    {
        yield return new WaitForSeconds(0.1f);

        currentLoudness = GetLoudnessFromMicrophone() * _loudnessSensitivity;

        EventManager.LatestLoudnessCaptured.Invoke(currentLoudness);
        
        if (currentLoudness == 0 && !_pauseHappening)
        {
            _pauseHappening = true;
            _pauseTractCoroutine = StartCoroutine(CheckForPause());
        }
        else if (_pauseHappening && currentLoudness != 0)
        {
            _pauseHappening = false;
        }

        _loudnessTractCoroutine = StartCoroutine(UpdateLoudness());
    }

    private IEnumerator CheckForPause()
    {
        float duration = 0; 

        while (_pauseHappening)
        {
            //yield return null;

            duration += Time.deltaTime;

            if (!(duration > 1f)) continue;
            
            StartCoroutine(WaitForPause(0.5f));
            yield break;
        }
    }
    
    private IEnumerator WaitForPause(float elapsedTime)
    {
        float duration = elapsedTime; 
        
        EventManager.PauseStarted.Invoke();
        
        while (_pauseHappening)
        {
            yield return null;

            duration += Time.deltaTime;
        }

        _pauseTractCoroutine = null;
        EventManager.OnPauseDone.Invoke(duration);
    }

    private float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip)
    {
        var startPosition = clipPosition - _sampleWindow;

        if (startPosition < 0 )
        {
            return 0;
        }
        
        var waveData = new float[_sampleWindow];
        clip.GetData(waveData, startPosition);
        
        //compute loudness
        float totalLoudness = 0;
        int usableSamples = 0; 

        for (var i = 0; i < _sampleWindow; i++)
        {
            var valueToAdd = AddingGainToAudio(waveData[i]);;
            
            if (!(valueToAdd >= threshold)) continue;
            
            valueToAdd = Mathf.Clamp(valueToAdd, -1.0f, 1.0f);
            
            totalLoudness += valueToAdd;
            usableSamples++;
        }

        if (usableSamples == 0) return 0; 
        
        return totalLoudness / usableSamples;
        
    }

    private float AddingGainToAudio(float data)
    {
        return Mathf.Abs(data * gain);
    }
}

