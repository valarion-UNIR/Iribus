using UnityEngine;
using System.Collections;

public class EnemigoMV5 : MonoBehaviour, IHitteable, IEnemigoMultiparte
{
    //VIDA
    [SerializeField] private int hp = 1;
    private bool invencible = true;

    // GENERALES
    [SerializeField] private Transform flipSpot;
    [SerializeField] Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask entityLayer;

    private bool isGrounded;
    private float groundCheckRadius = 0.2f;
    private bool movingRight = true;
    private Rigidbody2D rb;
    private Animator animator;

    // PATRULLA PARAMETROS
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float pauseChance = 0.01f;
    [SerializeField] private float minPauseDuration = 0.5f;
    [SerializeField] private float maxPauseDuration = 1f;
    [SerializeField] private float flipChance = 0.15f;

    private bool isPaused = false;
    private float pauseTimer = 0f;

    // CHASE Y ATTACK PARAMETROS
    [SerializeField] private ParticleSystem hurtParticles;
    [SerializeField] private ParticleSystem hurtNOParticles;
    [SerializeField] private float hurtTime = 0.5f;
    [SerializeField] private float knockBackForce = 10f;
    [SerializeField] private float knockBackPlayer = 25f;

    // GLBOO
    [SerializeField] private GameObject globo;
    [SerializeField] private float knockBackGlobo = 5f;
    [SerializeField] private float torqueGlobo = 10f;
    [SerializeField] private CuerdaEnemigo cuerda;

    [SerializeField] private float hurtTimeParteRota = 1.5f;
    [SerializeField] private float patrolSpeedParteRota = 3.5f;

    private float hurtTimer;
    private bool isHurt = false;

    private bool isAttacking = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        globo.GetComponentInChildren<EnemigoCuerpoSecundario>().SetMainCuerpo(this);
    }

    private void FixedUpdate()
    {
        if (isAttacking) return;

        CheckGround();

        Patrol();
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Patrol()
    {
        if (isHurt)
        {
            hurtTimer -= Time.fixedDeltaTime;
            if (hurtTimer <= 0f)
            {
                isHurt = false;
                Debug.Log("Se acabo lazear");
            }
            return;
        }

        if (isPaused)
        {
            pauseTimer -= Time.fixedDeltaTime;
            if (pauseTimer <= 0f)
            {
                isPaused = false;

                if (Random.value < flipChance)
                {
                    Flip();
                }
            }

            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (Random.value < pauseChance * Time.fixedDeltaTime)
        {
            isPaused = true;
            pauseTimer = Random.Range(minPauseDuration, maxPauseDuration);
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }


        float dir = movingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(dir * patrolSpeed, rb.linearVelocity.y);

        RaycastHit2D hit = Physics2D.CircleCast(flipSpot.position, 0.25f, Vector2.down, 1f, groundLayer);
        if (hit.collider == null && isGrounded)
        {
            Flip();
        }
    }

    private void Flip()
    {
        Debug.Log("Flip");
        movingRight = !movingRight;
        if (movingRight) transform.localScale = new Vector3(transform.localScale.x * -1 , transform.localScale.y, transform.localScale.z);
        else transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(flipSpot.position, 0.25f);
        Gizmos.DrawWireSphere(flipSpot.position + Vector3.down, 0.25f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(3, 1, 1));
    }

    public float GetHit(Vector3 direction, DamageTypes dmgType)
    {
        if(!invencible)
        {
            hp = hp - 1;
            if (hp <= 0)
            {
                GameManagerMetroidvania.Instance.GetParticleSpawner().SpawnByIndex(2, transform.localPosition, transform.rotation);
                Destroy(gameObject);
            }
            hurtParticles.transform.rotation = Quaternion.LookRotation(-direction);
            hurtParticles.Play();
        }
        else
        {
            hurtNOParticles.transform.rotation = Quaternion.LookRotation(-direction);
            hurtNOParticles.Play();
        }

        rb.AddForce(-direction * knockBackForce, ForceMode2D.Impulse);
        //if (balloonRb != null)
        //{
        //    balloonRb.AddForce(direction * knockBackGlobo, ForceMode2D.Impulse);
        //    balloonRb.AddTorque(-Mathf.Sign(direction.x) * torqueGlobo, ForceMode2D.Impulse);
        //}
        hurtTimer = hurtTime;
        isHurt = true;
        return knockBackPlayer;
    }

    public void ParteEliminated(EnemigoCuerpoSecundario parte)
    {
        cuerda.BreakRope();
        globo.GetComponent<SpringJoint2D>().enabled = false;
        globo.GetComponent<Rigidbody2D>().gravityScale = 4;
        globo.GetComponentInChildren<EnemigoCuerpoSecundario>().enabled = false;

        rb.linearVelocity = Vector2.zero;
        hurtTimer = hurtTimeParteRota;
        isHurt = true;
        patrolSpeed = patrolSpeedParteRota;
        invencible = false;
    }
}
