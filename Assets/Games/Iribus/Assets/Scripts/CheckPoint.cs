using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private ParticleSystem particulasCerrar;
    [SerializeField] private ParticleSystem particulasAbrir;

    [SerializeField] private int transicion = 0;

    private void Start()
    {
        
    }

    public void SpawnearParticulasCerrar()
    {
        particulasCerrar.Play();
    }
    public void SpawnearParticulasAbrir()
    {
        particulasAbrir.Play();
    }

    public int GetTransicion()
    {
        return transicion;
    }
}
