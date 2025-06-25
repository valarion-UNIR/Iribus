using UnityEngine;

public class EnemigoCuerpoSecundario : MonoBehaviour, IHitteable
{
    [SerializeField] private int hp = 3;
    [SerializeField] private ParticleSystem hurtParticles;
    [SerializeField] private float knockBackForce = 10f;
    [SerializeField] private float knockBackPlayer = 25f;

    [SerializeField] private Rigidbody2D rb;
    private IEnemigoMultiparte enemigoOriginal;

    void Start()
    {
        if(rb == null) {  rb = GetComponent<Rigidbody2D>(); }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerMovement>().GetHurt(transform);
        }
    }

    public float GetHit(Vector3 direction, DamageTypes dmgType)
    {
        hp = hp - 1;
        if (hp <= 0)
        {
            //GameManagerMetroidvania.Instance.GetParticleSpawner().SpawnByIndex(2, transform.position, transform.rotation);
            enemigoOriginal.ParteEliminated(this);
            Destroy(gameObject);
        }
        hurtParticles.transform.rotation = Quaternion.LookRotation(-direction);
        hurtParticles.Play();
        rb.AddForce(-direction * knockBackForce, ForceMode2D.Impulse);
        return knockBackPlayer;
    }

    public void SetMainCuerpo(IEnemigoMultiparte enemigoOriginal)
    {
        this.enemigoOriginal = enemigoOriginal;
    }
}
