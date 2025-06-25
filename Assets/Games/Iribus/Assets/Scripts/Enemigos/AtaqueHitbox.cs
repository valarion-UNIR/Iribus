using UnityEngine;

public class AtaqueHitbox : MonoBehaviour
{
    public PlayerMovement pMovement;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();    
    }

    private void OnEnable()
    {
        animator.SetTrigger("Slash");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemigoMV"))
        {
            if(collision.TryGetComponent<IHitteable>(out IHitteable hitteable))
            {
                float knockback = hitteable.GetHit(transform.up, DamageTypes.MELEE);
                pMovement.ApplyKnockback(transform.up, knockback);
            }
        }
    }
}
