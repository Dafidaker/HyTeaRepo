using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PresentationManager))]

public class PresentationManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PresentationManager myScript = (PresentationManager)target;
        if (GUILayout.Button("End Presentation"))
        {
            myScript.HandleEndPresentation();
        }
    }
}

