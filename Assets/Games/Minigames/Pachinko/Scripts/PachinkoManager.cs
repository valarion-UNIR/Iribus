using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PachinkoManager : MonoBehaviour
{
    [SerializeField] private GameObject ball;

    [SerializeField] private int numberOfRemainingBalls;
    [SerializeField] private Camera mainCamera;

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

    private GameObject instBall;

    private void Awake()
    {
        string s = CreateSlotCombination(2, slotsList);
        BuildSlotCombination(slotsList, s);
    }
    void Start()
    {
        instBall = CreatePachinkoBall(ball);
    }

    // Update is called once per frame
    void Update()
    {
        MoveArrowAndBall(throwPoint.gameObject);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mainCamera.transform.SetParent(instBall.transform);
            instBall.transform.SetParent(null);
            instBall.GetComponent<Rigidbody>().isKinematic = false;
            instBall.GetComponent<Rigidbody>().useGravity = true;
            instBall = null;
        }
    }

    private GameObject CreatePachinkoBall(GameObject ball)
    {
        if (numberOfRemainingBalls>0)
        {
            GameObject instantiatedBall = Instantiate(ball, throwPoint.position, Quaternion.identity);
            instantiatedBall.transform.SetParent(throwPoint);
            return instantiatedBall;
        }
        else
        {
            return null;
        }   
    }

    private void MoveArrowAndBall(GameObject go)
    {
        if (go != null)
        {
            float inputH = Input.GetAxisRaw("Horizontal");
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
            switch (t)
            {
                case 'X':
                    go.transform.GetChild(cont).GetChild(0).GetComponent<MeshRenderer>().material = matIncorrect;
                    break;
                case 'O':
                    go.transform.GetChild(cont).GetChild(0).GetComponent<MeshRenderer>().material = matCorrect;
                    break;
                case 'S':
                    go.transform.GetChild(cont).GetChild(0).GetComponent<MeshRenderer>().material = matStarred;
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
}
