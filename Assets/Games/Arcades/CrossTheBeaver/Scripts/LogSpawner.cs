using System.Collections;
using UnityEngine;

public class LogSpawner : MonoBehaviour
{
    [SerializeField] private float logSpeed = 1;
    [SerializeField] GameObject Log;
    [SerializeField] bool logRight = true;
    [SerializeField] float timeBetweenLog = 1;

    private void Start()
    {
        StartCoroutine(SpawnLog(timeBetweenLog));
    }
    private void IncreaseSpawnRate(float augment)
    {
        logSpeed += augment;
    }
    IEnumerator SpawnLog(float cooldown)
    {
        while(true) 
        {
            GameObject spawnedLog = Instantiate(Log, transform.position, Quaternion.identity);
            LogBehaviour logBehaviour = spawnedLog.GetComponent<LogBehaviour>();
            logBehaviour.Speed = logSpeed;
            logBehaviour.Right = logRight;

            yield return new WaitForSeconds(timeBetweenLog);
        }
    }
}
