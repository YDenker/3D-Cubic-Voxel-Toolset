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

    //GUIControl
    private float sceneHeight, sceneWidth;

    private readonly float guiWidth = 200f;
    private readonly float guiHeight = 50f;
    private readonly float guiOffset = 10f;

    private readonly float buttonWidth = 60f; //guiWidth / 4 +10;
    private readonly float buttonHeight = 25f; //guiHeight/2;
    private readonly float buttonGap = 5f;

    // Raycast

    private RaycastHit hit;
    private bool mouseOver = false;
    Vector3 centerCube = Vector3.zero;


    public ToggleGroup activeTool = new ToggleGroup(true);

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
            MouseHover();
            SceneGUI(sceneview);
        } else if (IsActive)
        {
            IsActive = false;
            Tools.lockedLayers = tempLayermask;
        }

        // Do Stuff that happens only when the tool is not active

    }

    private void SceneGUI(SceneView sceneview)
    {
        // Do Stuff that happens only when the tool is active
        sceneHeight = sceneview.camera.pixelRect.height;
        sceneWidth = sceneview.camera.pixelRect.width;

        // Scene GUI
        Handles.BeginGUI();

        GUI.Box(new Rect(sceneWidth/2 - guiWidth/2, guiOffset, guiWidth, guiHeight), "Tools");

        activeTool.Set(0, GUI.Toggle(new Rect(sceneWidth / 2 - guiWidth / 2 + 1*buttonGap + 0* buttonWidth, guiOffset + guiHeight / 2.5f, buttonWidth, buttonHeight), activeTool.Get(0), "Move", "Button"));
        activeTool.Set(1, GUI.Toggle(new Rect(sceneWidth / 2 - guiWidth / 2 + 2 * buttonGap + 1 * buttonWidth, guiOffset + guiHeight / 2.5f, buttonWidth, buttonHeight), activeTool.Get(1), "Add", "Button"));
        activeTool.Set(2, GUI.Toggle(new Rect(sceneWidth / 2 - guiWidth / 2 + 3 * buttonGap + 2 * buttonWidth, guiOffset + guiHeight / 2.5f, buttonWidth, buttonHeight), activeTool.Get(2), "Delete", "Button"));

        if (activeTool.Get(2))
        {
            GUI.Box(new Rect(sceneWidth / 2 - guiWidth / 2, sceneHeight - guiHeight - guiOffset, guiWidth, guiHeight), "[Enter], [Back], [ESC]");
            if (GUI.Button(new Rect(sceneWidth / 2 - guiWidth / 2 + 2*buttonGap, sceneHeight - guiOffset - guiHeight / 1.75f, buttonWidth * 3, buttonHeight), "Delete current selection"))
            {
                DeleteSelection();
            }
        }

        Handles.EndGUI();
    }

    private void MouseHover()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        mouseOver = Physics.Raycast(ray, out hit, 100f);
    }

    public override void OnToolGUI(EditorWindow window)
    {
        if(activeTool.Get(0))
            MoveTool();
        else if (activeTool.Get(1))
            AddTool();
        else if (activeTool.Get(2))
            DeleteTool();
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

            foreach (var t in Selection.transforms)
            {
                if (!t.TryGetComponent<GridGizmos>(out GridGizmos gizmos))
                {
                    t.position += delta;
                    t.gameObject.name = ((int)t.position.x).ToString() + "|" + ((int)t.position.y).ToString() + "|" + ((int)t.position.z).ToString();
                }
                else Debug.LogWarning("You are trying to move the parent. To move the parent object is not allowed :)! Simply deselect the parent in the hierachy");
            }
        }
    }

    private void AddTool()
    {
        Event e = Event.current;
        if (mouseOver)
        {
            Vector3 temp = centerCube;
            Vector3 hitpoint = hit.transform.position - hit.point;

            //Below
            if (hitpoint.y >= 0.5) centerCube = hit.transform.position - Vector3.up;
            //Above
            else if (hitpoint.y <= -0.5) centerCube = hit.transform.position + Vector3.up;
            //Left
            else if (hitpoint.x >= 0.5) centerCube = hit.transform.position - Vector3.right;
            //Right
            else if (hitpoint.x <= -0.5) centerCube = hit.transform.position + Vector3.right;
            //Front (-z)
            else if (hitpoint.z >= 0.5) centerCube = hit.transform.position - Vector3.forward;
            //Back  (+z)
            else if (hitpoint.z <= -0.5) centerCube = hit.transform.position + Vector3.forward;

            if(centerCube.y >= 0f)
            {
                if (e.isMouse && e.type == EventType.MouseDown && e.button == 0)
                {
                    GameObject go = Instantiate(cubePrefab, centerCube, Quaternion.identity, VoxelEditorWindow.Instance.ModelParent.transform);
                    go.name = ((int)centerCube.x).ToString() + "|" + ((int)centerCube.y).ToString() + "|" + ((int)centerCube.z).ToString();
                }
                using(new Handles.DrawingScope(Color.red))
                {
                    Handles.DrawWireCube(centerCube, Vector3.one);
                }

                if(!temp.Equals(centerCube))
                    SceneView.RepaintAll();
            }
        }
    }

    private void DeleteTool()
    {
        Event e = Event.current;
        if (e.isKey && (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.Backspace || e.keyCode == KeyCode.Delete))
        {
            DeleteSelection();
        }
    }

    private void DeleteSelection()
    {
        foreach (var transform in Selection.transforms)
        {
            if (!transform.TryGetComponent(out GridGizmos _))
            {
                DestroyImmediate(transform.gameObject);
            }
            else Debug.LogWarning("You are trying to delete the parent. Deleting the parent object is not allowed :)! Simply deselect the parent in the hierachy");
        }
    }
}

public struct ToggleGroup
{
    private int index;
    private readonly bool canHaveNoSelected;

    public ToggleGroup(bool canHaveNoSelected = false)
    {
        this.canHaveNoSelected = canHaveNoSelected;
        index = canHaveNoSelected ? -1 : 0;
    }

    public bool Get(int index) => this.index == index;
    public void Set(int index, bool value)
    {
        this.index = value ? index : (this.index == index && canHaveNoSelected) ? -1 : this.index;
    }
}
