using UnityEngine;
using TMPro;
using UnityEngine.Audio;

public class DASGameManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI raceTimerText;

    [SerializeField] private float raceInitTime;
    private float raceTimer;

    public static DASGameManager Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        raceTimer = raceInitTime;
    }


    void Update()
    {
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
}
