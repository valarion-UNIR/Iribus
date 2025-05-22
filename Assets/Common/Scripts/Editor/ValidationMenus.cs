using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer.Explorer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class ValidationMenus : MonoBehaviour
{

    [MenuItem("Iribus Validation/Validate opened scenes", priority = 1)]
    static void ValidateOpenedScenes()
        => ProcessScenes(true, true);

    [MenuItem("Iribus Validation/Validate all scenes", priority = 2)]
    static void ValidateAllScenes()
        => ProcessScenes(true, false);

    [MenuItem("Iribus Validation/Process opened scenes", priority = 101)]
    static void ProcessOpenedScenes()
        => ProcessScenes(false, true);

    [MenuItem("Iribus Validation/Process all scenes", priority = 102)]
    static void ProcessAllScenes()
        => ProcessScenes(false, false);

    static void ProcessScenes(bool dryRun, bool onlyOpened)
    {
        var loadedScenes = Enumerable.Range(0, SceneManager.sceneCount).Select(i => SceneManager.GetSceneAt(i)).ToArray();
        var loadedScenePaths = loadedScenes.Select(s => s.path).ToArray();

        var (sceneAssets, sceneAssetSubgames) = SubGameUtils.GetSceneSubGames();
        var subgamesdata = SubGameUtils.GetSubGameData();

        foreach (var asset in sceneAssets)
        {
            var path = AssetDatabase.GUIDToAssetPath(asset);
            var scene = loadedScenes.FirstOrDefault(s => s.path == path);
            var wasSceneOpened = scene.path == path;

            if (onlyOpened && !wasSceneOpened)
                continue;

            if (!sceneAssetSubgames.ContainsKey(path))
            {
                Debug.LogError($"Scene \"{path}\" is not contained in a sub game asset bundle. Please select the scene in the asset manager and select the appropiate asset bundle.");
                continue;
            }
            var subgame = sceneAssetSubgames[path];

            if (!subgamesdata.ContainsKey(subgame))
            {
                Debug.LogError($"There is no SubGameData definition for SubGame \"{subgame}\", please create it.");
                continue;
            }

            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);

            if (!wasSceneOpened)
            {
                if (onlyOpened)
                    continue;

                scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            }

            var changedSomething = ProcessScene(sceneAssetSubgames[path], scene, subgamesdata[subgame], dryRun);

            if(!changedSomething)
                Debug.Log($"Processed \"{scene.name}\" for subgame {subgame} and everything was correct");

            if (!wasSceneOpened && (dryRun || !changedSomething))
                EditorSceneManager.CloseScene(scene, true);
        }
    }


    private static bool ProcessScene(SubGame subgame, Scene scene, SubGameData data, bool dryRun)
    {
        Debug.Log($"Processing \"{scene.name}\" for subgame {subgame}");
        bool changedSomething = false;
        var subGameDefinition = GetSceneComponents<SubGame>(scene);
        foreach (var transform in scene.GetRootGameObjects().SelectMany(o => o.GetComponentsInChildren<Transform>()))
        {
            var gameObject = transform.gameObject;

            ProcessGameObject(subgame, data, dryRun, ref changedSomething, gameObject);

            if (changedSomething)
                EditorSceneManager.MarkSceneDirty(scene);
        }

        return changedSomething;
    }

    private static void ProcessGameObject(SubGame subgame, SubGameData data, bool dryRun, ref bool changedSomething, GameObject gameObject)
    {
        Process(gameObject.layer == 0, dryRun, ref changedSomething, gameObject,
                        $"{GetPrefix(gameObject)} is using the default layer instead of the {subgame} default layer \"{LayerMask.LayerToName(data.DefaultLayer)}\"",
                        $"{GetPrefix(gameObject)} was using the default layer, changed to {subgame} default layer \"{LayerMask.LayerToName(data.DefaultLayer)}\"",
                        () => gameObject.layer = data.DefaultLayer
                        );
        var singlelayermask = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));
        Process(gameObject.layer > 0 && (singlelayermask & data.CullingMask) != singlelayermask, dryRun, ref changedSomething, gameObject,
                        $"{GetPrefix(gameObject)} is using a layer outside of the {subgame} default layer mask",
                        $"{GetPrefix(gameObject)} was using  a layer outside of the {subgame} default layer mask, changed to {subgame} default layer \"{LayerMask.LayerToName(data.DefaultLayer)}\"",
                        () => gameObject.layer = data.DefaultLayer
                        );

        foreach (var camera in gameObject.GetComponents<Camera>())
        {
            Process((camera.cullingMask & data.CullingMask) != camera.cullingMask, dryRun, ref changedSomething, gameObject,
                $"{GetPrefix(camera)} is using a non standard culling mask for {subgame}",
                $"{GetPrefix(camera)} was using a non standard default layer, changed to {subgame} default layer",
                () => camera.cullingMask = (camera.cullingMask & data.CullingMask) > 0 ? (camera.cullingMask & data.CullingMask) : data.CullingMask
                );
        }

        foreach (var light in gameObject.GetComponents<Light>())
        {
            Process((light.cullingMask & data.CullingMask) != light.cullingMask, dryRun, ref changedSomething, gameObject,
                $"{GetPrefix(light)} is using a non standard culling mask for {subgame}",
                $"{GetPrefix(light)} was using a non standard culling mask, constrained to {subgame} culling mask",
                () => light.cullingMask = (light.cullingMask & data.CullingMask) > 0 ? (light.cullingMask & data.CullingMask) : data.CullingMask
                );
        }

        foreach (var light in gameObject.GetComponents<UniversalAdditionalLightData>())
        {
            Process((light.renderingLayers & data.RenderingLayerMask.value) != light.renderingLayers, dryRun, ref changedSomething, gameObject,
                $"{GetPrefix(light)} is using a non standard rendering layer mask for {subgame}",
                $"{GetPrefix(light)} was using a non standard rendering layer mask, constrained to {subgame} rendering layer mask",
                () => light.renderingLayers = (light.renderingLayers & data.RenderingLayerMask.value) > 0 ? (light.renderingLayers & data.RenderingLayerMask.value) : data.RenderingLayerMask.value
                );
        }

        foreach (var renderer in gameObject.GetComponents<Renderer>())
        {
            Process((renderer.renderingLayerMask & data.RenderingLayerMask) != renderer.renderingLayerMask, dryRun, ref changedSomething, gameObject,
                $"{GetPrefix(renderer)} is using a non standard rendering layer mask for {subgame}",
                $"{GetPrefix(renderer)} was using a non standard rendering layer mask, constrained to {subgame} rendering layer mask",
                () => renderer.renderingLayerMask = (renderer.renderingLayerMask & data.RenderingLayerMask) > 0 ? (renderer.renderingLayerMask & data.RenderingLayerMask) : data.RenderingLayerMask
                );
        }
    }

    //private static void ProcessMask(ref uint value, int mask, bool isRenderingLayer, bool dryRun, ref bool changedSomething, Component context, SubGame subgame)
    //{
    //    var isValueInMask = (value & mask) == value;
    //    var prefix = $"Scene {context.gameObject.scene.name} object {context.gameObject.name} {context.GetType().Name} {context.name}";
    //    var masktype = isRenderingLayer ? "rendering layer" : "culling";
    //    var 
    //    Process(isValueInMask, dryRun, ref changedSomething, context,
    //        $"{prefix} is using a non standard {masktype} mask for {subgame}",
    //        $"{prefix} was using a non standard rendering layer mask, constrained to {subgame} rendering layer mask",
    //        () => value = (value & mask > 0) ? 
    //        );
    //}

    private static void Process(bool condition, bool dryRun, ref bool changedSomething, UnityEngine.Object context, string dryErrorMessage, string notDryLogMessage, Action notDryAction)
    {
        if(dryRun)
        {
            if (condition)
                Debug.LogError(dryErrorMessage, context);
        }
        else
        {
            if (condition)
            {
                notDryAction();
                changedSomething = true;
                Debug.Log(notDryLogMessage, context);
            }
        }
    }

    private static string GetPrefix(GameObject obj)
        => $"Scene \"{obj.scene.name}\" GameObject \"{obj.name}\":";

    private static string GetPrefix(Component comp)
        => $"{GetPrefix(comp.gameObject)} {comp.GetType().Name} \"{comp.name}\":";

    private static IEnumerable<T> GetSceneComponents<T>(Scene scene, bool includeInactive = true)
        => scene.GetRootGameObjects().SelectMany(c => c.GetComponents<T>().Concat(c.GetComponentsInChildren<T>(includeInactive)));
}
