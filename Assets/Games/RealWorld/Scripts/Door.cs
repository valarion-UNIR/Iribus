using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
public class Door : MonoBehaviour, IInteract
{
    [SerializeField] private GameObject pivot;
    [SerializeField] private bool openable = true;
    [SerializeField] private float openRotation = 90;
    [SerializeField] private float rotationSpeed = 1;

    private bool openning = false;
    private bool opened = false;
    private float currentRotation = 0;
    private Outline outline;

    private void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public bool CanOpen()
    {
        return openable;
    }

    private void Update()
    {
        if (openning && !opened)
        {
            currentRotation += Time.deltaTime * rotationSpeed;
            if (currentRotation > 1)
                currentRotation = 1;

            pivot.transform.rotation = Quaternion.Euler(0, Mathf.Sign(transform.localScale.z) * Mathf.Lerp(0, openRotation, Mathf.Min(currentRotation, 1)), 0);
        }
    }

    public void Interact()
    {
        if (openable)
            openning = true;
    }

    public void Hightlight(bool hightlight)
    {
        outline.enabled = hightlight && SubGamePlayerControlManager.instance.CurrentGame == SubGame.RealWorld;
    }
}
