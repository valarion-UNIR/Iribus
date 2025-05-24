using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SubGameInteractable))]
public class SubGameScreen : MonoBehaviour
{
    private SubGameInteractable interactable;
    private SceneAsset sceneAsset;
    private MeshRenderer meshRenderer;
    private Scene scene;
    private Camera sceneCamera;
    private RenderTexture renderTexture;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactable = GetComponent<SubGameInteractable>();

        meshRenderer = GetComponentsInChildren<MeshRenderer>().Where(c => c.CompareTag("Screen")).FirstOrDefault();
        if (meshRenderer == null)
        {
            Debug.LogError("No screen found in hierarchy");
            return;
        }

        meshRenderer.material.mainTexture = renderTexture;
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        sceneAsset = SubGameDataManager.Instance.Data[interactable.SubGame].MainScene;
        var operation = SceneManager.LoadSceneAsync(sceneAsset.name, LoadSceneMode.Additive);
    }

    private void SceneManager_sceneLoaded(Scene loadedScene, LoadSceneMode mode)
    {
        if(loadedScene.name == sceneAsset.name)
        {
            scene = loadedScene;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
            InitializeFromScene();
        }
    }

    private void InitializeFromScene()
    {
        sceneCamera = GetSceneComponents<Camera>().Where(c => c.CompareTag("MainCamera")).First();
        renderTexture = new RenderTexture(sceneCamera.pixelWidth, sceneCamera.pixelHeight, 32, UnityEngine.Experimental.Rendering.DefaultFormat.HDR);
        sceneCamera.targetTexture = renderTexture;
        meshRenderer.material = new Material(meshRenderer.material) { mainTexture = renderTexture };
    }

    private IEnumerable<T> GetSceneComponents<T>(bool includeInactive = true)
        => scene.GetRootGameObjects().SelectMany(c => c.GetComponents<T>().Concat(c.GetComponentsInChildren<T>(includeInactive)));
}
