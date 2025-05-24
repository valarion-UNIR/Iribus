using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class PachinkoManager : SubGamePlayerController
{
    public override SubGame SubGame => SubGame.Pachinko;

    [SerializeField] private GameObject ball;
    
    [SerializeField] private CinemachineCamera mainCamera;

    [Header("Ball Thrower Opctions")]
    [SerializeField] private float ballLocationMoveSpeed;
    [SerializeField] private Transform leftLimit;
    [SerializeField] private Transform rightLimit;
    [SerializeField] private Transform throwPoint;

    [Header("Slots Configuration")]
    [SerializeField] private GameObject slotsList;
    [SerializeField] private Material matIncorrect;
    [SerializeField] private Material matCorrect;
    [SerializeField] private Material matStarred;

    [Header("Canvas")]
    [SerializeField] private Canvas canvas;
    [SerializeField] public int numberOfRemainingBalls;
    [SerializeField] public int numberOfPrize;
    [SerializeField] public int numberOfScore;

    private GameObject instBall;
    private bool ballThrown = false;

    protected override void Awake()
    {
        base.Awake();

        string s = CreateSlotCombination(2, slotsList);
        BuildSlotCombination(slotsList, s);
    }
    void Start()
    {
        instBall = CreatePachinkoBall(ball);
        ballThrown = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSideCanvas();
        if (!ballThrown)
        {
            MoveArrowAndBall(throwPoint.gameObject);
            if (Input.Pachinko.ReleaseBall.triggered)
            {
                numberOfRemainingBalls--;
                //mainCamera.transform.SetParent(instBall.transform);
                ballThrown = true;
                instBall.transform.SetParent(transform);
                instBall.GetComponent<Rigidbody>().isKinematic = false;
                instBall.GetComponent<Rigidbody>().useGravity = true;
                instBall.AddComponent<PachinkoBall>();
                instBall = null;
            }
        }
    }

    private GameObject CreatePachinkoBall(GameObject ball)
    {
        if (numberOfRemainingBalls>0)
        {
            GameObject instantiatedBall = Instantiate(ball, throwPoint.position, Quaternion.identity);
            instantiatedBall.transform.SetParent(throwPoint);
            instantiatedBall.transform.localScale = throwPoint.transform.localScale/2;
            return instantiatedBall;
        }
        else
        {
            return null;
        }   
    }

    private void UpdateSideCanvas()
    {
        char[] score = "0000".ToCharArray();
        int cont = score.Length - 1;

        foreach (char s in numberOfScore.ToString().Reverse())
        {
            score[cont] = s;
            cont--;
        }

        string finalScore = new string(score);

        canvas.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = finalScore;

        char[] balls = "00".ToCharArray();
        int cont2 = balls.Length - 1;

        foreach (char b in numberOfRemainingBalls.ToString().Reverse())
        {
            balls[cont2] = b;
            cont2--;
        }

        string finalBalls = new string(balls);

        canvas.transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>().text = "x"+finalBalls;
        canvas.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = numberOfPrize.ToString();
    }

    private void MoveArrowAndBall(GameObject go)
    {
        if (go != null)
        {
            float inputH = Input.Pachinko.Move.ReadValue<Vector2>().x;
            Vector3 direccion = (rightLimit.position - leftLimit.position).normalized;
            Vector3 movimiento = direccion * inputH * ballLocationMoveSpeed * Time.deltaTime;
            go.transform.position += movimiento;

            Vector3 inicioToPos = go.transform.position - leftLimit.position;
            float proyeccion = Vector3.Dot(inicioToPos, direccion);

            float distanciaTotal = Vector3.Distance(leftLimit.position, rightLimit.position);
            proyeccion = Mathf.Clamp(proyeccion, 0f, distanciaTotal);

            go.transform.position = leftLimit.position + direccion * proyeccion;
        }
    }

    private void BuildSlotCombination(GameObject go, string combString)
    {
        int cont = 0;
        foreach (char t in combString)
        {
            Transform child = go.transform.GetChild(cont);
            switch (t)
            {
                case 'X':
                    child.gameObject.tag = TagsIribus.PCKIncorrect;
                    child.GetChild(0).GetComponent<MeshRenderer>().material = matIncorrect;
                    foreach (Transform o in child)
                    {
                        o.gameObject.tag = TagsIribus.PCKIncorrect;
                    }
                    break;
                case 'O':
                    child.gameObject.tag = TagsIribus.PCKCorrect;
                    child.GetChild(0).GetComponent<MeshRenderer>().material = matCorrect;
                    foreach (Transform o in child)
                    {
                        o.gameObject.tag = TagsIribus.PCKCorrect;
                    }
                    break;
                case 'S':
                    child.gameObject.tag = TagsIribus.PCKStar;
                    child.GetChild(0).GetComponent<MeshRenderer>().material = matStarred;
                    foreach (Transform o in child)
                    {
                        o.gameObject.tag = TagsIribus.PCKStar;
                    }
                    break;
            }
            cont++;
        }
    }

    private string CreateSlotCombination(int numberIncorrect, GameObject slotList)
    {
        if ((slotList.transform.childCount - numberIncorrect) < 2)
        {
            Debug.LogWarning("Readjust the incorrect number of slots");
            return null;
        }

        List<char> chars = new List<char>();

        chars.AddRange(Enumerable.Repeat('X', numberIncorrect));                                        //INCORRECT
        chars.Add('S');                                                                                 //STAR
        chars.AddRange(Enumerable.Repeat('O', slotList.transform.childCount - numberIncorrect - 1));    //CORRECT

        chars = chars.OrderBy(_ => Random.value).ToList();

        string result = new string(chars.ToArray());
        return result;
    }

    public void NextBallToCreate()
    {
        if (numberOfRemainingBalls == 0) return;

        instBall = CreatePachinkoBall(ball);
        ballThrown = false;
    }
}
