using UnityEngine;

public class DASPlayerController : SubGamePlayerController
{
    public override SubGame SubGame => subGame;

    [SerializeField] private SubGame subGame;

    protected override void Awake()
    {
        base.Awake();
        Input.Enable();
    }

    void Start()
    {
        
    }


    void Update()
    {
        if (Input.DirtAndSteel.Accelerate.inProgress)
            print("lo");
    }
}
