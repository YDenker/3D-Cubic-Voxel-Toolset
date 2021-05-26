using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

[EditorTool("Voxel Mesh Tool")]
public class MeshEditorTool : EditorTool
{
    [SerializeField]
    Texture2D m_ToolIcon;

    GUIContent m_IconContent;

    private void OnEnable()
    {
        m_IconContent = new GUIContent()
        {
            image = m_ToolIcon,
            text = "Voxel Mesh Tool",
            tooltip = "To Add and Remove cubes from an existing Voxel Mesh"
        };
    }

    public override GUIContent toolbarIcon => base.toolbarIcon; // Can be own icon



    public override void OnToolGUI(EditorWindow window)
    {
        EditorGUI.BeginChangeCheck();

        Vector3 position = new Vector3((int)Tools.handlePosition.x, (int)Tools.handlePosition.y, (int)Tools.handlePosition.z);
        float posX = Tools.handlePosition.x,
            posY = Tools.handlePosition.y, 
            posZ = Tools.handlePosition.z;

        using (new Handles.DrawingScope(Color.red)) 
        {
            posX = (int)Handles.Slider(position, Vector3.right).x;           
        }
        using (new Handles.DrawingScope(Color.green))
        {
            posY = (int)Handles.Slider(position, Vector3.up).y;
        }
        using (new Handles.DrawingScope(Color.blue))
        {
            posZ = (int)Handles.Slider(position, Vector3.forward).z;
        }

        position = new Vector3(posX,posY,posZ);

        if (EditorGUI.EndChangeCheck())
        {
            Vector3 delta = position - Tools.handlePosition;
            Undo.RecordObjects(Selection.transforms, "Move Thing");

            foreach( var transform in Selection.transforms)
            {
                transform.position += delta;
                transform.gameObject.name = ((int)transform.position.x).ToString() + "|" + ((int)transform.position.y).ToString() + "|" + ((int)transform.position.z).ToString();
            }
        }
    }

}
