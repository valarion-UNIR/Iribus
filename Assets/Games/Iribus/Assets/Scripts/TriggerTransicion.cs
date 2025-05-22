using UnityEngine;

public class TriggerTransicion : MonoBehaviour
{
    [SerializeField] int escenaATransicionar = 0;
    [SerializeField] int entrada = 2;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("HOLAAA????");
            if (GameManagerMetroidvania.Instance.CurrentState != GameState.Playing) return;
            GameManagerMetroidvania.Instance.CargarSiguienteEscena(escenaATransicionar, entrada);
        }
    }
}
