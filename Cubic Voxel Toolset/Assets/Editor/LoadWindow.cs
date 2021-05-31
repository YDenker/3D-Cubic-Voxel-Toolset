using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LoadWindow : ScriptableWizard
{
    public ScriptableSaveState save;
    private VoxelEditorWindow vew;
    private Texture2D preview;

    public static void CreateWizard(VoxelEditorWindow current)
    {
        LoadWindow lw = ScriptableWizard.DisplayWizard<LoadWindow>("Select SaveState to load.", "Load");
        lw.vew = current;
    }

    private void Load()
    {
        if (save != null)
        {
            vew.Load(save);
            Close();
            return;
        }
        Debug.LogWarning("Could not load. Missing SaveState!");
    }

    private void OnGUI()
    {
        GUILayout.Label("Please select a SaveState:", EditorStyles.boldLabel);
        Space(5);
        save = (ScriptableSaveState)EditorGUILayout.ObjectField(save, typeof(ScriptableSaveState), true);
        if (save) preview = AssetPreview.GetAssetPreview(save.prefab);
        else preview = null;
        
        GUILayout.BeginHorizontal("box");
        var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        GUILayout.Label(preview,style);
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        if (vew.Loaded) EditorGUILayout.HelpBox("Warning: All unsaved changes to the current model will be lost!", MessageType.Warning);

        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("Load")) Load();
        if (GUILayout.Button("Back")) Close();
        GUILayout.EndHorizontal();
    }

    private void Space(float pixels = 10)
    {
        GUILayout.Space(pixels);
    }

}
