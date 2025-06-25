using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Eflatun.SceneReference;
using System.Collections.Generic;

public class SceneController : MonoBehaviour
{
    private Animator transicionAnimator;
    [SerializeField] private List<SceneReference> sceneReferences = new List<SceneReference>();

    public static SceneController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        transicionAnimator = GetComponent<Animator>();
    }

    public void CargarEscenaMetroidvania(int escena, int transicion, bool muerte)
    {
        PlayAnimacionTransicion(transicion);
        StartCoroutine(CargarEscenaTransicion(escena, muerte));
    }

    private IEnumerator CargarEscenaTransicion(int escena, bool muerte)
    {
        //yield return new WaitForSeconds(transicionAnimator.GetCurrentAnimatorStateInfo(0).length);

        Scene oldScene = SceneManager.GetActiveScene();

        if (muerte) 
        {
            yield return new WaitForSeconds(1.5f);
        }

        Awaitable sceneLoading =  SubGameSceneManager.LoadScene(SubGame.Iribus, sceneReferences[escena], null, LocalPhysicsMode.None);

        while(!sceneLoading.IsCompleted)
        {
            yield return null;
        }

        //Scene newScene = SceneManager.GetSceneByName(sceneReferences[escena].Name);
        //SceneManager.SetActiveScene(newScene);

        //AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(oldScene);
        //while (!unloadOp.isDone)
        //    yield return null;

        GameManagerMetroidvania.Instance.LoadPlayerOnScene();
    }

    private void PlayAnimacionTransicion(int transicion)
    {
        Debug.Log("Transicion " + transicion);
    }
}
