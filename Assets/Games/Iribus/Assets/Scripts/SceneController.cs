using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Eflatun.SceneReference;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SceneController : MonoBehaviour
{
    [SerializeField] private Animator transicionAnimator;
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
        //transicionAnimator = GetComponent<Animator>();
    }

    public void CargarEscenaMetroidvania(int escena, int transicion, bool muerte)
    {
        //PlayAnimacionTransicion(transicion);
        StartCoroutine(CargarEscenaTransicion(escena, muerte));
    }

    private IEnumerator CargarEscenaTransicion(int escena, bool muerte)
    {
        //yield return new WaitForSeconds(transicionAnimator.GetCurrentAnimatorStateInfo(0).length);

        Task task = TransicionTP("TransicionIn", 0);
        yield return new WaitUntil(() => task.IsCompleted);

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

        task = TransicionTP("TransicionOut", 0);
        yield return new WaitUntil(() => task.IsCompleted);
    }

    private void PlayAnimacionTransicion(int transicion)
    {
        Debug.Log("Transicion " + transicion);
    }

    public async Task TransicionTP(string transitionType, int transicionID)
    {
        transicionAnimator.SetInteger(transitionType, transicionID);
        Debug.Log("Hola");
        if(transitionType.Equals("TransicionIn"))
        {
            await WaitForAnimationToEnd("Terminado");
        }
        else
        {
            await WaitForAnimationToEnd("Empezado");
        }
        transicionAnimator.SetInteger(transitionType, -1);
        Debug.Log("Hola2");
    }
    async Task WaitForAnimationToEnd(string tipo)
    {

        while (!transicionAnimator.GetCurrentAnimatorStateInfo(0).IsName(tipo))
        {
            await Task.Yield();
        }
    }
}
