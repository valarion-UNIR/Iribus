using UnityEngine;


using System.Collections.Generic;

public class DASLapManager : MonoBehaviour
{
    [Header("Checkpoints")]
    [Tooltip("Lista de checkpoints en orden")]
    public List<DASLapCheck> lapChecks;


    private int currentCheckpointIndex = 0;
    private int lapCount = 0;

    [Header("Opcional")]
    public int totalLaps = 3;
    public UnityEngine.Events.UnityEvent onLapCompleted;
    public UnityEngine.Events.UnityEvent onRaceFinished;

    private void Start()
    {
        foreach (DASLapCheck check in lapChecks)
        {
            check.lapManager = this;
        }
    }

    public void CheckLapPoint(DASLapCheck lapCheck)
    {
        if (lapCheck == lapChecks[currentCheckpointIndex])
        {
            currentCheckpointIndex += 1;
            DASGameManager.Instance.LapCheckReached();
        }

        if(currentCheckpointIndex == lapChecks.Count)
        {
            lapCount++;
            currentCheckpointIndex = 0;
        }
    }

    public int GetLapCount() => lapCount;
    public int GetCurrentCheckpointIndex() => currentCheckpointIndex;
}

