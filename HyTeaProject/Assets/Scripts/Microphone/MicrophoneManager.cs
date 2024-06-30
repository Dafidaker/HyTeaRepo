using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneManager : Singleton<MicrophoneManager>
{
    private int _microphoneIndex;
    private string _microphoneString;
    private bool _isRecording;
    
    public AudioClip microphoneClip;
    private List<AudioClip> _previousAudioClips;

    private float Gain;
    private string[] _previousMicrophoneDevices;

    private void OnEnable()
    {
        EventManager.ChangedMicrophone.AddListener(ChangeSelectedMicrophone);
    }

    private void OnDisable()
    {
        EventManager.ChangedMicrophone.RemoveListener(ChangeSelectedMicrophone);
    }

    
    private void Awake()
    {
        _isRecording = false;
        _previousAudioClips = new List<AudioClip>();
        _previousMicrophoneDevices = Microphone.devices;
        
        // Select the first microphone initially
        ChangeSelectedMicrophone(0);
        
        RecordMicrophone();

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
        StopRecordingTheMicrophone();
        
        microphoneClip = Microphone.Start(_microphoneString, true, 10, AudioSettings.outputSampleRate);
        microphoneClip.name = Time.time + " by " + _microphoneString;

        if (microphoneClip != null)
        {
            _isRecording = true;
            EventManager.StartedNewRecording.Invoke();
        } 
    }
    
    private void StopRecordingTheMicrophone()
    {
        //_isRecording = false;
        Microphone.End(_microphoneString);
    }
    
    private void StopRecording()
    {
        _isRecording = false;
        Microphone.End(_microphoneString);
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
        //var wasRecording = IsMicrophoneRecording();
        
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
