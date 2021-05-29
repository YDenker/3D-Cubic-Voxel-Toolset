using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGizmos : MonoBehaviour
{
    [Header("Ground Grid")]
    public Vector2 groundGrid = new Vector2(1000, 1000);
    public bool enableGroundGrid = true;
    public Color groundGridColor = Color.grey;

    public void OnDrawGizmos()
    {
        //Draw the ground grid
        if(enableGroundGrid)
            GroundGrid(groundGridColor);
    }

    private void GroundGrid(Color color)
    {
        Gizmos.color = color;
        for (float z = -(groundGrid.y / 2); z < (groundGrid.y / 2); z++)
        {
            Gizmos.DrawLine(new Vector3(-(groundGrid.x / 2), -0.5f, z + 0.5f),
                            new Vector3((groundGrid.x / 2), -0.5f, z + 0.5f));
        }
        for (float x = -(groundGrid.x / 2); x < (groundGrid.x / 2); x++)
        {
            Gizmos.DrawLine(new Vector3(x + 0.5f, -0.5f, -(groundGrid.y / 2)),
                            new Vector3(x + 0.5f, -0.5f, (groundGrid.y / 2)));
        }
        Gizmos.color = Color.white;
    }
}
