using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneManager : Singleton<MicrophoneManager>
{
    private int _microphoneIndex;
    private string _microphoneString;
    
    public AudioClip microphoneClip;
    private List<AudioClip> _previousAudioClips;

    private float Gain;

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
        _previousAudioClips = new List<AudioClip>();
        
        //it selects the first microphone initially
        ChangeSelectedMicrophone(0);
        
        RecordMicrophone();
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
        StopRecordingMicrophone();
        
        microphoneClip = Microphone.Start(_microphoneString, true, 10, AudioSettings.outputSampleRate);
        
        if (microphoneClip != null) EventManager.StartedNewRecording.Invoke(); 
    }
    
    private void StopRecordingMicrophone()
    {
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
        var wasRecording = IsMicrophoneRecording();
        
        StopRecordingMicrophone();

        UpdateMicrophoneVariables(newIndex);

        if (wasRecording) RecordMicrophone();
    }
    
}
