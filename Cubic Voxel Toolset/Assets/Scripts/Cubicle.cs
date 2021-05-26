using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cubicle
{
    public Vector3Int coordinates;
    public Color32 colorRGBA;
    [Tooltip("[up,down,front,back,left,right]")]
    public bool[] faces;


    public Cubicle(Vector3Int coordinates = new Vector3Int(), Color32 colorRGBA = new Color32())
    {
        this.coordinates = coordinates;
        this.colorRGBA = colorRGBA;
        faces = new bool[6];
    }
}
