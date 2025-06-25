using UnityEngine;

public class EnemigoMV4 : MonoBehaviour, IHitteable
{
    [SerializeField] private int hp = 3;

    public float crawlSpeed = 2f;
    public float turnSpeed = 3f;
    public float rotationSpeed = 2f;
    public LayerMask crawlableLayer;
    public float checkDist = 0.2f;

    private Rigidbody2D rb;
    //private Vector2 crawlDir = Vector2.down;
    private Quaternion target;

    private bool rotingo = false;
    public bool isHurt = false;
    private float hurtTimer;
    public float hurtTime = 0.5f;
    [SerializeField] private float knockBackPlayer = 25f;

    [SerializeField] private ParticleSystem hurtParticles;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    void FixedUpdate()
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

        if(rotingo)
        {
            Quaternion current = transform.rotation;

            transform.rotation = Quaternion.RotateTowards(current, target, rotationSpeed * Time.fixedDeltaTime);
            rb.linearVelocity = transform.right * turnSpeed;

            float angleDiff = Quaternion.Angle(current, target);
            if (angleDiff == 0)
            {
                rotingo = false;
            }
            return;
        }

        bool hitGround = Physics2D.Raycast(transform.position, -transform.up, checkDist, crawlableLayer);
                
        if (!hitGround)
        {
            rotingo = true;
            target = transform.rotation * Quaternion.Euler(0f, 0f, -90f);
            target = target.normalized;
        }
        rb.linearVelocity = transform.right * crawlSpeed;
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position - transform.up);
        Gizmos.color = Color.red;
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
        rb.linearVelocity = Vector2.zero;
        hurtTimer = hurtTime;
        isHurt = true;
        return knockBackPlayer;
    }
}