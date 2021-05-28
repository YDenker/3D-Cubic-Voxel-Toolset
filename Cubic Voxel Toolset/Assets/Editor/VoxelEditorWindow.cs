using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class VoxelEditorWindow : EditorWindow
{

    public static string prefabsFolder ="Prefabs", saveStateFolder = "SaveStates";

    private static string prefabsPath => "Assets/" + prefabsFolder + "/";
    private static string saveStatePath => "Assets/" + saveStateFolder + "/";

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
        int selectedCubes = Selection.count;

        // GUI LAYOUT

        Space();
        saveName = EditorGUILayout.TextField("Name",saveName);
        Space();
        if (GUILayout.Button("New")) New();
        if (GUILayout.Button("Load")) Load();
        if (GUILayout.Button("Save")) Save(); 
        if (GUILayout.Button("Unload")) Unload();

        GUILayout.FlexibleSpace();

        EditorGUILayout.HelpBox("This is only a prototype!", MessageType.Info);
        Space();
        GUILayout.Label("Debugs", EditorStyles.boldLabel);
        GUILayout.Label("Currently Selected Cubes: "+ selectedCubes);
        GUILayout.Label("Currently Loaded SaveState:");
        EditorGUILayout.ObjectField(saveState, typeof(ScriptableSaveState), true);
        Space();
    }

    private void New()
    {
        CheckFolderStructure();
        if (NewObjectDialogs()) 
        { 
            // Remove previous object from scene
            if (saveState) DestroyImmediate(parent);

            // Create new SaveState instance
            saveState = ScriptableObject.CreateInstance<ScriptableSaveState>();

            // Assign basic values to Instance
            parent = new GameObject(saveName);
        }

        EditorQuit.isLoaded = true;
    }

    private void Load()
    {
        CheckFolderStructure();
        LoadWindow.CreateWizard(this);
    }

    public void Load(ScriptableSaveState save)
    {
        // Remove previous object from scene
        if (saveState) DestroyImmediate(parent);

        saveState = save;
        saveName = save.objectName;
        parent = Instantiate(saveState.prefab);

        EditorQuit.isLoaded = true;
    }

    private void Save()
    {
        CheckFolderStructure();
        CreateOrReplacePrefab();

        if (!AssetDatabase.Contains(saveState))
        AssetDatabase.CreateAsset(saveState, saveStatePath + saveName + ".asset");

        AssetDatabase.SaveAssets();
    }

    private void Unload()
    {
        if (UnloadDialog())
        {
            // Remove previous object from scene
            if (saveState) DestroyImmediate(parent);
            saveState = null;
            parent = null;
            saveName = "NewObject";
        }
        EditorSceneManager.SaveOpenScenes();
        EditorQuit.isLoaded = false;
    }

    private bool NewObjectDialogs()
    {
        return EditorUtility.DisplayDialog("Loose all current unsaved changes?", "Are you sure you want to create a new object with the name:\""+saveName+"\" to work on?\nAll unsaved changes to the current object will be lost!", "Yes create new", "Wait let me save first!");
    }

    private bool UnloadDialog()
    {
        return EditorUtility.DisplayDialog("Unload Current Object?","WARNING!\nAll unsaved changes will be lost!","Continue","Abort");
    }

    private bool ChangedObjectNameOverrideDialog()
    {
        return EditorUtility.DisplayDialog("You changed the name!", "You changed the name of the object. Do you wish to save the last save as a copy that is hold as an unused prefab for quick recovery or keep the name?", "keep older save!", "use old name!");
    }

    private void CreateOrReplacePrefab()
    {
        if (saveState.prefab && saveState.objectName != saveName)
        {
            if (ChangedObjectNameOverrideDialog())
            {
                UnusedPrefabs.Instance.Add(saveState.prefab);
                AssetDatabase.RenameAsset(saveStatePath + saveState.prefab.name + ".asset", saveName);
            }
            else saveName = saveState.objectName;
            
        }
        saveState.objectName = saveName;
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
