using UnityEngine;
using System.Collections;

public class EnemigoMV3 : MonoBehaviour, IHitteable
{
    [SerializeField] private int hp = 3;

    // GENERALES
    [SerializeField] private Transform flipSpot;
    [SerializeField] Transform groundCheck;
    [SerializeField] private Transform watchSpot;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask entityLayer;

    private bool isGrounded;
    private float groundCheckRadius = 0.2f;
    private bool movingRight = true;
    private Rigidbody2D rb;

    // PATRULLA PARAMETROS
    [SerializeField] private int patrolSpeed;
    [SerializeField] private float pauseChance = 0.1f; // 10%
    [SerializeField] private float minPauseDuration = 1f;
    [SerializeField] private float maxPauseDuration = 2f;
    [SerializeField] private float flipChance = 0.3f; // 30%

    private bool isPaused = false;
    private float pauseTimer = 0f;

    // CHASE Y ATTACK PARAMETROS
    [SerializeField] private float detectionBoxX = 3f;
    [SerializeField] private float detectionBoxY = 1.5f;

    [SerializeField] private float chaseSpeed;
    [SerializeField] private float attackRange = 0.75f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackWindup = 0.75f;
    [SerializeField] private float jumpAttackHeight = 2f;
    [SerializeField] private float jumpForwardSpeed = 8f;

    [SerializeField] private ParticleSystem hurtParticles;
    [SerializeField] private float hurtTime = 0.5f;
    [SerializeField] private float knockBackForce = 10f;
    [SerializeField] private float knockBackPlayer = 25f;

    private float hurtTimer;
    private bool isHurt = false;

    private bool isAttacking = false;
    private Transform playerTarget;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
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

        CheckGround();

        if (isAttacking) return;

        if (CanSeePlayer())
        {
            AttackPlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private bool CanSeePlayer()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position, new Vector2(detectionBoxX, detectionBoxY), 0f, entityLayer);
        if (hit != null && hit.gameObject.CompareTag("Player"))
        {
            Debug.Log("Se overlapio");
            Vector2 dir = (hit.transform.position - transform.position).normalized;
            float distance = Vector2.Distance(hit.transform.position, transform.position);

            RaycastHit2D visionHit = Physics2D.Raycast(transform.position, dir, distance, groundLayer);
            if (visionHit.collider == null)
            {
                playerTarget = hit.transform;
                return true;
            }
        }
        return false;
    }

    private void AttackPlayer()
    {
        if (playerTarget == null) return;

        float dirToPlayer = playerTarget.position.x - transform.position.x;
        float moveDir = Mathf.Sign(dirToPlayer);

        if ((moveDir > 0 && !movingRight) || (moveDir < 0 && movingRight))
            Flip();

        //rb.linearVelocity = new Vector2(moveDir * chaseSpeed, rb.linearVelocity.y);

        StartCoroutine(PerformAttack(moveDir));
    }

    private IEnumerator PerformAttack(float direction)
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(attackWindup);

        float gravity = Mathf.Abs(Physics2D.gravity.y);
        float verticalVelocity = Mathf.Sqrt(2 * gravity * jumpAttackHeight);
        float horizontalVelocity = direction * jumpForwardSpeed;

        rb.linearVelocity = new Vector2(horizontalVelocity, verticalVelocity);

        yield return new WaitForSeconds(0.5f);
        while (!isGrounded) yield return null;

        rb.linearVelocity = Vector2.zero;

        isAttacking = false;
    }

    private void Patrol()
    {
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
        if (movingRight) transform.localScale = new Vector3(1, 1, 1);
        else transform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(flipSpot.position, 0.25f);
        Gizmos.DrawWireSphere(flipSpot.position + Vector3.down, 0.25f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(detectionBoxX, detectionBoxY, 1));
    }

    public float GetHit(Vector3 direction, DamageTypes dmgType)
    {
        hp = hp - 1;
        if (hp <= 0)
        {
            GameManagerMetroidvania.Instance.GetParticleSpawner().SpawnByIndex(2, transform.localPosition, transform.rotation);
            Destroy(gameObject);
        }
        hurtParticles.transform.rotation = Quaternion.LookRotation(-direction);
        hurtParticles.Play();
        rb.AddForce(-direction * 5f, ForceMode2D.Impulse);
        hurtTimer = hurtTime;
        isHurt = true;
        return knockBackPlayer;
    }
}
