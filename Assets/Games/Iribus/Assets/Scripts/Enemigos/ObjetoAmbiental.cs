using System.Collections.Generic;
using UnityEngine;

public class ObjetoAmbiental : MonoBehaviour, IHitteable
{
    [SerializeField] private List<GameObject> partes;

    public float GetHit(Vector3 direction, DamageTypes dmgType)
    {
        foreach(GameObject parte in partes)
        {
            parte.SetActive(true);
            Vector2 fuerza = new Vector2(Random.Range(2, 4), Random.Range(4, 6));
            fuerza.x = fuerza.x * Mathf.Sign(-direction.x);
            parte.GetComponent<Rigidbody2D>().AddForce(fuerza, ForceMode2D.Impulse);
            float torque = Random.Range(0.5f, 1.5f);
            parte.GetComponent<Rigidbody2D>().AddTorque(torque, ForceMode2D.Impulse);
            Destroy(parte, Random.Range(2.5f, 5f));
        }
        GetComponent<SpriteRenderer>().enabled = false;
        return 0f;
    }
}
