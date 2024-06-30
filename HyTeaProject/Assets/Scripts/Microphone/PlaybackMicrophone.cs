using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackMicrophone : MonoBehaviour
{ 
    private AudioSource _playbackAudioSource;
    private AudioClip _previousAudioClip;
    
    private bool _isMicrophonePlaybackEnabled;
    
    private float _startTime;
    private float _gain;
    
    private void Awake()
    {
        _playbackAudioSource = gameObject.AddComponent<AudioSource>();

        _startTime = 0f;
    }

    private void OnEnable()
    {
        EventManager.ChangedGain.AddListener(SetGain);
        EventManager.StartedNewRecording.AddListener(UpdatePlayback);
        EventManager.ClickedPlaybackButton.AddListener(PlaybackAudio);
    }

    private void OnDisable()
    {
        EventManager.ChangedGain.RemoveListener(SetGain);
        EventManager.StartedNewRecording.RemoveListener(UpdatePlayback);
        EventManager.ClickedPlaybackButton.RemoveListener(PlaybackAudio);
    }
    
    void OnAudioFilterRead(float[] data, int channels)
    {
        // Increase the gain of the audio samples
        for (int i = 0; i < data.Length; i++)
        {
            data[i] *= _gain;
            data[i] = Mathf.Clamp(data[i], -1.0f, 1.0f); // Clamp to prevent clipping
        }
    }

    private void SetGain(float newGain)
    {
        _gain = newGain;
    }

    private void PlaybackMicrophoneRecording()
    {
        if (!_playbackAudioSource) return;
        
        //if the microphone is not recording or if the microphone shouldn't playback return
        if (!MicrophoneManager.Instance.IsMicrophoneRecording() || !_isMicrophonePlaybackEnabled)
        {
            _playbackAudioSource.Stop();
            return;
        }

        UpdateAudioClipStartTime();
        
        _playbackAudioSource.clip = MicrophoneManager.Instance.microphoneClip;
        _playbackAudioSource.loop = true;

        // Wait until the microphone has started recording
        while (!(Microphone.GetPosition(MicrophoneManager.Instance.GetSelectedMicrophoneString()) > 0)) { }
        _playbackAudioSource.Play(); 
    }
    
    private void StopPlayback()
    {
        _startTime = Time.time - _playbackAudioSource.time;
        _previousAudioClip = _playbackAudioSource.clip;
        if (_playbackAudioSource) _playbackAudioSource.Stop(); 
    }

    private void UpdateAudioClipStartTime()
    {
        if (MicrophoneManager.Instance.microphoneClip != _previousAudioClip) return;
        
        var elapsedTime = Time.time - _startTime;
        _playbackAudioSource.time = elapsedTime % _playbackAudioSource.clip.length;
    }
    
    private void PlaybackAudio(bool playback)
    {
        //sets the bool if the audio is being played or not 
        _isMicrophonePlaybackEnabled = playback;
        
        //play audio if true or dont if not true
        UpdatePlayback();
    }

    private void UpdatePlayback()
    {
        if (_isMicrophonePlaybackEnabled)
        {
            PlaybackMicrophoneRecording();
        }
        else StopPlayback();
    }
}
