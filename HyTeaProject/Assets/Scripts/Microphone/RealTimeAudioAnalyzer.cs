using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealTimeAudioAnalyzer : MonoBehaviour
{
    public string audioClipResourcePath; // Path to the audio clip in Resources folder
    public int sampleRate = 44100; // Sample rate of the audio clip (default is 44100 Hz)
    public int analysisDuration = 1; // Duration in seconds to analyze

    private AudioClip audioClip; // Loaded audio clip
    private float[] audioBuffer; // Circular buffer to store audio samples
    private int bufferSize; // Size of the circular buffer in samples
    private int bufferIndex = 0; // Index to track current position in the circular buffer

    void Start()
    {
        MicrophoneManager.Instance.RecordMicrophone();
        
        // Load audio clip from Resources
        audioClip = MicrophoneManager.Instance.microphoneClip;

        // Ensure audio clip and sample rate are valid
        if (audioClip != null)
        {
            // Initialize circular buffer
            bufferSize = sampleRate * analysisDuration; // Buffer size for 1 second of audio
            audioBuffer = new float[bufferSize];

            // Start reading audio samples into the buffer
            ReadInitialAudioSamples();

            // Example: Start real-time analysis coroutine
            StartCoroutine(RealTimeAnalysis());
        }
        else
        {
            Debug.LogError("Failed to load AudioClip from Resources folder.");
        }
    }

    IEnumerator RealTimeAnalysis()
    {
        while (true)
        {
            // Perform analysis on the buffered audio
            AnalyzeAudioBuffer();

            // Wait for next frame
            yield return null;
        }
    }

    void ReadInitialAudioSamples()
    {
        // Calculate the starting sample position (1 second behind the current time)
        int startSample = Mathf.Max(0, MicrophoneManager.Instance.microphoneClip.samples - sampleRate); // Start 1 second before the end
        startSample = Mathf.Max(0, startSample); // Ensure it doesn't go before the start of the clip

        // Read initial audio samples into the circular buffer
        int numSamplesToRead = Mathf.Min(MicrophoneManager.Instance.microphoneClip.samples - startSample, bufferSize);
        MicrophoneManager.Instance.microphoneClip.GetData(audioBuffer, startSample);

        // Update buffer index for circular wrapping
        bufferIndex = numSamplesToRead % bufferSize;
    }

    void AnalyzeAudioBuffer()
    {
        // Calculate RMS value of the audio buffer
        float sum = 0f;
        int numSamples = 0;

        // Iterate over the entire buffer
        for (int i = 0; i < bufferSize; i++)
        {
            float sample = audioBuffer[i] *= MicrophoneManager.Instance.GetGain();
            sample = Mathf.Clamp(sample, -1.0f, 1.0f); // Clamp to prevent clipping
            sum += sample * sample; // Sum of squared samples
            numSamples++;
        }

        // Calculate RMS value
        float rmsValue = Mathf.Sqrt(sum / numSamples);
        Debug.Log("RMS Value of previous " + analysisDuration + " seconds: " + rmsValue);
    }
}

