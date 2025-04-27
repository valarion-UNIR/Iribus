using UnityEngine;

public class GenericFirstPersonInteract : SubGamePlayerController
{
    public override SubGame SubGame => subGame;

    [SerializeField] private SubGame subGame;

    void Update()
    {
        if (Input.Player.Previous.triggered)
            SubGamePlayerControlManager.instance.CurrentGame = SubGame.RealWorld;
    }
}
