using System.Collections;
using UnityEngine;

public class DASPlayerController : SubGamePlayerController
{
    public override SubGame SubGame => subGame;

    [SerializeField] private SubGame subGame;

    [Header("Movement")]
    public float acceleration = 10f;
    public float maxSpeed = 15f;
    public float steering = 200f;

    [HideInInspector] public float baseMaxSpeed = 15f;

    [Header("Drifting")]
    public float driftFactorSticky = 0.8f;
    public float driftFactorSlippy = 0.95f;
    public float driftThreshold = 0.8f;
    public float driftMemoryTime = 0.5f; // Duration to continue drifting after releasing steering

    [SerializeField] private float decayStartPercent = 0.1f; // Start decaying after 40% of max speed
    [SerializeField] private float decaySharpness = 5f;      // How sharp the decay is

    [HideInInspector] public Rigidbody2D carRigidBody;
    [HideInInspector] public float steerInput;
    [HideInInspector] public float accelInput;
    private float currentAngularVelocity;
    private float driftTimer;
    [SerializeField] private float driftRecoveryTime = 0.5f; // Time to fully return to grip after drifting
    [SerializeField] private TrailRenderer trailL;
    [SerializeField] private TrailRenderer trailR;
    private float driftRecoveryTimer = 0f;
    //private float lastDriftSteerInput = 0f;
    [HideInInspector] public Vector2 currentDirection;

    private bool canAccelerate = true;

    [Header("Steering Brake")]
    [Tooltip("How much speed to lose per second when steering (0 = no brake, 1 = full stop)")]
    public float steerBrakeStrength = 0.2f;
    protected override void Awake()
    {
        base.Awake();
        carRigidBody = GetComponent<Rigidbody2D>();
        baseMaxSpeed = maxSpeed;
    }

    void Update()
    {
        currentDirection = transform.up;
        steerInput = Input.DirtAndSteel.Move.ReadValue<Vector2>().x;

        if (Input.DirtAndSteel.Drift.IsPressed())
        {
            driftTimer = driftMemoryTime;
            driftRecoveryTimer = 0f; // reset recovery if drifting again
        }
        else
        {
            if (driftTimer > 0f)
            {
                driftTimer -= Time.deltaTime;
                if (driftTimer <= 0f)
                {
                    // Drifting just ended — start recovery
                    driftRecoveryTimer = driftRecoveryTime;
                }
            }
            else
            {
                // Count down recovery if in progress
                driftRecoveryTimer -= Time.deltaTime;
            }
        }

        driftTimer = Mathf.Clamp(driftTimer, 0f, driftMemoryTime);

        if (Input.DirtAndSteel.SlowDownTime.IsPressed())
            DASSlowdownManager.Instance.StartSlowdown(0.5f, 5f);

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
        bool isSteering = Mathf.Abs(steerInput) > 0.1f;
        bool isDrifting = driftTimer > 0f || driftRecoveryTimer > 0f;

        if (Input.DirtAndSteel.Drift.IsPressed() || driftTimer > 0f)
        {
            trailL.emitting = true;
            trailR.emitting = true;
        }
        else
        {
            trailL.emitting = false;
            trailR.emitting = false;
        }

        if (isSteering && !isDrifting && maxSpeed == baseMaxSpeed)
        {
            carRigidBody.linearVelocity *=
                1f - steerBrakeStrength * Time.fixedDeltaTime * Mathf.Abs(steerInput);
        }

        if (accelInput == 0 && driftRecoveryTimer <= 0f)
            ApplyIdleFriction();

        if(canAccelerate)
            ApplyEngineForce();


        KillOrthogonalVelocity();
        ApplySteering();
    }


