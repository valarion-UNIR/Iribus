using UnityEngine;

public class EnemigoCuerpo : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponentInParent<PlayerMovement>().GetHurt(transform);
        }
    }
}