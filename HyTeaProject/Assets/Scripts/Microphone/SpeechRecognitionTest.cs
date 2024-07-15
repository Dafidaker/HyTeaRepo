using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using HuggingFace.API;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeechRecognitionUtill : Singleton<SpeechRecognitionUtill>
{
    private string errorResponse = "!error!";
    private string currentResponse = "";
    private bool _receivedResponse;
    [TextArea] public string resultString = "";
    
    
    public IEnumerator SendPreRecordedRecording(AudioClipInfo audioClipInfo, List<AudioClipInfo> sanitizedAudio)
    {
        if (audioClipInfo == null) yield break;
        
        var tries = 0;
        
        //var audioClips = new List<AudioClipInfo> { audioClipInfo };
        
        sanitizedAudio ??= new List<AudioClipInfo>();
        
        if (audioClipInfo.clip.length > 10f)
        {
            //audioClips.Clear();
            //audioClips.AddRange(SplitAudioClip(audioClipInfo, 10));
            
            sanitizedAudio.AddRange(SplitAudioClip(audioClipInfo, 10f));
        }
        else
        {
            sanitizedAudio.Add(audioClipInfo);
        }

        while (true)
        {
            foreach (var currentClipInfo in sanitizedAudio)
            {
                var speechStr = "";
                
                AudioClip currentClip = currentClipInfo.clip;
                
                // Extract data from the provided AudioClip
                float[] samples = new float[currentClip.samples * currentClip.channels];
                currentClip.GetData(samples, 0);

                // Encode the samples as WAV
                byte[] curBytes = EncodeAsWAV(samples, currentClip.frequency, currentClip.channels);

                // Optionally save the file locally for debugging
                //File.WriteAllBytes(Application.dataPath + "/test.wav", curBytes);

                string response = errorResponse;

                while (response == errorResponse && tries < 3)
                {
                    SendRecording(curBytes);

                    yield return new WaitWhile(() => !_receivedResponse );
                    response = currentResponse;
                    tries++;
                }

                if (response != errorResponse)
                {
                    //speechStr += " ";
                    speechStr += response;
                }

                currentClipInfo.transcription = speechStr;

                tries = 0;
                
            }
            
            //resultString = speechStr;
            yield break;
        }
        
    }
    
    private AudioClipInfo[] SplitAudioClip(AudioClipInfo clipInfo, float segmentLength)
    {
        float currentTime = clipInfo.startTime;

        AudioClip clip = clipInfo.clip;
        
        int totalSamples = clip.samples;
        int channels = clip.channels;
        int frequency = clip.frequency;
        float[] data = new float[totalSamples * channels];
        clip.GetData(data, 0);

        int segmentSamples = (int)(segmentLength * frequency) * channels;
        int segmentCount = Mathf.CeilToInt((float)totalSamples / segmentSamples);

        AudioClipInfo[] segments = new AudioClipInfo[segmentCount];

        for (int i = 0; i < segmentCount; i++)
        {
            int sampleStartIndex = i * segmentSamples;
            int sampleEndIndex = Mathf.Min((i + 1) * segmentSamples, totalSamples);

            float[] segmentData = new float[sampleEndIndex - sampleStartIndex];
            System.Array.Copy(data, sampleStartIndex, segmentData, 0, segmentData.Length);

            AudioClip segmentClip = AudioClip.Create(clip.name + "_segment_" + (i + 1), segmentData.Length / channels, channels, frequency, false);
            segmentClip.SetData(segmentData, 0);
            
            segments[i] = new AudioClipInfo(segmentClip,currentTime);
            currentTime += segmentClip.length;
        }

        return segments;
    }

    private void SendRecording(byte[] _bytes)
    {
        _receivedResponse = false;
        
        HuggingFaceAPI.AutomaticSpeechRecognition(_bytes, 
            response => 
        {

            currentResponse = response;
            _receivedResponse = true;
        }, 
            error => 
        {
            
            Debug.Log("error: " + error);
            
            currentResponse = "!error";
            
            if (error.Contains(@"""estimated_time"":"))
            {
                StartCoroutine(SetReceivedResponse(true, 25));
                
                Debug.Log("amount of time to wait is " + ExtractEstimatedTime(error).ToString());
            }
            else
            {
                _receivedResponse = true;
            }
        });
    }
    
    private int ExtractEstimatedTime(string response)
    {
        // Define the regular expression pattern to match the estimated_time value
        string pattern = "\"estimated_time\":(\\d+)";
        
        // Create a Regex object with the pattern
        Regex regex = new Regex(pattern);
        
        // Match the pattern in the response string
        Match match = regex.Match(response);

        // If a match is found, extract the value
        if (match.Success)
        {
            // Convert the matched value to an integer and return it
            return int.Parse(match.Groups[1].Value);
        }
        else
        {
            // If no match is found, return a default value or handle the case accordingly
            Debug.Log("No estimated_time found in the response.");
            return 0; // Default value if no match is found
        }
    }

    private IEnumerator SetReceivedResponse(bool value, float delay)
    {
        yield return new WaitForSeconds(delay);
        _receivedResponse = value;
    }
    
    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels) {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2)) {
            using (var writer = new BinaryWriter(memoryStream)) {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples) {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }
}


public class SpeechRecognitionTest : MonoBehaviour 
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private TextMeshProUGUI text;

    public AudioClip _clip;
    private byte[] _bytes;
    private bool _recording;
    
    private int _microphoneIndex;
    private bool _receivedResponse;
    private string errorResponse = "!error!";
    private string currentResponse = "";

    [TextArea] public string resultString = "";
    

    private void Start() 
    {
        _microphoneIndex = 0;
        startButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);
        stopButton.interactable = false;
         
        StartCoroutine(SendPreRecordedRecording(_clip));
    }

    private void Update() {
        if (_recording && Microphone.GetPosition(Microphone.devices[_microphoneIndex]) >= _clip.samples) {
            StopRecording();
        }
    }
    
    private void OnEnable()
    {
        EventManager.DifferentMicrophoneSelectedInUI.AddListener(ChangedMicrophone);
    }

    private void OnDisable()
    {
        EventManager.DifferentMicrophoneSelectedInUI.RemoveListener(ChangedMicrophone);
    }
    
    public void ChangedMicrophone(int microphoneIndex)
    {
        _microphoneIndex = microphoneIndex;
    }

    private void StartRecording() 
    {
        string microphoneName = Microphone.devices[_microphoneIndex];
        
        text.color = Color.white;
        text.text = "Recording...";
        startButton.interactable = false;
        stopButton.interactable = true;
        _clip = Microphone.Start(microphoneName, false, 10, 44100);
        _recording = true;
    }

    private void StopRecording()
    {
        string microphoneName = Microphone.devices[_microphoneIndex];
        
        var position = Microphone.GetPosition(microphoneName);
        Microphone.End(microphoneName);
        var samples = new float[position * _clip.channels];
        _clip.GetData(samples, 0);
        _bytes = EncodeAsWAV(samples, _clip.frequency, _clip.channels);
        _recording = false;
        File.WriteAllBytes(Application.dataPath + "/test.wav", _bytes);
        SendRecording();
    }

    private AudioClip[] SplitAudioClip(AudioClip clip, float segmentLength)
    {
        int totalSamples = clip.samples;
        int channels = clip.channels;
        int frequency = clip.frequency;
        float[] data = new float[totalSamples * channels];
        clip.GetData(data, 0);

        int segmentSamples = (int)(segmentLength * frequency) * channels;
        int segmentCount = Mathf.CeilToInt((float)totalSamples / segmentSamples);

        AudioClip[] segments = new AudioClip[segmentCount];

        for (int i = 0; i < segmentCount; i++)
        {
            int sampleStartIndex = i * segmentSamples;
            int sampleEndIndex = Mathf.Min((i + 1) * segmentSamples, totalSamples);

            float[] segmentData = new float[sampleEndIndex - sampleStartIndex];
            System.Array.Copy(data, sampleStartIndex, segmentData, 0, segmentData.Length);

            AudioClip segmentClip = AudioClip.Create(clip.name + "_segment_" + (i + 1), segmentData.Length / channels, channels, frequency, false);
            segmentClip.SetData(segmentData, 0);
            segments[i] = segmentClip;
        }

        return segments;
    }
    
    private IEnumerator SendPreRecordedRecording(AudioClip audioClip)
    {
        if (audioClip == null) yield break;

        var speechStr = "";
        var tries = 0;
        
        var audioClips = new List<AudioClip> { audioClip };

        if (audioClip.length > 10f)
        {
            audioClips.AddRange(SplitAudioClip(audioClip, 10));
        }

        while (true)
        {
            foreach (var currentClip in audioClips)
            {
                // Extract data from the provided AudioClip
                float[] samples = new float[currentClip.samples * currentClip.channels];
                currentClip.GetData(samples, 0);

                // Encode the samples as WAV
                byte[] curBytes = EncodeAsWAV(samples, currentClip.frequency, currentClip.channels);

                // Optionally save the file locally for debugging
                //File.WriteAllBytes(Application.dataPath + "/test.wav", curBytes);

                string response = errorResponse;

                while (response == errorResponse && tries < 3)
                {
                    SendRecording(curBytes);

                    yield return new WaitWhile(() => !_receivedResponse );
                    response = currentResponse;
                    tries++;
                }

                if (response != errorResponse)
                {
                    speechStr += " ";
                    speechStr += response;
                }

                tries = 0;
                
            }
            
            resultString = speechStr;
            yield break;
        }
        
    }

    private void SendRecording(byte[] _bytes)
    {
        _receivedResponse = false;
        
        HuggingFaceAPI.AutomaticSpeechRecognition(_bytes, 
            response => 
        {
            text.color = Color.white;
            text.text = response;
            startButton.interactable = true;

            currentResponse = response;
            _receivedResponse = true;
        }, 
            error => 
        {
            
            text.color = Color.red;
            text.text = error;
            startButton.interactable = true; 
                
            
            currentResponse = "!error";
            _receivedResponse = true;
        });
    }
    
    private void SendRecording() {
        text.color = Color.yellow;
        text.text = "Sending...";
        stopButton.interactable = false;
        HuggingFaceAPI.AutomaticSpeechRecognition(_bytes, response => {
            text.color = Color.white;
            text.text = response;
            startButton.interactable = true;
        }, error => {
            text.color = Color.red;
            text.text = error;
            startButton.interactable = true;
        });
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels) {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2)) {
            using (var writer = new BinaryWriter(memoryStream)) {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples) {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }
}
