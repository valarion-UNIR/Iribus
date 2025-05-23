using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FirstPersonInteract : RealWorldSubGamePlayerController
{
    private readonly HashSet<IInteractable> inTrigger = new();


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out var interact))
        {
            inTrigger.Add(interact);
            if (inTrigger.Count == 1)
                interact.Hightlight(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out var interact))
        {
            if(inTrigger.Count == 1)
                interact.Hightlight(false);

            inTrigger.Remove(interact);

            if (inTrigger.Count == 1)
                inTrigger.First().Hightlight(true);
        }
    }

    void Update()
    {
        if (Input.Subgames.RealWorld.triggered)
            SubGamePlayerControlManager.instance.CurrentGame = SubGame.RealWorld;

        else if (Input.Subgames.Iribus.triggered)
            SubGamePlayerControlManager.instance.CurrentGame = SubGame.Iribus;

        else if (Input.Subgames.CrossTheBeaver.triggered)
            SubGamePlayerControlManager.instance.CurrentGame = SubGame.CrossTheBeaver;

        else if (Input.Subgames.DirtAndSteel.triggered)
            SubGamePlayerControlManager.instance.CurrentGame = SubGame.DirtAndSteel;

        else if (Input.Subgames.TorpedoTrouble.triggered)
            SubGamePlayerControlManager.instance.CurrentGame = SubGame.TorpedoTrouble;

        else if (Input.Subgames.Stacker.triggered)
            SubGamePlayerControlManager.instance.CurrentGame = SubGame.Stacker;

        else if (Input.Subgames.Garra.triggered)
            SubGamePlayerControlManager.instance.CurrentGame = SubGame.Garra;

        else if (Input.Subgames.Pachinko.triggered)
            SubGamePlayerControlManager.instance.CurrentGame = SubGame.Pachinko;

        else if (Input.Player.Attack.triggered)
        {
            if (inTrigger.Count == 1)
                inTrigger.First().Interact();
        }    
    }
}
