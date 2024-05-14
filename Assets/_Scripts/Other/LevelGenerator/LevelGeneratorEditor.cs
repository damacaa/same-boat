#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelGenerator generator = (LevelGenerator)target;

        DrawDefaultInspector();

        if (!generator.IsGenerating)
        {

            if (GUILayout.Button("Generate level"))
            {
                generator.Generate();
            }
        }
        else
        {


            if (GUILayout.Button("Cancel"))
            {
                generator.Cancel();
            }
        }
    }
}

#endif