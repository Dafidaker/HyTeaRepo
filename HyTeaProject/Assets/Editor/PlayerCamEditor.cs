using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerCam))]
public class PlayerCamEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerCam myScript = (PlayerCam)target;
        if (GUILayout.Button("LockCamera"))
        {
            myScript.LockCamera();
        }
        
        if (GUILayout.Button("UnlockCamera"))
        {
            myScript.UnlockCamera();
        }
    }
}

