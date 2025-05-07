using UnityEngine;

public class DASPlayerController : SubGamePlayerController
{
    public override SubGame SubGame => subGame;

    [SerializeField] private SubGame subGame;

    [Header("Movement")]
    public float acceleration = 10f;
    public float maxSpeed = 15f;
    public float steering = 200f;

    [Header("Drifting")]
    public float driftFactorSticky = 0.8f;
    public float driftFactorSlippy = 0.95f;
    public float driftThreshold = 0.8f;

    private Rigidbody2D rb;
    private float steerInput;
    private float accelInput;
    private float currentAngularVelocity;
    public float steeringDecay = 2f; // Higher = faster decay

    protected override void Awake()
    {
        base.Awake();
        Input.Enable();
        rb = GetComponent<Rigidbody2D>();

    }

    void Update()
    {
        // Basic input handling
        steerInput = Input.DirtAndSteel.Move.ReadValue<Vector2>().x;   // A/D or Left/Right

        if ((!Input.DirtAndSteel.Accelerate.inProgress && !Input.DirtAndSteel.Break.inProgress) || (Input.DirtAndSteel.Accelerate.inProgress && Input.DirtAndSteel.Break.inProgress))
        {
            accelInput = 0;
            return;
        }

        if (Input.DirtAndSteel.Accelerate.IsPressed())
            accelInput = 1;

        if (Input.DirtAndSteel.Break.IsPressed())
            accelInput = -1;
    }

    void FixedUpdate()
    {
        if (accelInput == 0)
            ApplyIdleFriction();

        ApplyEngineForce();
        KillOrthogonalVelocity();
        ApplySteering();
    }

    void ApplyEngineForce()
    {
        Vector2 forward = transform.up;
        float speed = Vector2.Dot(rb.linearVelocity, forward);

        // Don't exceed max speed
        if (accelInput > 0 && speed >= maxSpeed)
            return;

        Vector2 force = forward * accelInput * acceleration;
        rb.AddForce(force, ForceMode2D.Force);
    }
    void ApplyIdleFriction()
    {
        if (Mathf.Approximately(accelInput, 0f))
        {
            float slowDown = 0.5f;
            rb.linearVelocity *= (1 - slowDown * Time.fixedDeltaTime);
        }
    }

    void ApplySteering()
    {
        float velocityFactor = rb.linearVelocity.magnitude / maxSpeed;
        float steerAmount = steerInput * steering * velocityFactor * Time.fixedDeltaTime;

        if (accelInput < 0f)
            steerAmount *= -1;



        rb.MoveRotation(rb.rotation - steerAmount); // NOTE: Unity rotates clockwise with negative angle
    }

    void KillOrthogonalVelocity()
    {
        Vector2 forward = transform.up;
        Vector2 right = transform.right;

        float forwardVelocity = Vector2.Dot(rb.linearVelocity, forward);
        float sidewaysVelocity = Vector2.Dot(rb.linearVelocity, right);

        float driftFactor = Mathf.Abs(steerInput) > driftThreshold ? driftFactorSlippy : driftFactorSticky;

        // Reconstruct new velocity vector
        rb.linearVelocity = forward * forwardVelocity + right * sidewaysVelocity * driftFactor;
    }
}
