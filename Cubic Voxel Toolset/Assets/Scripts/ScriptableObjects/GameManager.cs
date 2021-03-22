using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ToolManager", menuName="ScriptableObjects/Managers/ToolManager",order = 1)]
public class GameManager : ScriptableObject
{
    public Model currentModel;

    public List<Model> models = new List<Model>();


    public bool SaveCurrentModel()
    {
        throw new NotImplementedException();
    }

    public bool LoadModel(int index)
    {
        throw new NotImplementedException();
    }
}
