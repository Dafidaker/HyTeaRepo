using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PoseEditor : EditorWindow
{
    [MenuItem("Window/UI Toolkit/PoseEditor")]
    public static void ShowExample()
    {
        PoseEditor wnd = GetWindow<PoseEditor>();
        wnd.titleContent = new GUIContent("PoseEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);
        
    }
}
