using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FirstPersonInteract : RealWorldSubGamePlayerController
{
    [SerializeField] private Transform grabPosition;

    private readonly HashSet<IInteract> inTrigger = new();
    private IGrab grabbedObject;

    private void Start()
    {
        Input.Subgames.Back.performed += (_) => SubGamePlayerControlManager.instance.CurrentGame = SubGame.RealWorld;

        Input.Player.Attack.performed += (_) =>
        {
            if (inTrigger.Count == 1)
            {
                IInteract interactuar = inTrigger.First();

                if (interactuar == null) { return; }

                if (interactuar is IGrab agarrable)
                {
                    if (grabbedObject == null)
                    {
                        GrabObject(agarrable);
                    }
                    else
                    {
                        agarrable.Interact();
                    }
                }
                else if (interactuar is ISocket socket)
                {
                    if (grabbedObject != null)
                    {
                        if (socket.PlaceObject(grabbedObject))
                        {
                            DropObject();
                        }
                    }
                    else
                    {
                        socket.Interact();
                    }
                }
                else
                {
                    interactuar.Interact();
                }
            }
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        var interact = other.GetComponentInParent<IInteract>();
        if (interact != null || other.TryGetComponent(out interact))
            AddToTrigger(interact);
    }

    private void OnTriggerExit(Collider other)
    {
        var interact = other.GetComponentInParent<IInteract>();
        if (interact != null || other.TryGetComponent(out interact))
            RemoveFromTrigger(interact);
    }

    private void AddToTrigger(IInteract interact)
    {
        if (interact == grabbedObject)
            return;

        if (inTrigger.Count == 1)
            inTrigger.First().Hightlight(false);

        inTrigger.Add(interact);

        if (inTrigger.Count == 1)
            interact.Hightlight(true);
    }

    private void RemoveFromTrigger(IInteract interact)
    {
        if (inTrigger.Count == 1)
            interact.Hightlight(false);

        inTrigger.Remove(interact);

        if (inTrigger.Count == 1)
            inTrigger.First().Hightlight(true);
    }

    private void GrabObject(IGrab grab)
    {
        grabbedObject = grab;
        RemoveFromTrigger(grab);
        grabbedObject.GameObject.transform.parent = grabPosition;
        StartCoroutine(GrabObjectCoroutine());
    }

    private void DropObject()
    {
        grabbedObject = null;
    }
    private IEnumerator GrabObjectCoroutine()
    {
        Transform objTransform = grabbedObject.GameObject.transform;
        while (Vector3.Distance(objTransform.localPosition, Vector3.zero) > 0.05f)
        {
            objTransform.localPosition = Vector3.MoveTowards(objTransform.localPosition, Vector3.zero, 10f * Time.deltaTime);
            yield return null;
        }
        objTransform.localPosition = Vector3.zero;
    }
}
