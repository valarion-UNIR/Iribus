using Eflatun.SceneReference;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SubGameInteractable))]
public class SubGameScreen : MonoBehaviour
{
    private SubGameInteractable interactable;
    private AudioSource audioSourceTemplate;

    private SceneReference sceneReference;
    private Awaitable sceneLoading;
    private MeshRenderer meshRenderer;
    private Material material;
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
        sceneReference = SubGameDataManagerSingleton.Data[interactable.SubGame].MainScene;
        LoadScene(sceneReference, physics: LocalPhysicsMode.Physics2D | LocalPhysicsMode.Physics3D);
    }

    public void LoadScene(SceneReference newScene, SceneReference loadingScene = null, LocalPhysicsMode physics = LocalPhysicsMode.None)
        => sceneLoading = LoadSceneInner(newScene, loadingScene, physics, sceneLoading);

    private async Awaitable LoadSceneInner(SceneReference newScene, SceneReference loadingScene, LocalPhysicsMode physics, Awaitable currentSceneLoading)
    {
        if (currentSceneLoading != null)
            await sceneLoading;

        // Ensure running in main thread
        await Awaitable.MainThreadAsync();

        var previousScene = scene;

        // Load "loading" scene if it exists
        if (loadingScene != null)
        {
            await SceneManager.LoadSceneAsync(loadingScene.Name, new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.Physics2D | LocalPhysicsMode.Physics3D));
            sceneReference = loadingScene;
            scene = newScene.LoadedScene;
            InitializeFromScene();
        }

        // Unload previous scene if it exists
        if (!string.IsNullOrWhiteSpace(previousScene.name))
            await SceneManager.UnloadSceneAsync(previousScene);

        // Load new scene
        await SceneManager.LoadSceneAsync(newScene.Name, new LoadSceneParameters(LoadSceneMode.Additive, physics));

        // Unload "loading" scene if it exists
        if (loadingScene != null)
            await SceneManager.UnloadSceneAsync(loadingScene.LoadedScene);

        // Initialize 
        sceneReference = newScene;
        scene = newScene.LoadedScene;
        InitializeFromScene();

        sceneLoading = null;
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
        if (renderTexture != null)
            renderTexture.Release();
        if (material == null)
            material = meshRenderer.material;
        renderTexture = new RenderTexture(sceneCamera.pixelWidth, sceneCamera.pixelHeight, 32, UnityEngine.Experimental.Rendering.DefaultFormat.HDR);
        meshRenderer.material = new Material(material) { mainTexture = renderTexture };

        sceneCamera.targetTexture = renderTexture;

        foreach (var listener in GetSceneComponents<AudioListener>())
            listener.enabled = false;
    }

    private IEnumerable<T> GetSceneComponents<T>(bool includeInactive = true)
        => scene.GetRootGameObjects().SelectMany(c => c.GetComponents<T>().Concat(c.GetComponentsInChildren<T>(includeInactive)));
}
