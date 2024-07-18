using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneManager : Singleton<MicrophoneManager>
{
    private int _microphoneIndex;
    private string _microphoneString;
    private float Gain = 1;
    
    private bool _isRecording;
    
    public AudioClip microphoneClip;
    public List<AudioClip> _previousAudioClips;

    private string[] _previousMicrophoneDevices;

    private Coroutine _trackingCoroutine;

    private AudioLoudnessDetection _audioLoudnessDetection;
    
    public VolumeAnalyzer _volumeAnalyzer;
    
    
    private void OnEnable()
    {
        EventManager.DifferentMicrophoneSelectedInUI.AddListener(ChangeSelectedMicrophone);
    }

    private void OnDisable()
    {
        EventManager.DifferentMicrophoneSelectedInUI.RemoveListener(ChangeSelectedMicrophone);
    }

    protected override void Awake()
    {
        base.Awake();
        _isRecording = false;
        _previousAudioClips = new List<AudioClip>();
        
        _audioLoudnessDetection = gameObject.AddComponent<AudioLoudnessDetection>();
        
        _volumeAnalyzer = _volumeAnalyzer = new VolumeAnalyzer(0.1f,0.5f,0,1);
        //_previousMicrophoneDevices = Microphone.devices;
        
        // Select the first microphone initially
        ChangeSelectedMicrophone(0);
        
        InicializeMicrophoneData();
        
        // Start coroutine to check for microphone changes
        StartCoroutine(CheckForMicrophoneChanges());
        
    }


    public void SetGain(float newGain)
    {
        Gain = newGain;
        EventManager.ChangedGain.Invoke(newGain);
    }

    public float GetGain()
    {
        return Gain;
    }

    public List<AudioClip> GetAllAudioClips()
    {
        return _previousAudioClips;
    }

    public AudioLoudnessDetection GetAudioDetection()
    {
        return _audioLoudnessDetection;
    }

    private void AddGainToMicrophone()
    {
        if (IsMicrophoneRecording())
        {
            // Get the audio data from the clip
            float[] samples = new float[microphoneClip.samples * microphoneClip.channels];
            microphoneClip.GetData(samples, 0);

            // Increase the gain of the audio samples
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] *= Gain;
                samples[i] = Mathf.Clamp(samples[i], -1.0f, 1.0f); // Clamp to prevent clipping
            }

            // Set the modified data back to the clip
            microphoneClip.SetData(samples, 0);
        }
    }

    public void RecordMicrophone()
    {
        _trackingCoroutine = StartCoroutine(RecordMicrohponeCoroutine());
    }

    private IEnumerator RecordMicrohponeCoroutine()
    {
        int clipLength = 20;
        
        StopRecordingTheMicrophone();
        AddToPreviousAudioClips();
            
        microphoneClip = Microphone.Start(_microphoneString, true, clipLength, AudioSettings.outputSampleRate);
        microphoneClip.name = Time.time + " by " + _microphoneString;

        if (microphoneClip != null)
        {
            _isRecording = true;
            EventManager.StartedNewRecording.Invoke();
        }

        yield return new WaitForSeconds(clipLength);

        RecordMicrophone();
    }

    private void StopMicrophoneCoroutine()
    {
        if (_trackingCoroutine != null)
        {
            StopCoroutine(_trackingCoroutine);
        }
    }
    
    private void StopRecordingTheMicrophone()
    {
        Microphone.End(_microphoneString);
    }
    
    public void StopRecording()
    {
        _isRecording = false;
        Microphone.End(_microphoneString);
        StopMicrophoneCoroutine();

        AddToPreviousAudioClips();
        EventManager.StoppedRecording.Invoke();
    }

    private void AddToPreviousAudioClips()
    {
        if (microphoneClip == null) return;
        
        if (_previousAudioClips != null && !_previousAudioClips.Contains(microphoneClip))
        {
            _previousAudioClips?.Add(microphoneClip);
            EventManager.NewAudioClipFinished.Invoke(microphoneClip);
        }
    }

    public bool IsMicrophoneRecording()
    {
         return Microphone.IsRecording(_microphoneString);
    }

    public string GetSelectedMicrophoneString()
    {
        return _microphoneString;
    }

    private void UpdateMicrophoneVariables(int newIndex)
    {
        _microphoneIndex = newIndex;
        _microphoneString = Microphone.devices[_microphoneIndex];
    }
    
    private void ChangeSelectedMicrophone(int newIndex)
    {
        StopRecordingTheMicrophone();

        UpdateMicrophoneVariables(newIndex);

        if (_isRecording) RecordMicrophone();
    }

    private void InicializeMicrophoneData()
    {
        _previousMicrophoneDevices = Microphone.devices;
    }
    
    private IEnumerator CheckForMicrophoneChanges()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            string[] currentMicrophoneDevices = Microphone.devices;

            // Check if the microphone list has changed
            if (HasMicrophoneListChanged(currentMicrophoneDevices))
            {
                HandleMicrophoneDisconnection(currentMicrophoneDevices);
                _previousMicrophoneDevices = currentMicrophoneDevices;
            }
        }
    }

    private bool HasMicrophoneListChanged(string[] currentDevices)
    {
        if (currentDevices.Length != _previousMicrophoneDevices.Length)
        {
            return true;
        }

        for (int i = 0; i < currentDevices.Length; i++)
        {
            if (currentDevices[i] != _previousMicrophoneDevices[i])
            {
                return true;
            }
        }

        return false;
    }

    private void HandleMicrophoneDisconnection(string[] currentMicrophoneDevices)
    {
        int indexOfSelectedMicrophone = Array.IndexOf(currentMicrophoneDevices, _microphoneString);
        // Check if the currently selected microphone is still available
        if (indexOfSelectedMicrophone == -1)
        {
            // If the current microphone is not available, select a new one
            if (currentMicrophoneDevices.Length > 0)
            {
                ChangeSelectedMicrophone(0);
            }
            else
            {
                // No microphones are available
                StopRecordingTheMicrophone();
                _microphoneString = null;
            }
        }
        else
        {
            ChangeSelectedMicrophone(indexOfSelectedMicrophone);
        }
    }
    
}
