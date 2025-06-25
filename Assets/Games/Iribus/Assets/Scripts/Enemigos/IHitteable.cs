using UnityEngine;

public enum DamageTypes
{
    MELEE,
    PROYECTILE,
    EXPLOSION,
}


public interface IHitteable
{
    public float GetHit(Vector3 direction, DamageTypes dmgType);
}

public interface IEnemigoMultiparte
{
    public void ParteEliminated(EnemigoCuerpoSecundario parte);
}