using UnityEngine;

public class FirstPersonInteract : RealWorldSubGamePlayerController
{
    [SerializeField] private Camera raycastSource;
    [SerializeField] private float overlapDistance = 1;
    [SerializeField] private float overlapRadius = 0.5f;

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
            var overlaps = Physics.OverlapSphere(raycastSource.transform.position + raycastSource.transform.forward * overlapDistance, overlapRadius);
            foreach(var overlap in overlaps)
            {
                if(overlap.TryGetComponent(out Door door))
                {
                    door.Open();
                }
            }
        }    
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawSphere(raycastSource.transform.position + raycastSource.transform.forward * overlapDistance, overlapRadius);
    }
}
