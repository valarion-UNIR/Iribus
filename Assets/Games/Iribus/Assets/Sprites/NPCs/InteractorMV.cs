using UnityEngine;

public class InteractorMV : MonoBehaviour
{
    private IInteractMV interactable;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("InteractableMV"))
        {
            Debug.Log("Patatas frita1s");
            if (collision.TryGetComponent(out IInteractMV newInteractable))
            {
                if(interactable != null) interactable.Unhighlight();

                Debug.Log("Patatas fritas2");
                interactable = newInteractable;
                Debug.Log("Patatas fritas3");
                interactable.Highlight();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("InteractableMV"))
        {
            if (interactable != null)
            {
                interactable.Unhighlight();
                interactable = null;
            }
        }
    }

    public void Interactuar()
    {
        if (interactable == null) return;
        Debug.Log(interactable.GameObject.name);
        interactable.InteractMV();

    }
}
