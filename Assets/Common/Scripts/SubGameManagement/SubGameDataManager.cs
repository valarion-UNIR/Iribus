using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "SubGameData", menuName = "ScriptableObjects/SubGameData", order = 1)]
public class SubGameDataManager : ScriptableObject
{
    [SerializeField] private List<SubGameData> data = new();

    public ReadOnlyDictionary<SubGame, SubGameData> Data => new(data.ToDictionary(data => data.SubGame, data => data));
}

[Serializable]
public class SubGameData
{
    [SerializeField] private SubGame subGame;
    [SerializeField] private SceneAsset mainScene;
    [SerializeField, Layer] private int defaultLayer;
    [SerializeField] private LayerMask cullingMask;
    [SerializeField] private RenderingLayerMask renderingLayerMask;

    public SubGame SubGame => subGame;
    public SceneAsset MainScene => mainScene;
    public int DefaultLayer => defaultLayer;
    public LayerMask CullingMask => cullingMask;
    public RenderingLayerMask RenderingLayerMask => renderingLayerMask;
}