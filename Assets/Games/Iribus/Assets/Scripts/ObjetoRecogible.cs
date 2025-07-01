using UnityEngine;
using UnityEngine.Events;

public class ObjetoRecogible : MonoBehaviour
{
    [SerializeField] UnityEvent evento;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GetComponent<Collider2D>().enabled = false;
            GameManagerMetroidvania.Instance.SubirVidaMaxima();
            evento.Invoke();
            Destroy(gameObject);
        }
    }
}
