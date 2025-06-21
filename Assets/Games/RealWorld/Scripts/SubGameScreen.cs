using Eflatun.SceneReference;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SubGameInteractable))]
public class SubGameScreen : MonoBehaviour
{
    private static Awaitable sceneLoadingStatic;
    private Awaitable sceneLoading;

    // Scene properties
    private SceneReference sceneReference;
    private Scene scene;

    // SubGame GameObjects
    private Camera sceneCamera;

    // Real world GameObjects
    private AudioSource audioSourceTemplate;
    private MeshRenderer meshRenderer;
    private SubGameInteractable interactable;
    private RenderTexture renderTexture;
    private Material originalMaterial;
    
    // Properties for external access
    public AudioSource AudioSourceTemplate => audioSourceTemplate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactable = GetComponent<SubGameInteractable>();
        SubGameScreenManager.instance.Register(interactable.SubGame, this);

        // Speakers search
        audioSourceTemplate = GetComponentsInChildren<AudioSource>().Where(c => c.CompareTag("Speakers")).FirstOrDefault();
        if(audioSourceTemplate == null)
        {
            if (interactable.SubGame != SubGame.None)
                Debug.LogWarning("No speakers found in hierarchy, creating one");
            var speakers = new GameObject("Speakers");
            speakers.transform.SetParent(transform, false);
            audioSourceTemplate = speakers.AddComponent<AudioSource>();
        }

        // Renderer search
        meshRenderer = GetComponentsInChildren<MeshRenderer>().Where(c => c.CompareTag(TagsIribus.Screen)).FirstOrDefault();
        if (meshRenderer == null)
        {
            if(interactable.SubGame != SubGame.None)
                Debug.LogError("No screen found in hierarchy");
            return;
        }

        var sceneReference = SubGameDataManagerSingleton.Data[interactable.SubGame].MainScene;
        LoadScene(sceneReference, physics: LocalPhysicsMode.Physics2D | LocalPhysicsMode.Physics3D);
    }

    public Awaitable LoadScene(SceneReference newScene, SceneReference loadingScene = null, LocalPhysicsMode physics = LocalPhysicsMode.None)
        => sceneLoading = LoadSceneInner(newScene, loadingScene, physics, sceneLoading, this);

    public static Awaitable LoadSceneStatic(SceneReference newScene, SceneReference loadingScene = null, LocalPhysicsMode physics = LocalPhysicsMode.None)
        => sceneLoadingStatic = LoadSceneInner(newScene, loadingScene, physics, sceneLoadingStatic, null);

    private static async Awaitable LoadSceneInner(SceneReference newScene, SceneReference loadingScene, LocalPhysicsMode physics, Awaitable currentSceneLoading, SubGameScreen screen)
    {
        // Wait for last scene load
        if (currentSceneLoading != null)
            await currentSceneLoading;

        // Ensure running in main thread
        await Awaitable.MainThreadAsync();

        var previousScene = screen != null ? (Scene?)screen.scene : SceneManager.GetActiveScene();

        // Load "loading" scene if it exists
        if (loadingScene != null)
        {
            await SceneManager.LoadSceneAsync(loadingScene.Name, new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.Physics2D | LocalPhysicsMode.Physics3D));
            if(screen != null)
                screen.InitializeFromScene(loadingScene, false);
        }

        // Unload previous scene if it exists
        if (!string.IsNullOrWhiteSpace(previousScene?.name))
            await SceneManager.UnloadSceneAsync(previousScene.Value);

        // Load new scene
        await SceneManager.LoadSceneAsync(newScene.Name, new LoadSceneParameters(LoadSceneMode.Additive, physics));

        // Unload "loading" scene if it exists
        if (loadingScene != null)
            await SceneManager.UnloadSceneAsync(loadingScene.LoadedScene);

        // Initialize 
        if (screen != null)
            screen.InitializeFromScene(newScene, true);
    }

    private void InitializeFromScene(SceneReference newScene, bool resetSceneLoading)
    {
        // Set properties
        sceneReference = newScene;
        scene = newScene.LoadedScene;

        // Find scene camera
        sceneCamera = scene.GetSceneComponents<Camera>().Where(c => c.CompareTag("MainCamera")).First();
        sceneCamera.TryGetComponent<PixelPerfectCamera>(out var pixelCamera);

        // Create render texture from camera and assign it
        if (renderTexture != null)
            renderTexture.Release();
        if (pixelCamera != null)
            sceneCamera.targetTexture = renderTexture = new RenderTexture(pixelCamera.refResolutionX, pixelCamera.refResolutionY, 32, UnityEngine.Experimental.Rendering.DefaultFormat.HDR);
        else
            sceneCamera.targetTexture = renderTexture = new RenderTexture(sceneCamera.pixelWidth, sceneCamera.pixelHeight, 32, UnityEngine.Experimental.Rendering.DefaultFormat.HDR);

        // Create material from render texture and assign it to MeshRender
        if (originalMaterial == null)
            originalMaterial = meshRenderer.material;
        meshRenderer.material = new Material(originalMaterial) { mainTexture = renderTexture };

        // Disable scene audio listeners
        foreach (var listener in scene.GetSceneComponents<AudioListener>())
            listener.enabled = false;

        // Reset the scene loading variable if needed
        if (resetSceneLoading)
            sceneLoading = null;
    }
}
