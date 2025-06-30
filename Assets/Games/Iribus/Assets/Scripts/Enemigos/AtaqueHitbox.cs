using Unity.VisualScripting;
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
                //Vector3 directionHit = new Vector3(collision.transform.position.x - pMovement.transform.position.x, collision.transform.position.y - pMovement.transform.position.y, 0);
                float direction = pMovement.transform.localScale.x == 1 ? 1 : -1;
                float knockback = hitteable.GetHit(pMovement.transform.right * -direction, DamageTypes.MELEE);
                pMovement.ApplyKnockback(pMovement.transform.right * -direction, knockback);
            }
        }
    }
}
