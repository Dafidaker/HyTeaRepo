using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MicrophoneManager))]
public class MicrophoneManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MicrophoneManager myScript = (MicrophoneManager)target;
        if (GUILayout.Button("Start Recording"))
        {
            myScript.RecordMicrophone();
        }
        
        if (GUILayout.Button("Stop Recording"))
        {
            myScript.StopRecording();
        }
        
        
    }
}
