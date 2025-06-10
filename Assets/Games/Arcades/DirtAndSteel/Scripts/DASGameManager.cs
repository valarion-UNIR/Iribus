using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.InputSystem.Utilities;

public class DASGameManager : SubGamePlayerController
{
    public override SubGame SubGame => SubGame.DirtAndSteel;

    [SerializeField] private TextMeshProUGUI raceTimerText;
    [SerializeField] private DASCameraBehavior cameraBehavior;
    [SerializeField] private DASPlayerController playerController;

    [SerializeField] private float raceInitTime;
    private float raceTimer;

    private Vector3 playerInitPosition;
    private Quaternion playerInitRotation;
    public static DASGameManager Instance { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        playerInitPosition = playerController.transform.position;
        playerInitRotation = playerController.transform.rotation;
    }

    void Start()
    {
        raceTimer = raceInitTime;
    }


    void Update()
    {
        if (Input.DirtAndSteel.Restart.IsPressed())
            RestartRace();

        raceTimer -= Time.deltaTime;

        FormatTimer();
    }

    private void FormatTimer()
    {
        if (raceTimer < 0)
            raceTimer = 0;

        int minutes = (int)(raceTimer / 60);
        int seconds = (int)(raceTimer % 60);
        int milliseconds = (int)((raceTimer * 1000) % 1000);

        raceTimerText.text = string.Format("{0:D2}:{1:D2}:{2:D3}", minutes, seconds, milliseconds);
    }

    public void AddRaceTime(float ammount)
    {
        raceTimer += ammount;
    }

    public void LapCheckReached()
    {
        print(raceTimer);
    }

    public void CameraShakeStandard()
    {
        cameraBehavior.TriggerStandardShake();
    }

    public void CameraShake(float duration, float magnitude, float frequency)
    {
        cameraBehavior.TriggerShake(duration, magnitude, frequency);
    }

    public void RestartRace()
    {
        raceTimer = raceInitTime;

        playerController.transform.position = playerInitPosition;
        playerController.transform.rotation = playerInitRotation;
        playerController.carRigidBody.linearVelocity = Vector2.zero;
        playerController.carRigidBody.angularVelocity = 0f;
    }
}
