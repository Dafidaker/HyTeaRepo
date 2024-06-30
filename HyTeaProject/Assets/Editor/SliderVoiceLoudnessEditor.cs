using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SliderVoiceLoudness))]
public class SliderVoiceLoudnessEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SliderVoiceLoudness myScript = (SliderVoiceLoudness)target;
        if (GUILayout.Button("Create Dividors"))
        {
            myScript.ButtonCallExample();
        }
    }
}
