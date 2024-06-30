using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/GestureHolder")]
public class GestureHolder : ScriptableObject
{
    public string Name;
    public float Duration;
    public float Cooldown;
    [TableList,InlineEditor(InlineEditorModes.FullEditor, PreviewWidth = 100), TableColumnWidth(500)]
    public List<Gesture> Gestures;

    public List<PoseTrackingInfo.LandmarkNames> GetNeededTrackers()
    {
        List<PoseTrackingInfo.LandmarkNames> neededLandMarks = new List<PoseTrackingInfo.LandmarkNames>();

        foreach (var gesture in Gestures)
        {
            neededLandMarks.AddRange(gesture.GetNeededTrackers());
        }
        
        return neededLandMarks;
    }
}
