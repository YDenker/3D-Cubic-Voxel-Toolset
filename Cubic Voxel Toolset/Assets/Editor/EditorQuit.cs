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
            return EditorUtility.DisplayDialog("Before You Quit!", "Before you quit, please unload the current Model you are working on!\nNot doing so can create problems in the future.", "Quit", "Cancle");
        else
            return true;

    }

    static EditorQuit()
    {
        EditorApplication.wantsToQuit += Quit;
    }
}
