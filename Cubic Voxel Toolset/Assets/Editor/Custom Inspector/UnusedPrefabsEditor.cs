using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnusedPrefabs))]
public class UnusedPrefabsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.HelpBox("This container holds all the unused prefabs. If you know what you are doing you can use them to restore lost objects.",MessageType.Info);
    }
}
