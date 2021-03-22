using System.Collections.Generic;
using UnityEngine;

public class Model
{
    public string name;
    public float dimension;
    public int cubeAmount;
    public Dictionary<Vector3Int,Cubicle> cubes;

    public Model(string name = "empty", float dimension = 1f)
    {
        this.name = name;
        this.dimension = dimension;
        cubeAmount = 0;
        cubes = new Dictionary<Vector3Int, Cubicle>();
    }

    public void AddCube(Cubicle cube)
    {
        if (cubes.ContainsKey(cube.coordinates)) Debug.LogError("Cube already added");
        else cubes.Add(cube.coordinates, cube);
    }

    public void AddCubeAt(Vector3Int coordinates)
    {
        AddCube(new Cubicle(coordinates));
    }

    public void RemoveCube(Cubicle cube)
    {
        RemoveCubeAt(cube.coordinates);
    }
    public void RemoveCubeAt(Vector3Int coordinates)
    {
        if (cubes.ContainsKey(coordinates)) cubes.Remove(coordinates);
        else Debug.LogError("Cube cannot be removed! It does not exsist");
    }
}
