using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

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



    private Vector2 moveInput;

    private float clampedX;
    private float clampedY;

    private bool esitando = false;

    private void Awake()
    {
        base.Awake();
        //Cambiarlo a cuando se active el minijuego
        Input.Enable();
    }

    private void Update()
    {
        if (!esitando)
        {
            Input.Claw.Grab.performed += ClawDescentCall;
        }
        
    }

    private void FixedUpdate()
    {
        
        clampedX = Mathf.Clamp(transform.position.x, -limitX, limitX);
        clampedY = Mathf.Clamp(transform.position.z, -limitZ, limitZ);
        transform.position = new Vector3(clampedX, transform.position.y, clampedY);

        if (!esitando)
        {
            moveInput = Input.Claw.Move.ReadValue<Vector2>();

            if (moveInput != Vector2.zero)
            {
                // Para priorizar movimientos y evitar diagonal
                if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
                {
                    moveInput = new Vector2(Mathf.Sign(moveInput.x), 0);
                }

                else
                {
                    moveInput = new Vector2(0, Mathf.Sign(moveInput.y));
                }

                Vector3 dir = new Vector3(moveInput.x, 0, moveInput.y);
                transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
            }
        }
        
    }

    private void ClawDescentCall(InputAction.CallbackContext obj)
    {
        esitando = true;
        StartCoroutine(ClawDescent());
    }

    IEnumerator ClawDescent()
    {
        while (transform.localPosition.y > limitY)
        {
            transform.Translate(Vector3.down * verticalSpeed * Time.deltaTime);
            yield return null;
        }

    }

    IEnumerator CerrarAnim()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 2;
            clawArm1.localRotation = Quaternion.Slerp(abiertoRot, cerradoRot, t);
            clawArm2.localRotation = Quaternion.Slerp(abiertoRot, cerradoRot, t);
            clawArm3.localRotation = Quaternion.Slerp(abiertoRot, cerradoRot, t);
            yield return null;
        }
    }
}
