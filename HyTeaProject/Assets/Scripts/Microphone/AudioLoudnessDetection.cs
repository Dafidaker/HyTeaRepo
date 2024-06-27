using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public struct ClipInfo
{
    public AudioClip AudioClip;
    public float Loudness;
}

[RequireComponent(typeof(AudioSource))]
public class AudioLoudnessDetection : MonoBehaviour
{
    private int _sampleWindow;
    private int _microphoneIndex;
    private string _microphoneString;
    
    public AudioClip microphoneClip;

    [field:SerializeField]private List<AudioClip> _previousAudioClips;
    
    private Coroutine MicrophoneRecord;
    
    private AudioSource _audioSource;
    private ClipInfo _whisperingClip;
    private ClipInfo _yellingClip;

    private void Awake()
    {
        _sampleWindow = AudioSettings.outputSampleRate;
        
        _audioSource = GetComponent<AudioSource>();
        _audioSource.Play();
        
        _previousAudioClips = new List<AudioClip>();
    }

    private void Start()
    {
        StartMicrophoneCoroutine();
        
        ChangeMicrophoneIndex(0);
    }

    private void OnEnable()
    {
        EventManager.ChangedMicrophone.AddListener(ChangeMicrophoneIndex);
        EventManager.ClickedPlaybackButton.AddListener(PlaybackAudio);
    }

    private void OnDisable()
    {
        EventManager.ChangedMicrophone.RemoveListener(ChangeMicrophoneIndex);
        EventManager.ClickedPlaybackButton.RemoveListener(PlaybackAudio);
    }

    private void PlaybackLoopedAudio()
    {
        _audioSource.clip = Microphone.Start(_microphoneString, true, 10, AudioSettings.outputSampleRate);
        _audioSource.loop = true;

        // Wait until the microphone has started recording
        while (!(Microphone.GetPosition(_microphoneString) > 0)) { }
        _audioSource.Play();
    }

    private void UpdateMicrophone()
    {
        //Microphone.End(_microphoneString);

        StartMicrophoneCoroutine();
    }

    private void StartMicrophoneCoroutine()
    {
        if (MicrophoneRecord != null) StopCoroutine(MicrophoneRecord);
        MicrophoneRecord = StartCoroutine(MicrophoneToAudioCLipCoroutine());
    }
    
    private IEnumerator MicrophoneToAudioCLipCoroutine()
    {
        if (microphoneClip != null)
        {
            microphoneClip.name = Time.time.ToString();
            _previousAudioClips.Add(microphoneClip);
        }
        
        _audioSource.clip = Microphone.Start(_microphoneString, true, 10, AudioSettings.outputSampleRate);
        _audioSource.clip.name = Time.time.ToString();
        _audioSource.loop = true;

        // Wait until the microphone has started recording
        while (!(Microphone.GetPosition(_microphoneString) > 0)) { }
        _audioSource.Play();
        
        microphoneClip = _audioSource.clip;

        yield return new WaitForSeconds(3f);
        
        // Repeat the coroutine
        StartMicrophoneCoroutine();
    }

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
        return GetLoudnessFromAudioClip(Microphone.GetPosition(_microphoneString), microphoneClip);
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

        for (var i = 0; i < _sampleWindow; i++)
        {
            totalLoudness += Mathf.Abs(waveData[i]);
        }

        return totalLoudness / _sampleWindow;
    }

    private void ChangeMicrophoneIndex(int newIndex)
    {
        _microphoneIndex = newIndex;
        _microphoneString = Microphone.devices[_microphoneIndex];
        UpdateMicrophone();
    }

    private void PlaybackAudio(bool playback)
    {
        if (_audioSource == null) return;
        
        /*if (playback)
        {
            _audioSource.Play();
            UpdateMicrophone();
        } 
        else _audioSource.Play();*/
    }
}

