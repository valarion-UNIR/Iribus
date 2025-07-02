using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] private GameObject healthBall;
    [SerializeField] private GameObject firecrackerBall;
    [SerializeField] private GameObject joystickBall;
    [SerializeField] private int objectsToSpawn;
    [SerializeField] private float spawnDelay = 0.3f;
    void Start()
    {
        StartCoroutine(SpawnBallsCoroutine(1f));
    }

    //private void Spawn()
    //{
    //    for (int i = 0; i < objectsToSpawn; ++i)
    //    {
    //        GameObject helth = Instantiate(healthBall, transform.position, Quaternion.identity);
    //        Debug.Log(helth);
    //        GameObject amm = Instantiate(firecrackerBall, transform.position, Quaternion.identity);
    //        Debug.Log(amm);
    //    }

    //    if (!ProgressManager.Instance.isJoystickPicked())
    //    {
    //        Instantiate(joystickBall, transform.localPosition, Quaternion.identity);
    //    }
    //}

    private IEnumerator SpawnBallsCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

        for (int i = 0; i < objectsToSpawn; ++i)
        {
            offset = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
            randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            GameObject health = Instantiate(healthBall, transform.position + offset, randomRotation);
            health.transform.localScale = new Vector3(0.285f / 4, 0.285f / 4, 0.285f / 4);
            Debug.Log(health);

            yield return new WaitForSeconds(spawnDelay);

            offset = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
            randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            GameObject ammo = Instantiate(firecrackerBall, transform.position + offset, randomRotation);
            ammo.transform.localScale = new Vector3(0.285f / 4, 0.285f / 4, 0.285f / 4);
            Debug.Log(ammo);

            yield return new WaitForSeconds(spawnDelay);
        }

        if (!ProgressManager.Instance.isJoystickPicked())
        {   
            GameObject unlock = Instantiate(joystickBall, transform.position + offset, randomRotation);
            unlock.transform.localScale = new Vector3(0.285f / 4, 0.285f / 4, 0.285f / 4);
        }
    }
}
