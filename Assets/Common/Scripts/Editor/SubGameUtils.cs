using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class SubGameUtils
{
    public static (string[] SceneAssets, Dictionary<string, SubGame> ReverseLookup) GetSceneSubGames()
    {
        var assets = AssetDatabase.FindAssets($"t:{nameof(SceneAsset)}", new[] { "Assets" });
        var subgameBundles = AssetDatabase.GetAllAssetBundleNames()
                    .Select(n => Enum.TryParse<SubGame>(n, true, out var value) ? value : SubGame.None)
                    .Except(new[] { SubGame.None })
                    .ToArray();

        var lookup = subgameBundles
            .ToDictionary(s => s, s => AssetDatabase.GetAssetPathsFromAssetBundle(s.ToString().ToLower()));

        var reverseLookup = lookup
            .SelectMany(pair => pair.Value.Select(value => (pair.Key, Value: value)))
            .ToDictionary(pair => pair.Value, pair => pair.Key);

        return (assets, reverseLookup);
    }

    public static ReadOnlyDictionary<SubGame, SubGameData> GetSubGameData()
    {
        var dataAssets = AssetDatabase.FindAssets($"t:{nameof(SubGameDataManager)}", new[] { "Assets" });
        if (dataAssets.Length < 1)
        {
            Debug.LogError($"Asset of type \"{nameof(SubGameDataManager)}\" missing for this action.");
            return null;
        }
        else if (dataAssets.Length > 1)
            Debug.LogError($"More than one asset of type \"{nameof(SubGameDataManager)}\" exists, using the first one found.");

        var manager = AssetDatabase.LoadAssetAtPath<SubGameDataManager>(AssetDatabase.GUIDToAssetPath(dataAssets[0]));

        var subgamesdata = manager.Data;

        return subgamesdata;
    }
}