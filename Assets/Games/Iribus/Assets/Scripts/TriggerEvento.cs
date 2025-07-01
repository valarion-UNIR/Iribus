using UnityEngine;
using UnityEngine.Events;

public class TriggerEvento : MonoBehaviour
{
    [SerializeField] UnityEvent evento;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            evento.Invoke();    
        }
    }
}
