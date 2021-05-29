using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScriptableSaveState))]
public class ScriptableSaveStateEditor : Editor
{
    ScriptableSaveState reference;
    public void OnEnable()
    {
        reference = (ScriptableSaveState)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Please do not try and delete opened objects. Unload the save first! I have not fixed a bug that occures when you don't follow this instruction!", MessageType.Warning);
        if(GUILayout.Button("Delete this save completely!"))
        {
            if (EditorUtility.DisplayDialog("Are you sure?", "If you delete this save, there is no way you can get it back! (unless you exported it already). Also, make sure that the object you want to delete IS unloaded.", "DELETE", "CANCEL"))
                reference.DeleteSave();
        }
    }
}
