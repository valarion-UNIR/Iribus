using Eflatun.SceneReference;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SubGameInteractable))]
public class SubGameScreen : MonoBehaviour
{
    private SubGameInteractable interactable;
    private AudioSource audioSourceTemplate;

    private SceneReference sceneReference;
    private MeshRenderer meshRenderer;
    private Scene scene;
    private Camera sceneCamera;
    private RenderTexture renderTexture;

    public AudioSource AudioSourceTemplate => audioSourceTemplate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactable = GetComponent<SubGameInteractable>();
        SubGameScreenManager.instance.Register(interactable.SubGame, this);

        audioSourceTemplate = GetComponentsInChildren<AudioSource>().Where(c => c.CompareTag("Speakers")).FirstOrDefault();
        if(audioSourceTemplate == null)
        {
            Debug.LogWarning("No speakers found in hierarchy, creating one");
            var speakers = new GameObject("Speakers");
            speakers.transform.SetParent(transform, false);
            audioSourceTemplate = speakers.AddComponent<AudioSource>();
        }

        meshRenderer = GetComponentsInChildren<MeshRenderer>().Where(c => c.CompareTag(TagsIribus.Screen)).FirstOrDefault();
        if (meshRenderer == null)
        {
            Debug.LogError("No screen found in hierarchy");
            return;
        }

        meshRenderer.material.mainTexture = renderTexture;
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        sceneReference = SubGameDataManagerSingleton.Data[interactable.SubGame].MainScene;
        var operation = SceneManager.LoadSceneAsync(sceneReference.Name, LoadSceneMode.Additive);
    }

    private void SceneManager_sceneLoaded(Scene loadedScene, LoadSceneMode mode)
    {
        if(loadedScene.name == sceneReference.Name)
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

        foreach (var listener in GetSceneComponents<AudioListener>())
            listener.enabled = false;
    }

    private IEnumerable<T> GetSceneComponents<T>(bool includeInactive = true)
        => scene.GetRootGameObjects().SelectMany(c => c.GetComponents<T>().Concat(c.GetComponentsInChildren<T>(includeInactive)));
}
