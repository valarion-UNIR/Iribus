using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class AbstractSubGameObjectCreationManager<ObjType, SingletonType> : ScriptableSingleton<SingletonType>
    where ObjType : class
    where SingletonType : ScriptableObject
{
    protected readonly Dictionary<SubGame, ObjType> objects = new();

    public virtual ObjType this[SubGame subGame]
        => objects.ContainsKey(subGame) ? objects[subGame] : (objects[subGame] = CreateObject(subGame));

    protected abstract ObjType CreateObject(SubGame subgame);
}