using UnityEngine;
using UnityEngine.Events;

public class InteractableEvento : MonoBehaviour, IInteractMV
{
    public GameObject GameObject => gameObject;

    [SerializeField] private UnityEvent evento;

    [SerializeField] private bool repetible;

    public void InteractMV()
    {
        if(!repetible)
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
        evento.Invoke();
    }

    public void Highlight()
    {
        Debug.Log("Jilightiaron");
    }

    public void Unhighlight()
    {
        Debug.Log("NOOOO Unjiligtiaron");
    }
}
