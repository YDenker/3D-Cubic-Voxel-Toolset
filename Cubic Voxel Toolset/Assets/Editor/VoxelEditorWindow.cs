using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class VoxelEditorWindow : EditorWindow
{

    public static string prefabsFolder ="Prefabs", saveStateFolder = "SaveStates";

    public static string PrefabsPath => "Assets/" + prefabsFolder;
    public static string SaveStatePath => "Assets/" + saveStateFolder;

    private static VoxelEditorWindow instance;

    public static VoxelEditorWindow Instance {
        get
        {
            if (instance) return instance;
            return GetWindow<VoxelEditorWindow>();
        }
        set
        {
            instance = value;
        }
    }

    public bool Loaded => saveState;

    public GameObject ModelParent => parent;

    private ScriptableSaveState saveState;

    private string saveName = "NewObject";
    private GameObject parent;

    private Vector2 scrollView = Vector2.zero;
    
    private bool showFileButtons = true; // Show New, Load, Save, Unload , Import and Export buttons
    private bool showToolExtension = false; // Show extended tool functionality


    [MenuItem("Tools/Voxel Editor")]
    public static void OpenWindow()
    {
        Instance = GetWindow<VoxelEditorWindow>("Voxel Editor");

        // Check folder structure and adjust it
        CheckFolderStructure();
    }

    private void Awake()
    {
        GridGizmos[] temp = FindObjectsOfType<GridGizmos>(true);
        if(!saveState && temp.Length > 0)
        {
            foreach (var obsolete in temp)
            {
                DestroyImmediate(obsolete.gameObject);
            }
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        Unload();
    }

    private void OnGUI()
    {
        int selectedObjects = Selection.count;

        // GUI LAYOUT
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("Name: ", EditorStyles.label);
        GUILayout.Label(saveName, EditorStyles.boldLabel);
        GUILayout.EndHorizontal();

        scrollView = EditorGUILayout.BeginScrollView(scrollView);

        GUILayout.BeginVertical("box");

        showFileButtons = EditorGUILayout.Foldout(showFileButtons, "File");

        if (showFileButtons)
        {
            GUILayout.BeginVertical("box");
            if (GUILayout.Button("New")) New();
            if (GUILayout.Button("Load")) Load();
            if (GUILayout.Button("Save")) Save(); 
            if (GUILayout.Button("Import")) Import();
            if (GUILayout.Button("Export")) Export();
            GUILayout.Label("Debugs:");
            if (GUILayout.Button("Unload")) Unload();
            GUILayout.EndVertical();
        }

        if (Tools.current == Tool.Custom)
        {
            Space();
            showToolExtension = EditorGUILayout.Foldout(showToolExtension, "Tool extension");

            if (showToolExtension)
            {
                GUILayout.BeginVertical("box");

                GUILayout.EndVertical();
            }
        }

        GUILayout.FlexibleSpace();

        EditorGUILayout.HelpBox("This is only a prototype!", MessageType.Info);
        Space();
        GUILayout.Label("Debugs", EditorStyles.boldLabel);
        GUILayout.Label("Currently Selected Objects: "+ selectedObjects);
        GUILayout.Label("Currently Loaded SaveState:");
        EditorGUILayout.ObjectField(saveState, typeof(ScriptableSaveState), true);
        GUILayout.Label("Current Window Instance:\n[" + Instance + "]");
        Space();
        GUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    private void New()
    {
        CheckFolderStructure();
        NewWindow.CreateWizard(this);
    }

    public void New(string objectName)
    {
        saveName = objectName;
        // Remove previous object from scene
        if (saveState) DestroyImmediate(parent);

        // Create new SaveState instance
        saveState = ScriptableObject.CreateInstance<ScriptableSaveState>();

        // Assign basic values to Instance
        parent = new GameObject(saveName)
        {
            layer = LayerMask.NameToLayer("Ignore Raycast")
        };
        parent.AddComponent<GridGizmos>();

        EditorQuit.isLoaded = true;
        Repaint();
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
        Repaint();
    }

    private void Save()
    {
        CheckFolderStructure();
        CreateOrReplacePrefab();

        if (!AssetDatabase.Contains(saveState))
        AssetDatabase.CreateAsset(saveState, SaveStatePath + "/"+saveName + ".asset");

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
            saveName = "--------";
        }
        EditorSceneManager.SaveOpenScenes();
        EditorQuit.isLoaded = false;
        Repaint();
    }

    private void Export()
    {
        Debug.LogWarning("This feature is not implemented yet");
    }

    private void Import()
    {
        Debug.LogWarning("This feature is not implemented yet");
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
                AssetDatabase.RenameAsset(SaveStatePath +"/"+ saveState.prefab.name + ".asset", saveName);
            }
            else saveName = saveState.objectName;
            
        }
        saveState.objectName = saveName;
        saveState.prefab = PrefabUtility.SaveAsPrefabAsset(parent,PrefabsPath +"/" + saveName + ".prefab");
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
