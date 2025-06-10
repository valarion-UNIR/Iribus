using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Eflatun.SceneReference;

public class SceneController : MonoBehaviour
{
    private Animator transicionAnimator;
    [SerializeField] private SceneReference sceneReference;

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
        if (muerte) 
        {
            yield return new WaitForSeconds(1.5f);
        }
        SubGameSceneManager.LoadScene(SubGame.Iribus, sceneReference, null, LocalPhysicsMode.None);
        //SceneManager.LoadScene(escena);
    }

    private void PlayAnimacionTransicion(int transicion)
    {
        Debug.Log("Transicion " + transicion);
    }
}
