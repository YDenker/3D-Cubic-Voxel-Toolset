using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScriptableSaveState : ScriptableObject
{
    public string objectName;
    public GameObject prefab;

    public void DeleteSave()
    {
        AssetDatabase.GetAssetPath(prefab);
        string prefabPath = AssetDatabase.GetAssetPath(prefab);
        AssetDatabase.DeleteAsset(prefabPath);
        string saveStatePath = AssetDatabase.GetAssetPath(this);
        AssetDatabase.DeleteAsset(saveStatePath);
    }
}
