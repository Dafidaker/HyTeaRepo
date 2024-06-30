using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GestureEditor : EditorWindow
{
    [MenuItem("Window/UI Toolkit/GestureEditor")]
    public static void ShowExample()
    {
        GestureEditor wnd = GetWindow<GestureEditor>();
        wnd.titleContent = new GUIContent("GestureEditor");
    }

    private VisualElement _rightPane;
    
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Get a list of all sprites in the project
        var allGestureHolderGuid = AssetDatabase.FindAssets("t:GestureHolder");
        var allGestureHolder = new List<GestureHolder>();
        foreach (string guid in allGestureHolderGuid)
        {
            allGestureHolder.Add(AssetDatabase.LoadAssetAtPath<GestureHolder>(AssetDatabase.GUIDToAssetPath(guid)));
        }
        
        // Create a two-pane view with the left pane being fixed.
        var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);

        // Add the view to the visual tree by adding it as a child to the root element.
        rootVisualElement.Add(splitView);

        // A TwoPaneSplitView needs exactly two child elements.
        var leftPane = new ListView();
        splitView.Add(leftPane);
        _rightPane = new VisualElement();
        splitView.Add(_rightPane);
        
        // Initialize the list view with all sprites' names
        leftPane.makeItem = () => new Label();
        leftPane.bindItem = (item, index) => { ((Label)item).text = allGestureHolder[index].Name; };
        leftPane.itemsSource = allGestureHolder;
        
        leftPane.selectionChanged += OnGestureHolderSelectionChanged;

    }
    
    private void OnGestureHolderSelectionChanged(IEnumerable<object> selectedItems)
    {
        // Clear all previous content from the pane.
        _rightPane.Clear();

        // Get the selected sprite and display it.
        using var enumerator = selectedItems.GetEnumerator();
        if (enumerator.MoveNext())
        {
            var selectedHolder = enumerator.Current as GestureHolder;
            if (selectedHolder != null)
            {
                // Create a label that shows the duration of the gestureholder
                var durationLabel = new Label($"Duration: {selectedHolder.Duration}");

                var textField = new TextField("Duration");
                textField.SetValueWithoutNotify(selectedHolder.Duration.ToString());

                
                textField.RegisterValueChangedCallback(evt =>
                {
                    if (!float.TryParse(evt.newValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) &&
                        !Regex.IsMatch(evt.newValue, @"^\d*\.?$"))
                    {
                        textField.SetValueWithoutNotify(evt.previousValue);
                    }
                    else if (Regex.IsMatch(evt.newValue, @"^\d*\.?$"))
                    {
                        textField.SetValueWithoutNotify(result.ToString(CultureInfo.InvariantCulture));
                    }
                });
                
                // Add the Image control to the right-hand pane.
                _rightPane.Add(durationLabel);
                _rightPane.Add(textField);
            }
        }
        
    }
}
