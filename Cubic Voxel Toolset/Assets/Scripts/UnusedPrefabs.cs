using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnusedPrefabs : ScriptableObject
{
    private static UnusedPrefabs instance = null;

    public static UnusedPrefabs Instance
    {
        get
        {
            if (!instance) instance = Resources.FindObjectsOfTypeAll<UnusedPrefabs>().FirstOrDefault();
            return instance;
        }
    }

    public List<GameObject> prefabs = new List<GameObject>();

    public void Add(GameObject prefab)
    {
        prefabs.Add(prefab);
    }
}
