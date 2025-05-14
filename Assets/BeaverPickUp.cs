using UnityEngine;

public class BeaverPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Beaver"))
        {
            //Unlockear lo que sea
            Destroy(gameObject);
        }
    }
}
