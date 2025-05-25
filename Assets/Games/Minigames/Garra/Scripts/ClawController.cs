using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClawController : SubGamePlayerController
{
    public override SubGame SubGame => subGame;

    [SerializeField] private SubGame subGame;

    [Header("Coordinate Limits")]
    [SerializeField] private float limitX;
    [SerializeField] private float limitZ;
    [SerializeField] private float limitY;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float verticalSpeed;

    [Header("Claw Arms")]
    [SerializeField] private GameObject clawArm1;
    [SerializeField] private GameObject clawArm2;
    [SerializeField] private GameObject clawArm3;

    [SerializeField] private float closeDuration;

    [Header("Target")]
    [SerializeField] private Transform prizeTarget;
    [SerializeField] private float detectionRadius;
    [SerializeField] private LayerMask ballsLayer;

    private Vector2 moveInput;

    private bool inProgress = false;

    private Collider currentCol;

    private void Start()
    {
        detectionRadius *= transform.localScale.y;
    }

    private void OnEnable()
    {
        Input.Claw.Grab.performed += ClawDescentCall;
    }

    private void OnDisable()
    {
        Input.Claw.Grab.performed -= ClawDescentCall;
    }

    private void FixedUpdate()
    {

        if (!inProgress)
        {
            moveInput = Input.Claw.Move.ReadValue<Vector2>();

            if (moveInput != Vector2.zero)
            {
                // Para priorizr un eje y no se mueva en diagonal
                if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
                    moveInput = new Vector2(Mathf.Sign(moveInput.x), 0);
                else
                    moveInput = new Vector2(0, Mathf.Sign(moveInput.y));

                // Movimiento propuesto
                Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);
                Vector3 proposedLocalPos = transform.localPosition + moveDir * moveSpeed * Time.deltaTime;

                // Clamp antes de aplicar el movimiento
                proposedLocalPos.x = Mathf.Clamp(proposedLocalPos.x, -limitX, limitX);
                proposedLocalPos.z = Mathf.Clamp(proposedLocalPos.z, -limitZ, limitZ);

                transform.localPosition = proposedLocalPos;
            }
        }

    }

    private void ClawDescentCall(InputAction.CallbackContext obj)
    {
        if (!inProgress)
        {
            inProgress = true;
            StartCoroutine(ClawDescent());
        }
    }

    IEnumerator ClawDescent()
    {   
        //Guardamos la posición inicial
        Vector3 startPosition = transform.localPosition;

        //Bajamos hasta llegar al límite vertical
        while (transform.localPosition.y > limitY)
        {   
            transform.Translate(Vector3.down * verticalSpeed * Time.deltaTime);

            //Mientras bja abrimos la garra
            StartCoroutine(CloseOpen(clawArm1.transform, 60f, closeDuration));
            StartCoroutine(CloseOpen(clawArm2.transform, 60f, closeDuration));
            StartCoroutine(CloseOpen(clawArm3.transform, 60f, closeDuration));
            yield return null;
        }
        
        yield return new WaitForSeconds(2f);
        
        //Se cierra la garra
        StartCoroutine(CloseOpen(clawArm1.transform, 0f, closeDuration));
        StartCoroutine(CloseOpen(clawArm2.transform, 0f, closeDuration));
        StartCoroutine(CloseOpen(clawArm3.transform, 0f, closeDuration));

        yield return new WaitForSeconds(1.5f);

        //Subimos el objeto
        while (transform.localPosition.y < startPosition.y)
        {
            transform.Translate(Vector3.up * verticalSpeed * Time.deltaTime);
            yield return null;
        }

        //Aqui arriba comprobamos si hay un objeto y se lo atachemos al padre para que no se salga
        if (Physics.OverlapSphere(transform.position, detectionRadius, ballsLayer).Length > 0)
        {
            currentCol = Physics.OverlapSphere(transform.position, detectionRadius, ballsLayer)[0];
            currentCol.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            currentCol.gameObject.transform.SetParent(transform);
        }
        

        yield return new WaitForSeconds(1f);

        //Vamos a l zona de dejado
        while(transform.localPosition.x > -limitX)
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            yield return null;
        }

        while (transform.localPosition.z > -limitZ)
        {
            transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime);
            yield return null;
        }

        //Desatacheamos el objeto
        if(currentCol != null)
        {
            currentCol.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            currentCol.gameObject.transform.SetParent(null);
        }
        
        //Volvemos a abrir
        StartCoroutine(CloseOpen(clawArm1.transform, 60f, closeDuration));
        StartCoroutine(CloseOpen(clawArm2.transform, 60f, closeDuration));
        StartCoroutine(CloseOpen(clawArm3.transform, 60f, closeDuration));

        yield return new WaitForSeconds(1.5f);

        //Cerramos una vez mas
        StartCoroutine(CloseOpen(clawArm1.transform, 0f, closeDuration));
        StartCoroutine(CloseOpen(clawArm2.transform, 0f, closeDuration));
        StartCoroutine(CloseOpen(clawArm3.transform, 0f, closeDuration));

        //Y volvemos  la posiciñon inicial
        while (transform.localPosition.x < startPosition.x)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            yield return null;
        }

        while (transform.localPosition.z < startPosition.z)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            yield return null;
        }

        //Permitimos que se vuelva a jugar
        inProgress = false;

    }

    IEnumerator CloseOpen(Transform arm, float angle, float duration)
    {
        float time = 0f;
        float startingAngle = arm.localRotation.eulerAngles.z;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time/duration);
            float zangle = Mathf.Lerp(startingAngle, angle, t);


            Vector3 rotEulers = arm.localRotation.eulerAngles;
            rotEulers.z = zangle;
            arm.localRotation = Quaternion.Euler(rotEulers);


            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
