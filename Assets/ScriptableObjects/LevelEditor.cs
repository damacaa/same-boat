using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Cast target to your ScriptableObject type
        Level yourScriptableObject = (Level)target;

        // Add a whimsical text area for extra charm
        EditorGUILayout.Space(); // Create some space for visual appeal

        GUILayout.Label("Name", EditorStyles.boldLabel); // Label for the section
        yourScriptableObject.name = EditorGUILayout.TextField(yourScriptableObject.name); // Text area for custom notes

        GUILayout.Label("Description", EditorStyles.boldLabel); // Label for the section
        yourScriptableObject.Description = EditorGUILayout.TextArea(yourScriptableObject.Description); // Text area for custom notes
        EditorGUILayout.Space();

        // Display default inspector
        DrawDefaultInspector();
    }
}
#endif