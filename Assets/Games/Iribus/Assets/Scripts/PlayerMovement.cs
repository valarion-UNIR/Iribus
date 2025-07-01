using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    //MOVIMIENTO SUELO
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float groundAcceleration = 1f;
    [SerializeField] private float airAcceleration = 0.5f;
    [SerializeField] private float stopDrag = 10f;
    [SerializeField] private float moveDrag = 0f;

    //SALTO
    [SerializeField] private float jumpForce = 18f;
    [SerializeField] private float fallMultiplier = 6f;
    [SerializeField] private float lowJumpMultiplier = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundAccelerationTime = 0.1f;
    [SerializeField] private float maxFallSpeed = 20f;

    private float coyoteTime = 0.1f;
    private float coyoteCounter;
    private float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;

    //SALTO BOMBA O GROUND POUND PARA LOS AMIGOS
    [SerializeField] private float groundPoundImpulsoInicial = 20f;
    [SerializeField] private float groundPoundFallMult = 8f;
    [SerializeField] private bool groundPoundeadaDeManual = false;
    [SerializeField] private float groundPoundTime = 0.25f;
    [SerializeField] private float groundPoundJumpForce = 24f;

    private bool particulasGroundPound = false;
    private float groundPoundCounter;

    //ATACAR
    [SerializeField] GameObject attackHitbox;
    [SerializeField] private float attackDuration = 0.3f;
    [SerializeField] private float attackDelay = 0.3f;

    private bool isAttacking = false;
    private float attackTimer = 0f;
    private float attackDelayTimer = 0f;

    //SER GOLPEADO
    [SerializeField] private float hurtSideForce = 8f;
    [SerializeField] private float hurtUpForce = 7f;
    [SerializeField] private float hurtNoControlTime = 0.15f;
    [SerializeField] private float iFrames = 0.5f;

    private bool gotHurt = false;
    private float hurtTimer = 0f;
    private float iFramesTimer = 0f;

    //GENERAL
    [SerializeField] private ParticleSystem particulasHumo;
    [SerializeField] private ParticleSystem particulasHumo2;

    [SerializeField] private InteractorMV interactor;

    private Animator animator;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool jumpPressed;

    private bool blocked = false;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = moveDrag;
        attackHitbox.GetComponent<AtaqueHitbox>().pMovement = this;
    }

    void Update()
    {
        if (blocked) return;

        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            Debug.Log("Atacar");
            StartAttack();
        }

        moveInput = Input.GetAxisRaw("Horizontal");

        if (!isAttacking)
        {
            //if (moveInput > 0) sprite.flipX = false;
            //else if (moveInput < 0) sprite.flipX = true;
            if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
            else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
        }

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

        if (Input.GetKeyDown(KeyCode.E))
        {
            interactor.Interactuar();
        }
    }

    private void GroundPoundear()
    {

        rb.AddForce(Vector2.down * groundPoundImpulsoInicial, ForceMode2D.Impulse);
        rb.gravityScale = groundPoundFallMult;
        groundPoundeadaDeManual = true;
        particulasGroundPound = true;
        animator.SetBool("GroundPounde", true);
    }

    void FixedUpdate()
    {
        if (isAttacking)
        {
            attackTimer -= Time.fixedDeltaTime;
            if (attackTimer <= 0f)
            {
                attackHitbox.SetActive(false);

                attackDelayTimer -= Time.fixedDeltaTime;
                if (attackDelayTimer <= 0f)
                {
                    isAttacking = false;
                    Debug.Log("Se acabo atacar");
                }
            }
        }

        CheckGround();

        if (gotHurt)
        {
            hurtTimer -= Time.fixedDeltaTime;
            if (hurtTimer <= 0f) gotHurt = false;
        }
        else
        {
            Move();
        }
        JumpController();

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
        if (isGrounded && groundPoundeadaDeManual && particulasGroundPound)
        {
            particulasGroundPound = false;
            GameManagerMetroidvania.Instance.GetParticleSpawner().SpawnByIndex(1, groundCheck.position - new Vector3(0, 0.5f, 0), Quaternion.Euler(-90, 0, 0));
            animator.SetBool("GroundPounde", false);
        }

        animator.SetBool("saltando", !isGrounded);
    }

    private void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackDuration;
        attackDelayTimer = attackDelay;
        //animator.SetTrigger("Attack");
        attackHitbox.SetActive(true);
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

        if(jumpPressed)
        {
            rb.linearDamping = moveDrag;
        }
        else
        {
            rb.linearDamping = (moveInput == 0 && isGrounded)
                ? stopDrag
                : moveDrag;
        }

        //Debug.Log(rb.linearDamping);
        //if(jumpPressed) rb.linearDamping = moveDrag;

        if (Mathf.Abs(rb.linearVelocity.x) < 0.01f)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        if(groundPoundeadaDeManual)
        {
            particulasHumo2.Play();
            rb.AddForce(Vector2.up * groundPoundJumpForce, ForceMode2D.Impulse);
            animator.SetTrigger("GroundPound");
            groundPoundeadaDeManual = false;
        }
        else
        {
            particulasHumo.Play();
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void JumpController()
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

    public void GetHurt(Transform enemigo)
    {
        if (!gotHurt)
        {
            if(GameManagerMetroidvania.Instance.GetHurtPersonaje()) PlayerDie();

            gotHurt = true;
            hurtTimer = hurtNoControlTime;
            rb.linearDamping = 0;
            float dirToPlayer = enemigo.position.x - transform.position.x;
            float moveDir = Mathf.Sign(dirToPlayer);

            Vector2 forcePupa = new Vector2(hurtSideForce * -moveDir, hurtUpForce);

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(forcePupa, ForceMode2D.Impulse);
        }
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        if (gotHurt) return;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public void BlockMovement(bool block)
    {
        if(block)
        {
            blocked = true;
            moveInput = 0;
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            blocked = false;
        }
    }
}