using UnityEngine;

public class BolitaRojaEnemigo : MonoBehaviour
{
    private CircleCollider2D circleCollider;
    private Rigidbody2D rb;
    public bool activated = false;
    public bool playerInRange = false;

    float currentVelX = 0f;
    float smoothTime = 0.55f;

    private Transform player;

    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (activated)
        {
            if(playerInRange)
            {
                float targetX = player.position.x;
                float newX = Mathf.SmoothDamp(transform.position.x, targetX, ref currentVelX, smoothTime);
                rb.MovePosition(new Vector2(newX, transform.position.y));
            }
            else
            {
                rb.linearVelocity = new Vector2(5f, 0f);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            if(player == null) player = GameManagerMetroidvania.Instance.GetPlayerTransform();
            playerInRange = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 12)
        {
            activated = true;
        }
        else if (collision.gameObject.layer == 3)
        {
            Destroy(gameObject);
        }
    }
}
