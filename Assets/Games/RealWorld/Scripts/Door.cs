using System.Linq;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject pivot;
    [SerializeField] private bool openable = true;
    [SerializeField] private float openRotation = 90;
    [SerializeField] private float rotationSpeed = 1;

    private bool openning = false;
    private bool opened = false;
    private float currentRotation = 0;

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

            pivot.transform.rotation = Quaternion.Euler(0, Mathf.Lerp(0, openRotation, Mathf.Min(currentRotation, 1)), 0);
        }
    }

    public void Interact()
    {
        if (openable)
            openning = true;
    }

    public void Hightlight(bool hightlight)
    {
        Debug.Log($"Door {this.name} {(hightlight ? "can be opened now" : "no longer can be opened")}");

        foreach(var mesh in GetComponentsInChildren<MeshRenderer>())
        {
            //mesh.
        }
    }
}
