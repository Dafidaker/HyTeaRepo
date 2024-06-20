using System;
using UnityEngine;

public struct ClipInfo
{
    public AudioClip AudioClip;
    public float Loudness;
}


public class AudioLoudnessDetection : MonoBehaviour
{
    public int sampleWindow = 2;
    public int microphoneIndex;
    
    public AudioClip microphoneClip;

    private ClipInfo _whisperingClip;
    private ClipInfo _yellingClip;

    private void Start()
    {
        microphoneIndex = 0;
        MicrophoneToAudioCLip();
    }

    private void OnEnable()
    {
        MicrophoneSelector.OnMicrophoneChanged += ChangedMicrophone;
    }

    private void OnDisable()
    {
        MicrophoneSelector.OnMicrophoneChanged -= ChangedMicrophone;
    }

    public void ChangedMicrophone(int _microphoneIndex)
    {
        microphoneIndex = _microphoneIndex;
    }

    public void MicrophoneToAudioCLip()
    {
        string microphoneName = Microphone.devices[microphoneIndex];
        microphoneClip = Microphone.Start(microphoneName, true, 20, AudioSettings.outputSampleRate);
    }
    
    public void MicrophoneToAudioCLip(ClipInfo clipInfo, int length = 5, bool loop = false)
    {
        string microphoneName = Microphone.devices[microphoneIndex];
        clipInfo.AudioClip = Microphone.Start(microphoneName, loop, length, AudioSettings.outputSampleRate);
    }
    
    public float GetLoudnessFromMicrophone()
    {
        return GetLoudnessFromAudioClip(Microphone.GetPosition(Microphone.devices[microphoneIndex]), microphoneClip);
    }

    private float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip)
    {
        var startPosition = clipPosition - sampleWindow;

        if (startPosition < 0 )
        {
            return 0;
        }
        
        var waveData = new float[sampleWindow];
        clip.GetData(waveData, startPosition);
        
        //compute loudness
        float totalLoudness = 0;

        for (var i = 0; i < sampleWindow; i++)
        {
            totalLoudness += Mathf.Abs(waveData[i]);
        }

        return totalLoudness / sampleWindow;
    }

    public ClipInfo GetWhisperingAudio(ClipInfo audio)
    {
        MicrophoneToAudioCLip(audio);
        audio.Loudness = GetLoudnessFromAudioClip(Microphone.GetPosition(Microphone.devices[microphoneIndex]), audio.AudioClip);
        return audio;
    }

    public ClipInfo GetLoudnessAudio(ClipInfo audio)
    {
        MicrophoneToAudioCLip(audio);
        audio.Loudness = GetLoudnessFromAudioClip(Microphone.GetPosition(Microphone.devices[microphoneIndex]), audio.AudioClip);
        return audio;
    }
    
    
}