    void ApplyEngineForce()
    {
        Vector2 forward = transform.up;
        float speed = Vector2.Dot(carRigidBody.linearVelocity, forward);

        if (accelInput == 0 || carRigidBody.linearVelocity.magnitude > maxSpeed)
            return;

        float maxSpeedInCurrentDirection = maxSpeed * Mathf.Sign(accelInput);
        float speedPercent = Mathf.Clamp01(speed / maxSpeedInCurrentDirection);

        float decayFactor;

        if (speedPercent < decayStartPercent)
            decayFactor = 1f;
        else
        {
            // Map speed from [decayStart, 1] to [0, 1] for the decay curve
            float normalized = (speedPercent - decayStartPercent) / (1f - decayStartPercent);
            decayFactor = Mathf.Pow(1f - normalized, decaySharpness);
        }

        Vector2 force = forward * accelInput * acceleration * decayFactor;
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
        float velocityFactor = carRigidBody.linearVelocity.magnitude / baseMaxSpeed;
        float steerAmount = steerInput * steering * velocityFactor * Time.fixedDeltaTime;

        if (accelInput < 0f)
            steerAmount *= -1;

        if (driftTimer > 0f)
            steerAmount *= 1.25f;


        carRigidBody.MoveRotation(carRigidBody.rotation - steerAmount);
    }

    public void ApplySuddenVelocityChange(float velocityMultiplier, float duration)
    {
        StartCoroutine(CoApplySuddenVelocityChange(velocityMultiplier, duration));
    }

    public IEnumerator CoApplySuddenVelocityChange(float velocityMultiplier, float duration)
    {
        float timer = 0f;

        Vector2 originalVelocity = carRigidBody.linearVelocity;
        Vector2 targetVelocity = originalVelocity * velocityMultiplier;

        canAccelerate = false;

        while (timer < duration)
        {
            float t = timer / duration;
            float easedT = 1f - Mathf.Pow(1f - t, 4f);

            carRigidBody.linearVelocity = Vector2.Lerp(originalVelocity, targetVelocity, easedT);

            timer += Time.deltaTime;
            yield return null;
        }

        carRigidBody.linearVelocity = targetVelocity;
        canAccelerate = true;
    }

    void KillOrthogonalVelocity()
    {
        Vector2 forward = transform.up;
        Vector2 right = transform.right;

        float forwardVelocity = Vector2.Dot(carRigidBody.linearVelocity, forward);
        float sidewaysVelocity = Vector2.Dot(carRigidBody.linearVelocity, right);

        float driftFactor = driftFactorSticky;

        if (driftTimer > 0f)
        {
            // Actively drifting
            driftFactor = driftFactorSlippy;
        }
        else if (driftRecoveryTimer > 0f)
        {
            float t = 1f - (driftRecoveryTimer / driftRecoveryTime);
            t = Mathf.SmoothStep(0f, 1f, t); // ease-out shape
            driftFactor = Mathf.Lerp(driftFactorSlippy, driftFactorSticky, t);
        }
        else if (Mathf.Abs(steerInput) > 0.1f)
        {
            // Steering but not drifting: apply a smooth grip lerp
            float steerSlipLerp = Mathf.Abs(steerInput); // Could also be scaled by speed
            driftFactor = Mathf.Lerp(driftFactorSticky, 1f, steerSlipLerp * 0.5f); // small allowance for slip
        }

        carRigidBody.linearVelocity = forward * forwardVelocity + right * sidewaysVelocity * driftFactor;
    }

    public void LimitMaxSpeed(float speedLimit, float lerpSpeed)
    {
        maxSpeed = speedLimit;

        Vector2 velocity = carRigidBody.linearVelocity;
        float currentSpeed = velocity.magnitude;

        if (currentSpeed > speedLimit)
        {
            Vector2 targetVelocity = velocity.normalized * speedLimit;
            carRigidBody.linearVelocity = Vector2.Lerp(velocity, targetVelocity, lerpSpeed * Time.fixedDeltaTime);
        }
    }

    public void SetBaseMaxSpeed()
    {
        maxSpeed = baseMaxSpeed;
    }
}
