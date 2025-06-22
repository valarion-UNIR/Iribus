using System.Linq;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Class that makes that when creating a new game object or a new component on a game  object, assigns the layer and masks corresponding to the subgame.
/// </summary>
[InitializeOnLoad]
public class SubGameComponentAdded
{
    static SubGameComponentAdded()
    {
        ObjectFactory.componentWasAdded += ObjectFactory_componentWasAdded;
        ObjectChangeEvents.changesPublished += ObjectChangeEvents_changesPublished;
    }

    private static void ObjectChangeEvents_changesPublished(ref ObjectChangeEventStream stream)
    {
        for (int i = 0; i < stream.length; ++i)
        {
            var type = stream.GetEventType(i);
            if (type != ObjectChangeKind.CreateGameObjectHierarchy)
                continue;

            stream.GetCreateGameObjectHierarchyEvent(i, out var createGameObjectHierarchyEvent);
            var obj = EditorUtility.InstanceIDToObject(createGameObjectHierarchyEvent.instanceId) as GameObject;
            foreach (var child in new[] { obj.transform }.Concat(obj.GetComponentsInChildren<Transform>()))
            {
                foreach (var component in child.gameObject.GetComponents<Component>())
                {
                    ObjectFactory_componentWasAdded(component);
                }
            }
            break;

        }
    }

    private static void ObjectFactory_componentWasAdded(Component obj)
    {
        // Check that scene asset is assigned to a subgame
        var (sceneAssets, sceneAssetSubgames) = SubGameUtils.GetSceneSubGames();
        if (!sceneAssetSubgames.ContainsKey(obj.gameObject.scene.path))
        {
            Debug.LogError($"Scene \"{obj.gameObject.scene.path}\" is not contained in a sub game asset bundle. Please select the scene in the asset manager and select the appropiate asset bundle.");
            return;
        }
        var subgame = sceneAssetSubgames[obj.gameObject.scene.path];

        // Check that the subgame has data corresponding to it
        var subgamesdata = SubGameUtils.GetSubGameData();
        if (!subgamesdata.ContainsKey(subgame))
        {
            Debug.LogError($"There is no SubGameData definition for SubGame \"{subgame}\", please create it.");
            return;
        }
        var data = subgamesdata[subgame];

        // Assign the different properties acording to component type
        switch (obj)
        {
            case Transform transform: // New game object
                transform.gameObject.layer = data.DefaultLayer;
                break;

            case Camera camera:
                camera.cullingMask = data.CullingMask;
                var cameradata = camera.GetComponent<UniversalAdditionalCameraData>();
                if (cameradata == null)
                    camera.AddComponent<UniversalAdditionalCameraData>();
                break;

            case Light light:
                light.cullingMask = data.CullingMask;
                var lightdata = light.GetComponent<UniversalAdditionalLightData>();
                if (lightdata == null)
                    light.AddComponent<UniversalAdditionalLightData>();
                break;

            case UniversalAdditionalLightData light:
                light.renderingLayers = data.RenderingLayerMask;
                break;

            case UniversalAdditionalCameraData camera:
                camera.volumeLayerMask = data.CullingMask;
                break;

            case CinemachineCamera camera:
                camera.OutputChannel = data.CinemachineChannel;
                break;

            case CinemachineBrain camera:
                camera.ChannelMask = data.CinemachineChannel;
                break;

            case Renderer renderer:
                renderer.renderingLayerMask = data.RenderingLayerMask;
                break;
        }
    }
}