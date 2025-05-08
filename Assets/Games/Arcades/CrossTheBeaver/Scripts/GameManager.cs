using System;
using System.Collections.Generic;
using UnityEngine;

public class BeaverManager : MonoBehaviour
{
    public static BeaverManager Instance { get; private set; }
    //public static event Action CameraGoUp; 
    [SerializeField] private CameraController cameraController;
    //Luego cambiar esto para ajustarse a los planos pero en principio asi
    private float cameraDistance = 10 * 2;
    [Header("Levels")]
    [SerializeField] private BeaverController player;
    public int currentScore = 0;
    [SerializeField] private List<GameObject> levels;
    private GameObject currentLevel;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
       currentLevel = Instantiate(levels[currentScore], new Vector3(0, 10, 0), levels[currentScore].transform.rotation);
    }

    private void OnEnable()
    {
        NextLevelTrigger.OnNextScreenTriggered += AdvanceScreen;
    }

    private void OnDisable()
    {
        NextLevelTrigger.OnNextScreenTriggered -= AdvanceScreen;
    }

    private void AdvanceScreen()
    {
        currentScore ++;

        //currentLevel = Instantiate(levels[currentScore], new Vector3(0,currentLevel.transform.localScale.z * 10f + (currentLevel.transform.localScale.z * 10f)/2 ,0) , levels[currentScore].transform.rotation);
        float previousLevelTop = currentLevel.transform.position.y + (currentLevel.GetComponent<Renderer>().bounds.size.y);

        currentLevel = Instantiate(
        levels[currentScore],
        new Vector3(0, previousLevelTop, 0),
        levels[currentScore].transform.rotation
        );
        
        cameraController.GoUp(cameraDistance);
    }

    public BeaverController ReturnBeaver()
    {
        return player;
    }

}
