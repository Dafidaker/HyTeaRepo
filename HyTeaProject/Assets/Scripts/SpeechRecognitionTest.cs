using System.IO;
using HuggingFace.API;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeechRecognitionTest : MonoBehaviour 
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private TextMeshProUGUI text;

    private AudioClip _clip;
    private byte[] _bytes;
    private bool _recording;
    
    private int _microphoneIndex;

    private void Start() 
    {
        _microphoneIndex = 0;
        startButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);
        stopButton.interactable = false;
    }

    private void Update() {
        if (_recording && Microphone.GetPosition(Microphone.devices[_microphoneIndex]) >= _clip.samples) {
            StopRecording();
        }
    }
    
    private void OnEnable()
    {
        MicrophoneSelector.OnMicrophoneChanged += ChangedMicrophone;
    }

    private void OnDisable()
    {
        MicrophoneSelector.OnMicrophoneChanged -= ChangedMicrophone;
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
