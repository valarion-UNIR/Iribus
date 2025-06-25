using System.Collections;
using System.IO;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Menu, Loading, Playing, Paused, Inventory, Cutscene, GameOver }
public class GameManagerMetroidvania : MonoBehaviour
{
    public static GameManagerMetroidvania Instance;
    private ProgresoMetroidvania progreso;

    private SceneController sceneController;

    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private TextMeshProUGUI textoState;
    [SerializeField] private TextMeshProUGUI textoSave;

    [SerializeField] private ParticleSpawner particleSpawner;
    [SerializeField] private TextoEscribir dialogoPrueba;

    [SerializeField] private GameObject dialogoCaja;
    [SerializeField] private TextoEscribir dialogoTexto;

    [SerializeField] private DialogueManager dialogueManager;

    [SerializeField] private Canvas uIEstados;
    [SerializeField] private Canvas uIDialogos;

    private Vector2 velPlayer;

    private GameObject playerInstance;

    private CheckPointManager checkPManager;
    private CinemachineCamera currentActiveCamera;

    private CinemachineCamera dialogoCamera;
    private CinemachineCamera onHoldCamera;

    private int entradaUsada;

    public GameState CurrentState { get; private set; }
    void Awake()
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
        sceneController = SceneController.Instance;
        ChangeState(GameState.Menu);
        CargarProgreso();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            BorrarProgreso();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            CheckGuardado();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            GetComponent<DialogueManager>().SetLanguage("ja");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log(GetComponent<DialogueManager>().GetDialogue("Hello wellers"));
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            dialogoPrueba.EscribirDialogo();
        }
    }

    public void EmpezarPartida()
    {
        if (CurrentState != GameState.Menu) return;
        ChangeState(GameState.Loading);
        if(!CargarProgreso())
        {
            // ACTIVAR CINEMATICA DE INICIO
            Debug.Log("No existe guardado");
            progreso = new ProgresoMetroidvania(1, 0);
            GuardarProgreso();

            sceneController.CargarEscenaMetroidvania(progreso.escena, 0, false);
        }
        else
        {
            entradaUsada = progreso.checkpoint;
            sceneController.CargarEscenaMetroidvania(progreso.escena, 0, false);
            Debug.Log("Existe guardado");
        }
    }

    private void CheckGuardado()
    {
        var path = Path.Combine(Application.persistentDataPath, "metroidvaniaSave.json");
        if (File.Exists(path))
        {
            textoSave.text = "Guardado: " + progreso.escena + "/" + progreso.checkpoint;
        }
        else
        {
            textoSave.text = "Guardado: Vacío";
        }
    }

    private void BorrarProgreso()
    {
        var path = Path.Combine(Application.persistentDataPath, "metroidvaniaSave.json");
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        CheckGuardado();
    }

    private void GuardarProgreso()
    {
        var json = JsonUtility.ToJson(progreso);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "metroidvaniaSave.json"), json);
        CheckGuardado();
    }
    private bool CargarProgreso()
    {
        var path = Path.Combine(Application.persistentDataPath, "metroidvaniaSave.json");
        if (File.Exists(path))
        {
            progreso = JsonUtility.FromJson<ProgresoMetroidvania>(File.ReadAllText(path));
            CheckGuardado();
            return true;
        }
        else
        {
            CheckGuardado();
            return false;
        }
    }


    public void ChangeState(GameState newState)
    {
        ExitState(CurrentState);
        EnterState(newState);
        CurrentState = newState;
        textoState.text = "Estado: " + CurrentState.ToString();
    }

    public void PlayerMuerte()
    {
        if (CurrentState == GameState.GameOver) return;
        ChangeState(GameState.GameOver);
        playerInstance = null;

        sceneController.CargarEscenaMetroidvania(progreso.escena, 0, true);
    }


    public void CargarSiguienteEscena(int escenaSiguiente, int entrada)
    {
        ChangeState(GameState.Loading);
        velPlayer = playerInstance.GetComponent<Rigidbody2D>().linearVelocity;
        playerInstance = null;
        entradaUsada = entrada;

        sceneController.CargarEscenaMetroidvania(escenaSiguiente, 0, false);
    }

    public void SpawnPlayer(Transform spawnTransform)
    {
        playerInstance = Instantiate(playerPrefab, spawnTransform.position, Quaternion.identity, spawnTransform);
        playerInstance.transform.parent = null;
        if (currentActiveCamera.Follow != null)
        {
            Debug.Log("Spawn player y seguir");
            currentActiveCamera.Follow = playerInstance.transform;
        }

        if (velPlayer != null)
        {
            playerInstance.GetComponent<Rigidbody2D>().linearVelocity = velPlayer;
        }
        ChangeState(GameState.Playing);
    }

    void EnterState(GameState state)
    {
        switch(state)
        {
            case GameState.Menu:
                if(playerInstance != null)
                {
                    playerInstance.GetComponent<PlayerMovement>().BlockMovement(true);
                }
                break;
            case GameState.Playing:
                if (playerInstance != null)
                {
                    playerInstance.GetComponent<PlayerMovement>().BlockMovement(false);
                }
                break;
        }
    }
    void ExitState(GameState state)
    {

    }

    private IEnumerator AnimacionSpawn(CheckPoint checkPoint)
    {
        Debug.Log("AnimacionSpawn");
        checkPoint.SpawnearParticulasCerrar();
        yield return new WaitForSeconds(0.5f);
        SpawnPlayer(checkPoint.transform);
        checkPoint.SpawnearParticulasAbrir();
    }

    public int GetCheckpointProgreso()
    {
        return progreso.checkpoint;
    }

    public void LoadPlayerOnScene()
    {
        checkPManager = FindAnyObjectByType<CheckPointManager>();

        if(checkPManager != null)
        {
            uIEstados.worldCamera = checkPManager.GetSceneCamera();
            uIDialogos.worldCamera = checkPManager.GetSceneCamera();
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        if(CurrentState == GameState.GameOver)
        {
            CheckPoint checkPoint = checkPManager.GetCheckPoint(progreso.checkpoint);
            currentActiveCamera = checkPoint.GetCheckPointCamera();
            currentActiveCamera.enabled = true;
            StartCoroutine(AnimacionSpawn(checkPoint));
        }
        else if (CurrentState == GameState.Loading)
        {
            CheckPoint checkPoint = checkPManager.GetEntrada(entradaUsada);
            currentActiveCamera = checkPoint.GetCheckPointCamera();
            currentActiveCamera.enabled = true;
            SpawnPlayer(checkPoint.transform);
        }
    }

    public Transform GetPlayerTransform()
    {
        return playerInstance.transform;
    }

    public ParticleSpawner GetParticleSpawner()
    {
        return particleSpawner;
    }

    public DialogueManager GetDialogueManager()
    {
        return dialogueManager;
    }

    public void StartDialogue(DialogoConcreto dialogo, CinemachineCamera dialogoCamera)
    {
        if(CurrentState != GameState.Playing)
        {
            Debug.Log("No se puede iniciar dialogo");
            return;
        }

        ChangeState(GameState.Menu);

        this.dialogoCamera = dialogoCamera;

        onHoldCamera = checkPManager.GetActiveCMCamera();
        onHoldCamera.enabled = false;
        dialogoCamera.enabled = true;

        dialogoCaja.SetActive(true);
        dialogoTexto.SetDialogoEscribir(dialogo);
        dialogoTexto.EscribirDialogo();
    }

    public void EndDialogue()
    {
        if (CurrentState != GameState.Menu)
        {
            Debug.Log("No se puede terminar dialogo");
            return;
        }

        dialogoCaja.SetActive(false);

        dialogoCamera.enabled = false;
        onHoldCamera.enabled = true;

        ChangeState(GameState.Playing);
    }
}