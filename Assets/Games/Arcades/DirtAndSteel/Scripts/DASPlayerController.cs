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

    private Rigidbody2D carRigidBody;
    private float steerInput;
    private float accelInput;
    private float currentAngularVelocity;
    private float driftTimer;

    protected override void Awake()
    {
        base.Awake();
        Input.Enable();
        carRigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
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

        if (accelInput > 0 && speed >= maxSpeed)
            return;

        Vector2 force = forward * accelInput * acceleration;
        carRigidBody.AddForce(force, ForceMode2D.Force);
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
