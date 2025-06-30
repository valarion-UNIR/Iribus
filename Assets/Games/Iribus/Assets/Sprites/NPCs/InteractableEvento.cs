using UnityEngine;
using UnityEngine.Events;

public class InteractableEvento : MonoBehaviour, IInteractMV
{
    public GameObject GameObject => gameObject;

    [SerializeField] private UnityEvent evento;

    [SerializeField] private bool repetible;
    [SerializeField] GameObject interactCosa;

    public void InteractMV()
    {
        if (!repetible)
        {
            interactCosa.SetActive(false);
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
        evento.Invoke();
    }

    public void Highlight()
    {
        interactCosa.SetActive(true);
        Debug.Log("Jilightiaron");
    }

    public void Unhighlight()
    {
        interactCosa.SetActive(false);
        Debug.Log("NOOOO Unjiligtiaron");
    }
}
