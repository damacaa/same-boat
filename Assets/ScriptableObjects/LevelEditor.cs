using Localization;
using System;
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
        Level levelSO = (Level)target;

        // Add a whimsical text area for extra charm
        EditorGUILayout.Space(); // Create some space for visual appeal



        EditorStyles.textField.wordWrap = true;

        foreach (Language language in Enum.GetValues(typeof(Language)))
        {
            GUILayout.Label($"Name({language})", EditorStyles.boldLabel);
            levelSO.SetName(EditorGUILayout.TextField(levelSO.GetName(language)), language);

            string title = $"Description";

            GUILayout.Label(title, EditorStyles.boldLabel); // Label for the section
            levelSO.SetDescription(EditorGUILayout.TextArea(levelSO.GetDescription(language)), language);

            if (GUILayout.Button("Generate"))
            {
                levelSO.SetDescription(levelSO.GenerateDescription(language), language);
            }

            EditorGUILayout.Space(20);

            // Store new value
            //text.SetText(language, content);
        }

        EditorGUILayout.Space(100);

        // Display default inspector
        DrawDefaultInspector();

        EditorUtility.SetDirty(levelSO);
    }
}
#endif