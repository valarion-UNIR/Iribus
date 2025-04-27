using UnityEngine;

public class FirstPersonInteract : RealWorldSubGamePlayerController
{
    void Update()
    {
        if (Input.Player.Next.triggered)
            SubGamePlayerControlManager.instance.CurrentGame = SubGame.Iribus;
    }
}
