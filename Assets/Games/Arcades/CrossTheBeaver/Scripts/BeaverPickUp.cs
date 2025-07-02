using UnityEngine;

public class BeaverPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Beaver"))
        {
            ProgressManager.Instance.setMeleeUnlocked(true);
            Destroy(gameObject);
        }
    }
}
