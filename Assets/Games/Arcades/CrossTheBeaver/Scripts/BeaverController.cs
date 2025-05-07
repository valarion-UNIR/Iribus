using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BeaverController : SubGamePlayerController
{
    public override SubGame SubGame => subGame;

    [SerializeField] private SubGame subGame;

    [SerializeField] private float moveDistanceVertical = 1.0f;
    [SerializeField] private float moveDistanceHorizontal = 1.0f;
    [SerializeField] private float moveSpeed = 50;
    private bool isMoving = false;

    private Vector2 inputDir;
    private Vector2 lastInput;
    private Vector3 targetPos;
    private bool isVertical;
    private LogBehaviour currentLog = null;

    private Rigidbody2D rb;


    protected override void Awake()
    {
        base.Awake();
        Input.Enable();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        

    }

    private void FixedUpdate()
    {
        // Leer input actual del vector 2D
        inputDir = Input.CrossTheBeaver.Move.ReadValue<Vector2>();

        if (inputDir != Vector2.zero)
        {
            // Para priorizar movimientos y evitar diagonal
            if (Mathf.Abs(inputDir.x) > Mathf.Abs(inputDir.y)) 
            {
                inputDir = new Vector2(Mathf.Sign(inputDir.x), 0);
                isVertical = false;
            }


            else
            {
                inputDir = new Vector2(0, Mathf.Sign(inputDir.y));
                isVertical = true;
            }
                

            // Evita repetición si mantienes pulsado
            if (inputDir != lastInput)
            {
                lastInput = inputDir;
                Vector2 dir = inputDir;
                if(isVertical) { targetPos = rb.position + dir * moveDistanceVertical; }
                else { targetPos = rb.position + dir * moveDistanceHorizontal; }
                
                StartCoroutine(MoveTo(targetPos));
            }
        }

        //Si no se presiona nada pues 0
        if (inputDir == Vector2.zero)
        {
            lastInput = Vector2.zero;
        }

        if (currentLog != null && !isMoving)
        {
            rb.MovePosition(rb.position + (Vector2)currentLog.DeltaPosition);
        }
    }

    IEnumerator MoveTo(Vector2 destination)
    {
        isMoving = true;
        while (Vector2.Distance(rb.position, destination) > 0.01f)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, destination, moveSpeed * Time.deltaTime));
            yield return null;
        }
        rb.MovePosition(destination);
        isMoving = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CTBLog"))
        {
            LogBehaviour log = collision.GetComponent<LogBehaviour>();
            if (log != null)
            {
                currentLog = log;
            }

            moveDistanceVertical = 3.0f;
            moveSpeed = moveSpeed * 2.5f;
        }

        else if(collision.gameObject.CompareTag("CTBWater"))
        {
            if(!isMoving && currentLog == null) 
            {
                Debug.Log("Muelto");
            }
        }
        

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(!isMoving && currentLog == null && collision.gameObject.CompareTag("CTBWater")) { Debug.Log("Muelto"); }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("CTBLog")) 
        { 
            currentLog = null;
            moveDistanceVertical = 1.0f;
            moveSpeed = moveSpeed / 2.5f;
        }
        
    }
}
