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
    public float driftMemoryTime = 0.5f; // Duration to continue drifting after releasing steering

    [SerializeField] private float decayStartPercent = 0.1f; // Start decaying after 40% of max speed
    [SerializeField] private float decaySharpness = 5f;      // How sharp the decay is

    public Rigidbody2D carRigidBody;
    [HideInInspector] public float steerInput;
    [HideInInspector] public float accelInput;
    private float currentAngularVelocity;
    private float driftTimer;

    [HideInInspector] public Vector2 currentDirection;

    protected override void Awake()
    {
        base.Awake();
        carRigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        currentDirection = transform.up;
        steerInput = Input.DirtAndSteel.Move.ReadValue<Vector2>().x;

        if (Mathf.Abs(steerInput) > driftThreshold)
            driftTimer = driftMemoryTime; 
        else
            driftTimer -= Time.deltaTime; 

        if ((!Input.DirtAndSteel.Accelerate.inProgress && !Input.DirtAndSteel.Break.inProgress) ||
            (Input.DirtAndSteel.Accelerate.inProgress && Input.DirtAndSteel.Break.inProgress))
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
        float speed = Vector2.Dot(carRigidBody.linearVelocity, forward);

        if (accelInput == 0)
            return;

        float maxSpeedInCurrentDirection = maxSpeed * Mathf.Sign(accelInput);
        float speedPercent = Mathf.Clamp01(speed / maxSpeedInCurrentDirection);

        float decayFactor;

        if (speedPercent < decayStartPercent)
        {
            // No decay yet
            decayFactor = 1f;
        }
        else
        {
            // Map speed from [decayStart, 1] to [0, 1] for the decay curve
            float normalized = (speedPercent - decayStartPercent) / (1f - decayStartPercent);
            decayFactor = Mathf.Pow(1f - normalized, decaySharpness);
        }

        Vector2 force = forward * accelInput * acceleration * decayFactor;
        carRigidBody.AddForce(force, ForceMode2D.Force);

        print(carRigidBody.linearVelocity);
    }



    void ApplyIdleFriction()
    {
        if (Mathf.Approximately(accelInput, 0f))
        {
            float slowDown = 0.5f;
            carRigidBody.linearVelocity *= (1 - slowDown * Time.fixedDeltaTime);
        }
    }

    void ApplySteering()
    {
        float velocityFactor = carRigidBody.linearVelocity.magnitude / maxSpeed;
        float steerAmount = steerInput * steering * velocityFactor * Time.fixedDeltaTime;

        if (accelInput < 0f)
            steerAmount *= -1;

        carRigidBody.MoveRotation(carRigidBody.rotation - steerAmount);
    }

    void KillOrthogonalVelocity()
    {
        Vector2 forward = transform.up;
        Vector2 right = transform.right;

        float forwardVelocity = Vector2.Dot(carRigidBody.linearVelocity, forward);
        float sidewaysVelocity = Vector2.Dot(carRigidBody.linearVelocity, right);

        bool drifting = driftTimer > 0f;
        float driftFactor = drifting ? driftFactorSlippy : driftFactorSticky;

        carRigidBody.linearVelocity = forward * forwardVelocity + right * sidewaysVelocity * driftFactor;
    }
}
