using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FirstPersonInteract : RealWorldSubGamePlayerController
{
    private readonly HashSet<IInteractable> inTrigger = new();

    private void Start()
    {
        Input.Subgames.Back.performed += (_) => SubGamePlayerControlManager.instance.CurrentGame = SubGame.RealWorld;

        Input.Player.Attack.performed += (_) =>
        {
            if (inTrigger.Count == 1)
                inTrigger.First().Interact();
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        var interact = other.GetComponentInParent<IInteractable>();
        if (interact != null || other.TryGetComponent(out interact))
        {
            if(inTrigger.Count == 1)
                inTrigger.First().Hightlight(false);

            inTrigger.Add(interact);

            if (inTrigger.Count == 1)
                interact.Hightlight(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var interact = other.GetComponentInParent<IInteractable>();
        if (interact != null || other.TryGetComponent(out interact))
        {
            if(inTrigger.Count == 1)
                interact.Hightlight(false);

            inTrigger.Remove(interact);

            if (inTrigger.Count == 1)
                inTrigger.First().Hightlight(true);
        }
    }
}
