using System;
using System.IO;
using UnityEngine;

//
// Summary:
//     Generic class for storing Editor state.
public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
{
    private static T s_Instance;

    public static T instance
    {
        get
        {
            if (s_Instance == null)
            {
                CreateAndLoad();
            }

            return s_Instance;
        }
    }

    protected ScriptableSingleton()
    {
        if (s_Instance != null)
        {
            Debug.LogError("ScriptableSingleton already exists. Did you query the singleton in a constructor?");
        }
        else
        {
            s_Instance = this as T;
        }
    }

    private static void CreateAndLoad()
    {
        if (s_Instance == null)
        {
            T val = CreateInstance<T>();
            val.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
        }
    }
}