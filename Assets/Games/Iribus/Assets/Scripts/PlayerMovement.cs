using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float groundAcceleration = 1f;
    public float airAcceleration = 0.5f;
    public float stopDrag = 10f;
    public float moveDrag = 0f;

    public float jumpForce = 18f;
    public float fallMultiplier = 6f;
    public float lowJumpMultiplier = 10f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public float groundAccelerationTime = 0.1f; 

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool jumpPressed;

    private float coyoteTime = 0.1f;
    private float coyoteCounter;
    private float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;

    public float maxFallSpeed = 20f;

    public float groundPoundFallMult = 8f;
    public bool groundPoundeadaDeManual = false;
    private float groundPoundTime = 0.25f;
    private float groundPoundCounter;
    public float groundPoundJumpForce = 24f;

    private Animator animator;
    private SpriteRenderer sprite;
    private ParticleSystem particulasHumo;

    void Start()
    {
        particulasHumo = GetComponentInChildren<ParticleSystem>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = moveDrag;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        if (moveInput > 0)
            sprite.flipX = false;
        else if (moveInput < 0)
            sprite.flipX = true;

        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;
        coyoteCounter = isGrounded ? coyoteTime : coyoteCounter - Time.deltaTime;

        if (jumpBufferCounter > 0 && coyoteCounter > 0f)
        {
            Jump();
            jumpBufferCounter = 0f;
        }

        if (groundPoundeadaDeManual && isGrounded)
        {
            groundPoundCounter -= Time.deltaTime;
            if(groundPoundCounter < 0f)
            {
                groundPoundeadaDeManual = false;
            }
        }
        else
        {
            groundPoundCounter = groundPoundTime;
        }

        if (Input.GetKeyDown(KeyCode.S) && !isGrounded)
        {
            GroundPoundear();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            PlayerDie();
        }
    }

    private void GroundPoundear()
    {
        animator.SetTrigger("GroundPound");
        rb.AddForce(Vector2.down * 10f, ForceMode2D.Impulse);
        rb.gravityScale = groundPoundFallMult;
        groundPoundeadaDeManual = true;
    }

    void FixedUpdate()
    {
        CheckGround();
        Move();
        BetterJump();

        if (jumpPressed)
        {
            Jump();
            jumpPressed = false;
        }

        if (rb.linearVelocity.y < -maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);
        }
        animator.SetFloat("xVel", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("yVel", rb.linearVelocity.y);
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position, groundCheckRadius, groundLayer
        );
        animator.SetBool("saltando", !isGrounded);
    }

    void Move()
    {
        float targetSpeed = moveInput * moveSpeed;
        float accelRate = (groundAccelerationTime > 0f)
            ? moveSpeed / groundAccelerationTime
            : Mathf.Infinity;

        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float forceX = rb.mass * speedDiff * accelRate;
        rb.AddForce(Vector2.right * forceX, ForceMode2D.Force);

        rb.linearDamping = (moveInput == 0 && isGrounded)
            ? stopDrag
            : moveDrag;

        if (Mathf.Abs(rb.linearVelocity.x) < 0.01f)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    void Jump()
    {
        particulasHumo.Play();
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        if(groundPoundeadaDeManual)
        {
            rb.AddForce(Vector2.up * groundPoundJumpForce, ForceMode2D.Impulse);
            groundPoundeadaDeManual = false;
        }
        else
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void BetterJump()
    {
        if (rb.linearVelocity.y < 0)
            rb.gravityScale = fallMultiplier;
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
            rb.gravityScale = lowJumpMultiplier;
        else
            rb.gravityScale = 5f;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    private void PlayerDie()
    {
        Debug.Log(GameManagerMetroidvania.Instance.GetParticleSpawner());
        GameManagerMetroidvania.Instance.GetParticleSpawner().SpawnByIndex(0, transform.localPosition, transform.rotation);
        GameManagerMetroidvania.Instance.PlayerMuerte();
        Destroy(gameObject);
    }
}