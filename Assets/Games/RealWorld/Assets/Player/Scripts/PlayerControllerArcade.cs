using UnityEngine;

public class PlayerControllerArcade : RealWorldSubGamePlayerController
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravityMultiplier = 2f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.2f;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private CameraController camController;

    private Rigidbody rb;
    private bool isGrounded;
    private float verticalVelocity;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        if (camController == null)
            Debug.LogWarning("Falta Camera Controller");
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);
    }

    private void FixedUpdate()
    {
        float inputX = Input.Player.Move.ReadValue<Vector2>().x;    
        float inputZ = Input.Player.Move.ReadValue<Vector2>().y;

        float yaw = camController != null
            ? camController.GetCurrentYaw()
            : Camera.main.transform.eulerAngles.y;
        Quaternion yawRot = Quaternion.Euler(0f, yaw, 0f);
        Vector3 moveInput = new Vector3(inputX, 0f, inputZ);
        Vector3 moveDir = yawRot * moveInput;
        moveDir = Vector3.ClampMagnitude(moveDir, 1f);

        Vector3 horizontalVel = moveDir * moveSpeed;

        float gravityThisFrame = Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
        verticalVelocity += gravityThisFrame;
        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        Vector3 finalVel = horizontalVel + Vector3.up * verticalVelocity;
        rb.linearVelocity = finalVel;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}