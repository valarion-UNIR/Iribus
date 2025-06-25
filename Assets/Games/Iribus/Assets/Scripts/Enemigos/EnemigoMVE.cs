using UnityEngine;

public abstract class EnemigoMVE : MonoBehaviour, IHitteable
{
    [SerializeField] protected int hp;

    [SerializeField] protected float hurtTime = 0.5f;
    [SerializeField] protected float knockBackPlayer = 25f;
    [SerializeField] protected ParticleSystem hurtParticles;

    protected bool isHurt = false;
    protected Rigidbody2D rb;
    private float hurtTimer;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public abstract float GetHit(Vector3 direction, DamageTypes dmgType);
}
