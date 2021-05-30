using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditorInternal;

[EditorTool("Voxel Mesh Tool")]
public class MeshEditorTool : EditorTool
{
    [SerializeField]
    private Texture2D m_ToolIcon;

    private GUIContent m_IconContent;

    [SerializeField]
    private GameObject cubePrefab;

    private LayerMask tempLayermask;

    public static bool IsActive { get; private set; }

    private void OnEnable()
    {
        m_IconContent = new GUIContent()
        {
            image = m_ToolIcon,
            text = "Voxel Mesh Tool",
            tooltip = "To Add and Remove cubes from an existing Voxel Mesh"
        };
        SceneView.duringSceneGui += OnSceneGUI;
    }

    public void OnDisable()
    {
        Tools.current = Tool.Move;
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    public override GUIContent toolbarIcon => m_IconContent; // base.toolbarIcon

    private void OnSceneGUI(SceneView sceneview)
    {
        if (ToolManager.IsActiveTool(this) && !IsActive)
        {
            IsActive = true;
            tempLayermask = Tools.lockedLayers;
            Tools.lockedLayers = LayerMask.GetMask("Ignore Raycast");
            return;
        } else if (ToolManager.IsActiveTool(this)) 
        {
            // Do Stuff that happens only when the tool is active
            Event e = Event.current;

            if (e.isKey && e.character == 'a') Debug.Log("A");

        } else if (IsActive)
        {
            IsActive = false;
            Tools.lockedLayers = tempLayermask;
        }


        // Do Stuff that happens only when the tool is not active
        
        
    }

    public override void OnToolGUI(EditorWindow window)
    {
        MoveTool();
    }

    private void MoveTool()
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

        position = new Vector3(posX, Mathf.Max(0,posY), posZ);

        if (EditorGUI.EndChangeCheck())
        {
            Vector3 delta = position - Tools.handlePosition;
            Undo.RecordObjects(Selection.transforms, "Move Thing");

            foreach (var transform in Selection.transforms)
            {
                if (!transform.TryGetComponent<GridGizmos>(out GridGizmos gizmos))
                {
                    transform.position += delta;
                    transform.gameObject.name = ((int)transform.position.x).ToString() + "|" + ((int)transform.position.y).ToString() + "|" + ((int)transform.position.z).ToString();
                }
                else Debug.LogWarning("You are trying to move the parent. To move the parent object is not allowed :)! Simply deselect the parent in the hierachy");
            }
        }
    }
}
