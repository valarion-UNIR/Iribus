using UnityEngine;

public class FirstPersonInteract : RealWorldSubGamePlayerController
{
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
    }
}
