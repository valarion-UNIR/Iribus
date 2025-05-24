using UnityEngine;

[RequireComponent(typeof(Outline))]
public class GrabbableObject : MonoBehaviour, IGrab
{
    [SerializeField] private string itemID;

    public string InteractId => itemID;
    public GameObject GameObject => gameObject;

    private Outline outline;

    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public bool Combine(IGrab objeto)
    {
        return false;
    }

    public void Hightlight(bool hightlight)
    {
        outline.enabled = hightlight && SubGamePlayerControlManager.instance.CurrentGame == SubGame.RealWorld;
    }

    public void Interact()
    {

    }

    public void Use()
    {

    }
}
