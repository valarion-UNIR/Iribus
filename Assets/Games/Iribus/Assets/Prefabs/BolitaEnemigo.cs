using UnityEngine;

public class BolitaEnemigo : MonoBehaviour
{
    private CircleCollider2D circleCollider;
    private Rigidbody2D rb;
    private bool activated = false;

    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (activated)
        {
            rb.linearVelocity = new Vector2(5f, 0f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            activated = true;
        }
        else if (collision.gameObject.layer == 3)
        {
            Destroy(gameObject);
        }
    }
}
