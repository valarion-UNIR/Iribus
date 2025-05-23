using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubGameScreen : MonoBehaviour
{
    [SerializeField] private SubGame subGame;
    
    private SceneAsset sceneAsset;
    private MeshRenderer meshRenderer;
    private Scene scene;
    private Camera sceneCamera;
    private RenderTexture renderTexture;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshRenderer = GetComponentsInChildren<MeshRenderer>().Where(c => c.CompareTag("Screen")).FirstOrDefault();
        if (meshRenderer == null)
            Debug.LogWarning("No screen found in hierarchy");
        meshRenderer.material.mainTexture = renderTexture;
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        sceneAsset = GetAllInstances<SubGameDataManager>().First().Data[subGame].MainScene;
        var operation = SceneManager.LoadSceneAsync(sceneAsset.name, LoadSceneMode.Additive);
    }

    public static IEnumerable<T> GetAllInstances<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets("t:"+typeof(T).Name); //FindAssets uses tags check documentation for more info
        foreach(var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            yield return AssetDatabase.LoadAssetAtPath<T>(path);
        }
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
