using UnityEngine;
using System.Collections;

public class TrampaBolita : MonoBehaviour
{
    [SerializeField] private BoxCollider2D trampaCollider;
    [SerializeField] private GameObject bolitaPrefab;

    private GameObject bolitaActiva;

    void Start()
    {
        RespawnBolita();
    }

    void Update()
    {
        
    }

    private void ActivarTrampa()
    {
        Debug.Log("Trampa bolita activada");
        bolitaActiva.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        trampaCollider.enabled = false;
        bolitaActiva.transform.parent = null;
        bolitaActiva = null;

        StartCoroutine(CooldownBolita());
    }

    private IEnumerator CooldownBolita()
    {
        yield return new WaitForSeconds(5f);
        RespawnBolita();
        trampaCollider.enabled = true;
    }

    private void RespawnBolita()
    {
        if (bolitaActiva != null) return;
        bolitaActiva = Instantiate(bolitaPrefab, transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            ActivarTrampa();
        }
    }
}
