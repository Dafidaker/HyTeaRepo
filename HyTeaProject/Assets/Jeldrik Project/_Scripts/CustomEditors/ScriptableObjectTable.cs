using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class ScriptableObjectTable<T> where T : ScriptableObject
{
    [TableList(IsReadOnly = true, AlwaysExpanded = true), ShowInInspector]
    private List<T> _allGestureHolder = FetchAllObjects();
    
    private static List<T> FetchAllObjects()
    {
        return AssetDatabase.FindAssets("t:" + typeof(T).Name).Select( AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<T>).ToList();
    }

    public void UpdateObjects()
    {
        _allGestureHolder = FetchAllObjects();
    }
}
