using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BeaverManager : MonoBehaviour
{
    public static BeaverManager Instance { get; private set; }
    //public static event Action CameraGoUp; 
    [SerializeField] private BeaverCameraController cameraController;
    //Luego cambiar esto para ajustarse a los planos pero en principio asi
    private float cameraDistance = 10 * 2;
    [Header("Levels")]
    [SerializeField] private GameObject player;
    public int currentLevelIndex = 0;
    [SerializeField] private List<GameObject> levels;
    private GameObject currentLevel;

    private BeaverData saveData = new BeaverData(0);

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    //IMPLEMENTAR POOLING PARA LOS TRONCOS //Igual tampoco es necesario


    private void OnEnable()
    {
        NextLevelTrigger.OnNextScreenTriggered += AdvanceScreen;
    }

    private void OnDisable()
    {
        NextLevelTrigger.OnNextScreenTriggered -= AdvanceScreen;
    }

    public void StartNewGame()
    {
        DeleteData();
        StartCoroutine(SetupGameScene(true));
    }
    
    public void ContinueGame()
    {
        Debug.Log("COntinueGame");
        LoadData();
        StartCoroutine (SetupGameScene(false));

    }
    

    private void AdvanceScreen()
    {
        currentLevelIndex ++;

        //currentLevel = Instantiate(levels[currentScore], new Vector3(0,currentLevel.transform.localScale.z * 10f + (currentLevel.transform.localScale.z * 10f)/2 ,0) , levels[currentScore].transform.rotation);
        float previousLevelTop = currentLevel.transform.position.y + (currentLevel.GetComponent<Renderer>().bounds.size.y);

        currentLevel = Instantiate(
            levels[currentLevelIndex],
            new Vector3(0, previousLevelTop, 0),
            levels[currentLevelIndex].transform.rotation
        );
        
        SaveFile();
        cameraController.GoUp(cameraDistance);
    }
    

    private void SaveFile()
    {   
        saveData.levelNumber = currentLevelIndex;
        var json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "beaverSave.json"), json);
    }

    private bool LoadData()
    {
        var path = Path.Combine(Application.persistentDataPath, "beaverSave.json");
        if (File.Exists(path))
        {
            saveData = JsonUtility.FromJson<BeaverData>(File.ReadAllText(path));
            return true;
        }
        else
        {   
            SaveFile();
            return false;
        }
    }

    private void DeleteData()
    {
        var path = Path.Combine(Application.persistentDataPath, "beaverSave.json");
        if(!File.Exists(path)) { File.Delete(path); }
        currentLevelIndex = 0;
        saveData.levelNumber = 0;
    }

    public BeaverController ReturnBeaver()
    {
        return player.GetComponent<BeaverController>();
    }

    private IEnumerator SetupGameScene(bool newGame)
    {
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(4);
        yield return new WaitUntil(() => asyncLoad.isDone);
        yield return null;

        if (!newGame)
        {
            currentLevelIndex = saveData.levelNumber;
        }

        Instantiate(player, new Vector3(0f, 1f, -0.48f), Quaternion.identity);

        cameraController = FindAnyObjectByType<BeaverCameraController>();
        currentLevel = Instantiate(levels[currentLevelIndex], new Vector3(0, 10, 0), levels[currentLevelIndex].transform.rotation);
        Debug.Log(currentLevelIndex);
    }

    private IEnumerator ReturnToMenu()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(3);
        yield return new WaitUntil(() => asyncLoad.isDone);
        yield return null;

        Debug.Log("Cambia de escena");

        GameObject botonContinuar = GameObject.Find("Continue");
        GameObject botonNuevo = GameObject.Find("New Game");

        if (botonContinuar != null && botonNuevo != null)
        {
            Debug.Log("Botones encontrados correctamente.");
            botonContinuar.GetComponent<Button>().onClick.AddListener(() => BeaverManager.Instance.ContinueGame());
            botonNuevo.GetComponent<Button>().onClick.AddListener(() => BeaverManager.Instance.StartNewGame());
        }
        else
        {
            Debug.LogWarning("No se encontraron los botones.");
        }

    }

    public void ReturnToMenuCall()
    {
        StartCoroutine(ReturnToMenu());
    }

}
