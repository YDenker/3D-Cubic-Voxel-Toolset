using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NewWindow : ScriptableWizard
{
    private VoxelEditorWindow vew;
    private string objectName = "New Object";
    private bool legalName = true, uniqueName = true;
    public static void CreateWizard(VoxelEditorWindow current)
    {
        NewWindow nw = ScriptableWizard.DisplayWizard<NewWindow>("Create a new Object.", "Create");
        nw.vew = current;
    }

    private void Create()
    {
        if (legalName && uniqueName) // No Name Conflict
        {
            vew.New(objectName);
            Close();
        }
        else
            EditorUtility.DisplayDialog("Name conflict", "The name you have chosen is either illegal or already taken. Please make sure, that no errors are displayed below the name text field!","OK");
    }

    private void OnGUI()
    {
        GUILayout.Label("Name your new object as:", EditorStyles.boldLabel);
        objectName = EditorGUILayout.TextField(objectName);
        legalName = CheckIfNameIsLegal();
        uniqueName = CheckIfNameIsUnique();

        if (!legalName) EditorGUILayout.HelpBox("Illegal naming convention!\nNames must start with a letter,\nbe between 3 and 20 characters long and\ncontain only letters and numbers.", MessageType.Error);
        if (!uniqueName) EditorGUILayout.HelpBox("The name is already taken. Choose another name or delete the SaveFile in the asset database (and loose that object).", MessageType.Error);

        GUILayout.FlexibleSpace();

        if(vew.Loaded) EditorGUILayout.HelpBox("WARNING: All unsaved changes to the current model will be lost!", MessageType.Warning);

        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("Create")) Create();
        if (GUILayout.Button("Back")) Close();
        GUILayout.EndHorizontal();
    }

    private bool CheckIfNameIsUnique()
    {
        // Unfortunately the search does not look for exact nameing. I hope i find a better way to do this...
        string[] guids = AssetDatabase.FindAssets(objectName + " t:ScriptableSaveState", new[] { VoxelEditorWindow.SaveStatePath});
        for (int i = 0; i < guids.Length; i++)
        {
            string[] temp = AssetDatabase.GUIDToAssetPath(guids[i]).Split('/');

            if (temp[temp.Length - 1].ToLower().Equals(objectName.ToLower() + ".asset")) return false;
        }
        return true;
    }

    private bool CheckIfNameIsLegal()
    {
        char[] temp = objectName.ToLower().ToCharArray();
        if (temp.Length <= 2 || temp.Length >= 21) return false;
        if (temp[0] >= 123 || temp[0] <= 96) return false; // Can only start with a letter from the alphabet

        foreach (var letter in temp)
        {
            if ((letter >= 123 || letter <= 96) && (letter >= 58 || letter <= 47) && letter != 32) return false;
        }

        return true;
    }
}
