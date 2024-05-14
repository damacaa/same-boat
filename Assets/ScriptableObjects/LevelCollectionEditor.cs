using Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;




#if UNITY_EDITOR
[CustomEditor(typeof(LevelCollection))]
public class LevelCollectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Cast target to your ScriptableObject type
        LevelCollection collection = (LevelCollection)target;

        // Display default inspector
        DrawDefaultInspector();

        // Button
        EditorGUILayout.Space(10);
        if (GUILayout.Button("Reset descriptions"))
        {
           collection.UpdateDescriptions();
        }
    }
}
#endif


