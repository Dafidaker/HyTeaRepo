using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PresentationEvaluationUIController))]

public class PresentationEvaluationUIControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PresentationEvaluationUIController myScript = (PresentationEvaluationUIController)target;
        if (GUILayout.Button("Show Feedback"))
        {
            myScript.PopulateScreenEditorButton();
        }
    }
}
