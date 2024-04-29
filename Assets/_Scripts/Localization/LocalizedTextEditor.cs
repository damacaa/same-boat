using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Localization
{
    [CustomEditor(typeof(LocalizedText))]
    public class LocalizedTextEditor : Editor
    {
        // This method is called to draw the Inspector
        public override void OnInspectorGUI()
        {
            // Get the target object (the instance of MyCustomClass being edited)
            LocalizedText text = (LocalizedText)target;

            //DrawDefaultInspector();

            foreach (Language language in Enum.GetValues(typeof(Language)))
            {
                string title = language.ToString();
                string content = EditorGUILayout.TextField(
                    title,
                    text.GetText(language));

                // Store new value
                text.SetText(language, content);
            }

            // Apply changes if any
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}