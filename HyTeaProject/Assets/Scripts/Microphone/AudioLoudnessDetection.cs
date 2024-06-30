using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public struct ClipInfo
{
    public AudioClip AudioClip;
    public float Loudness;
}

//[RequireComponent(typeof(AudioSource))]
public class AudioLoudnessDetection : MonoBehaviour
{
    private int _sampleWindow;
    
    private Coroutine MicrophoneRecord;
    
    [field:SerializeField] private float Gain;
    [field:SerializeField] private float threashold = 0.001f;
    [field:SerializeField] private TextMeshProUGUI loudnessText;

    public float currentLoudness;
    private bool _pauseHappening;
    
    private void Awake()
    {
        _sampleWindow = 1024;

        currentLoudness = 0;
    }

    private void Start()
    {
        StartCoroutine(UpdateLoudness());
    }

    private void OnEnable()
    {
        EventManager.ChangedGain.AddListener(SetGain);
    }

    private void OnDisable()
    {
        EventManager.ChangedGain.RemoveListener(SetGain);
    }

    private void SetGain(float newGain)
    {
        Gain = newGain;
    }
    
    /*private void StartMicrophoneCoroutine()
    {
        if (MicrophoneRecord != null) StopCoroutine(MicrophoneRecord);
        MicrophoneRecord = StartCoroutine(MicrophoneToAudioCLipCoroutine());
    }*/
    
    /*private IEnumerator MicrophoneToAudioCLipCoroutine()
    {
        //saves the audio clip
        if (microphoneClip != null)
        {
            microphoneClip.name = Time.time.ToString();
            _previousAudioClips.Add(microphoneClip);
        }
        
        //creates the audio clip
        _audioSource.clip = Microphone.Start(_microphoneString, true, 10, AudioSettings.outputSampleRate);
        _audioSource.clip.name = Time.time.ToString();
        _audioSource.loop = true;

        // Wait until the microphone has started recording
        while (!(Microphone.GetPosition(_microphoneString) > 0)) { }
        _audioSource.Play();
        
        microphoneClip = _audioSource.clip;

        yield return new WaitForSeconds(10f);
        
        // Repeat the coroutine
        StartMicrophoneCoroutine();
    }*/

    /*private void MicrophoneToAudioCLip()
    {
        _audioSource.clip = Microphone.Start(_microphoneString, true, 20, AudioSettings.outputSampleRate);
        microphoneClip = _audioSource.clip;
        //_audioSource.Play();
    }
    
    public void MicrophoneToAudioCLip(ClipInfo clipInfo, int length = 5, bool loop = false)
    {
        clipInfo.AudioClip = Microphone.Start(_microphoneString, loop, length, AudioSettings.outputSampleRate);
    }*/
    
    public float GetLoudnessFromMicrophone()
    {
        return GetLoudnessFromAudioClip(Microphone.GetPosition(MicrophoneManager.Instance.GetSelectedMicrophoneString()), MicrophoneManager.Instance.microphoneClip);
    }

    private IEnumerator UpdateLoudness()
    {
        yield return new WaitForSeconds(0.01f);

        currentLoudness = GetLoudnessFromMicrophone();
        
        if (currentLoudness == 0)
        {
            _pauseHappening = true;
            StartCoroutine(CheckForPause());
        }
        else
        {
            _pauseHappening = false;
        }

        StartCoroutine(UpdateLoudness());
    }

    private IEnumerator CheckForPause()
    {
        float duration = 0; 

        while (_pauseHappening)
        {
            yield return null;

            duration += Time.deltaTime;

            if (!(duration > 0.5f)) continue;
            
            StartCoroutine(WaitForPause(0.5f));
            yield break;
        }
    }
    
    private IEnumerator WaitForPause(float elapsedTime)
    {
        float duration = elapsedTime; 
        
        EventManager.OnPauseStateChanged.Invoke(_pauseHappening);
        
        while (_pauseHappening)
        {
            yield return null;

            duration += Time.deltaTime;
        }
        
        EventManager.OnPauseStateChanged.Invoke(_pauseHappening);
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
            var valueToAdd = Mathf.Abs(AddingGainToAudio(waveData[i]));
            
            if (!(valueToAdd >= threashold)) continue;
            
            totalLoudness += valueToAdd;
            usableSamples++;
        }

        if (usableSamples == 0) return 0; 
        
        return totalLoudness / usableSamples;
        
    }

    private float AddingGainToAudio(float data)
    {
        return Mathf.Clamp(data * Gain, -1.0f, 1.0f);
    }
}

