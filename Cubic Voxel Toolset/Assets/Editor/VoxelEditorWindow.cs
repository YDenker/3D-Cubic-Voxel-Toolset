using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VoxelEditorWindow : EditorWindow
{

    public static string prefabsFolder ="Prefabs", saveStateFolder = "SaveStates";

    private string prefabsPath => "Assets/" + prefabsFolder + "/";
    private string saveStatePath => "Assets/" + saveStateFolder + "/";

    private ScriptableSaveState saveState;

    private string saveName = "NewObject";
    private GameObject parent;


    [MenuItem("Tools/Voxel Editor")]
    public static void OpenWindow()
    {
        GetWindow<VoxelEditorWindow>("Voxel Editor");

        // Check folder structure and adjust it
        CheckFolderStructure();
    }


    private void OnGUI()
    {
        Tools.current = Tool.Custom;

        int selectedCubes = Selection.count;

        // GUI LAYOUT

        Space();
        saveName = EditorGUILayout.TextField("Name",saveName);
        Space();
        if (GUILayout.Button("New")) New();
        if (GUILayout.Button("Load")) Load();
        if (GUILayout.Button("Save")) Save();

        Space();
        EditorGUILayout.HelpBox("This is only a prototype!", MessageType.Info);
        Space();
        GUILayout.Label("Debugs", EditorStyles.boldLabel);
        GUILayout.Label("Currently Selected Cubes: "+ selectedCubes);
    }

    private void New()
    {
        if (NewObjectDialogs()) 
        { 
            // Remove previous object from scene
            if (saveState) DestroyImmediate(parent);

            // Create new SaveState instance
            saveState = ScriptableObject.CreateInstance<ScriptableSaveState>();

            // Assign basic values to Instance
            parent = new GameObject(saveName);
            Save();
        }
    }

    private void Load()
    {
        CheckFolderStructure();
    }

    private void Save()
    {
        CheckFolderStructure();
        saveState.objectName = saveName;
        CreateOrReplacePrefab();

        if (!AssetDatabase.Contains(saveState))
        AssetDatabase.CreateAsset(saveState, saveStatePath + saveName + ".asset");

        AssetDatabase.SaveAssets();
    }

    private bool NewObjectDialogs()
    {
        return EditorUtility.DisplayDialog("Loose all current unsaved changes?", "Are you sure you want to create a new object to work on?\nAll unsaved changes to the current object will be lost!", "Yes create new", "Wait let me save first!");
    }

    private void CreateOrReplacePrefab()
    {
        saveState.prefab = PrefabUtility.SaveAsPrefabAsset(parent,prefabsPath + saveName + ".prefab");
    }

    private static void CheckFolderStructure()
    {
        if (!AssetDatabase.IsValidFolder("Assets/" + prefabsFolder)) AssetDatabase.CreateFolder("Assets", prefabsFolder);
        if (!AssetDatabase.IsValidFolder("Assets/" + saveStateFolder)) AssetDatabase.CreateFolder("Assets", saveStateFolder);
    }

    private void Space(float pixels = 10)
    {
        GUILayout.Space(pixels);
    }
}
