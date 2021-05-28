using UnityEngine;
using UnityEditor;

public class ExampleWindow : EditorWindow
{

    string myString = "Hello World!";

    [MenuItem("Window/Example")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<ExampleWindow>("Example");
    }

    private void OnGUI()
    {
        // Window Code

        //spaces, buttons and labels
        GUILayout.Label("This is a lable", EditorStyles.boldLabel);

        //Fields and Properties
        myString = EditorGUILayout.TextField("Name", myString);

        if(GUILayout.Button("Press Me"))
        {

        }
    }

}
