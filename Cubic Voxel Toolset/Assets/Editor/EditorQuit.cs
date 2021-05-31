using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class EditorQuit
{
    public static bool isLoaded = false;

    static bool Quit()
    {
        if (isLoaded)
            return EditorUtility.DisplayDialog("Before You Quit!", "All unsaved changes that you made on the current model will be lost.", "Quit", "Cancle");

        return true;
    }

    static EditorQuit()
    {
        EditorApplication.wantsToQuit += Quit;
    }
}
