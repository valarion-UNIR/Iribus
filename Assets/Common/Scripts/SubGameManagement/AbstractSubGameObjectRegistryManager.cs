using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AbstractSubGameObjectRegistryManager<ObjType, SingletonType> : ScriptableSingleton<SingletonType>
    where ObjType : class
    where SingletonType : ScriptableObject
{
    private readonly Dictionary<SubGame, ObjType> objects = new();

    public virtual ObjType this[SubGame subGame]
        => objects.ContainsKey(subGame) ? objects[subGame] : null;

    public virtual void Register(SubGame subGame, ObjType obj)
    {
        if (!objects.ContainsKey(subGame) && obj != null)
            objects[subGame] = obj;
    }
}
