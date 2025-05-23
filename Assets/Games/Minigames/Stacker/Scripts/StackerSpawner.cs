using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StackerSpawner : SubGamePlayerController
{
    public override SubGame SubGame => subGame;

    [SerializeField] private SubGame subGame;

    [SerializeField] private GameObject tile, bottomTile;

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private List<GameObject> stackOfTiles;

    [Header("Prizes")]
    [SerializeField] private GameObject lowPrizeSign;
    [SerializeField] private int lpNumber;
    [SerializeField] private GameObject mediumPrizeSign;
    [SerializeField] private int mpNumber;
    [SerializeField] private GameObject highPrizeSign;
    [SerializeField] private int hpNumber;

    private bool hasGameStarted, hasGameEnded;
    [Header("Colors")]
    [SerializeField] private List<Color32> listColors;
    private int modifier;
    private int colorIndex;
    public bool gameOverBool;
    private Vector3 initialCameraPosition;

    public static StackerSpawner instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {
        base.Awake();

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        LocatePrize(lowPrizeSign, lpNumber);
        LocatePrize(mediumPrizeSign, mpNumber);
        LocatePrize(highPrizeSign, hpNumber);

        initialCameraPosition = FindFirstObjectByType<Camera>().transform.position;
        if (scoreText == null)
        {
            scoreText = GameObject.Find("ScoreText").GetComponent<TMP_Text>();
        }
        stackOfTiles = new List<GameObject>();
        hasGameEnded = false;
        hasGameStarted = true;
        modifier = 1;
        colorIndex = 0;
        stackOfTiles.Add(bottomTile);
        stackOfTiles[0].GetComponent<MeshRenderer>().material.color = listColors[0];
        CreateTile();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasGameEnded || !hasGameStarted) return;

        if (Input.Stacker.PutBlock.triggered)
        {
            if (stackOfTiles.Count == 2)
            {
                Destroy(stackOfTiles[stackOfTiles.Count - 1].GetComponent<StackerTile>());
            }
            else if (stackOfTiles.Count > 2)
            {
                stackOfTiles[stackOfTiles.Count - 1].GetComponent<StackerTile>().ScaleTile();
            }
            if (hasGameEnded)
            {
                Destroy(stackOfTiles[stackOfTiles.Count - 1]);
                return;
            }
            if (stackOfTiles.Count > 8)
            {
                StartCoroutine(MoveCamera());
            }
            scoreText.text = (stackOfTiles.Count - 1).ToString();
            CreateTile();
        }
    }

    IEnumerator MoveCamera()
    {
        float moveLenght = tile.transform.localScale.y;
        GameObject camera = FindFirstObjectByType<Camera>().gameObject;
        while (moveLenght > 0)
        {
            float stepLenght = 0.1f;
            moveLenght -= stepLenght;
            camera.transform.Translate(0, stepLenght, 0, Space.World);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void CreateTile()
    {
        GameObject activeTile;
        GameObject previousTile = stackOfTiles[stackOfTiles.Count - 1];

        activeTile = Instantiate(tile);
        stackOfTiles.Add(activeTile);

        if (stackOfTiles.Count > 2)
        {
            activeTile.transform.localScale = previousTile.transform.localScale;
        }

        activeTile.transform.position = new Vector3(
            previousTile.transform.position.x,
            previousTile.transform.position.y + previousTile.transform.localScale.y,
            previousTile.transform.position.z
            );

        colorIndex += modifier;
        if (colorIndex == listColors.Count || colorIndex == -1)
        {
            modifier *= -1;
            colorIndex += 2 * modifier;
        }
        activeTile.GetComponent<MeshRenderer>().material.color = listColors[colorIndex];
    }

    public GameObject GetFirstTile()
    {
        return stackOfTiles[0];
    }

    public void GameOver()
    {
        hasGameEnded = true;
    }

    private void LocatePrize(GameObject sign, int signNumber)
    {
        GameObject obj = Instantiate(sign);
        obj.transform.position = new Vector3(0, (signNumber * 2) - 1,0.5f);
        StartCoroutine(StartMovingPrizeSign(obj, signNumber));
    }

    IEnumerator StartMovingPrizeSign(GameObject g, int num)
    {
        Material m = g.GetComponent<MeshRenderer>().material;
        float cont = 0f;
        float speed = 0.5f;

        while (stackOfTiles.Count - 1 <= num)
        {
            cont += speed * Time.deltaTime;
            float offset = Mathf.Repeat(cont, 1f);

            m.mainTextureOffset = new Vector2(offset,0f);

            yield return null;
        }
    }
}
