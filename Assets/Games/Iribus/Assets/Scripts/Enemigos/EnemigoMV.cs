using System.Collections;
using UnityEngine;

public class EnemigoMV : MonoBehaviour
{
    [SerializeField] private int hp;


    [SerializeField] private Transform flipSpot;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask entityLayer;

    [SerializeField] private Transform watchSpot;

    [SerializeField] Transform groundCheck;
    private bool isGrounded;
    private float groundCheckRadius = 0.2f;



    // PATRULLA PARAMETROS
    private bool isPaused = false;
    private float pauseTimer = 0f;

    [SerializeField] private int patrolSpeed;
    [SerializeField] private float pauseChance = 0.1f; // 10%
    [SerializeField] private float minPauseDuration = 1f;
    [SerializeField] private float maxPauseDuration = 2f;
    [SerializeField] private float flipChance = 0.3f; // 30%

    private Transform playerTarget;
    [SerializeField] private float chaseSpeed;
    private float attackTimer = 0f;
    public float attackRange = 0.75f;
    public float attackCooldown = 1f;
    private bool isAttacking = false;
    public float attackWindup = 0.5f;
    public float lungeSpeed = 8f;
    public float lungeDuration = 0.2f;

    private bool movingRight = true;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (isAttacking) return;

        CheckGround();
        if (CanSeePlayer())
        {
                ChasePlayer();
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
        Collider2D hit = Physics2D.OverlapCircle(watchSpot.position, 3, entityLayer);
        if (hit != null && hit.gameObject.CompareTag("Player"))
        {
            Vector2 dir = (hit.transform.position - transform.position).normalized;
            float distance = Vector2.Distance(hit.transform.position, transform.position);

            RaycastHit2D visionHit = Physics2D.Raycast(transform.position, dir, distance, groundLayer);
            if(visionHit.collider == null)
            {
                playerTarget = hit.transform;
                return true;
            }
        }
        return false;
    }

    private void ChasePlayer()
    {
        if (playerTarget == null) return;

        attackTimer -= Time.deltaTime;

        float dirToPlayer = playerTarget.position.x - transform.position.x;
        float moveDir = Mathf.Sign(dirToPlayer);

        if ((moveDir > 0 && !movingRight) || (moveDir < 0 && movingRight))
            Flip();

        rb.linearVelocity = new Vector2(moveDir * chaseSpeed, rb.linearVelocity.y);

        if (Mathf.Abs(dirToPlayer) <= attackRange && attackTimer <= 0f)
        {
            StartCoroutine(PerformAttack(moveDir));
            return;
        }
    }

    private IEnumerator PerformAttack(float direction)
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(attackWindup);

        float timer = 0f;
        while (timer < lungeDuration)
        {
            rb.linearVelocity = new Vector2(direction * lungeSpeed, rb.linearVelocity.y);
            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        attackTimer = attackCooldown;
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
        Gizmos.DrawWireSphere(watchSpot.position, 3);
    }

}
