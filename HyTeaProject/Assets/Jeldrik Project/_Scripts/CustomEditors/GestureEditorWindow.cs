#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;

public class GestureEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("Tools/GPE/GestureEditor")]
        private static void Open()
        {
            var window = GetWindow<GestureEditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true);
            
            tree.Add("GestureHolder" , new ScriptableObjectTable<GestureHolder>());
            tree.Add("BaseGestures" , new ScriptableObjectTable<Gesture>());
            
            return tree;
        }
    }
#endif
