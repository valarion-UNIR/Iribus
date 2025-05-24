using System.Collections.Generic;
using UnityEngine;

public class DASSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject prefab;
    public Transform outTunnel;

    [Header("Spawning")]
    public float spawnInterval = 1.5f;
    public float obstacleSpeed = 5f;

    private List<GameObject> pool = new List<GameObject>();
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            Spawn();
        }
    }

    void Spawn()
    {
        GameObject obj = GetFromPool();
        obj.transform.position = transform.position;

        Vector2 direction = (outTunnel.position - obj.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        obj.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        var obstacle = obj.GetComponent<DASObstacleSpawned>();
        obstacle.SetTarget(outTunnel.position);
        obstacle.SetSpeed(obstacleSpeed);

        obj.SetActive(true);
    }


    GameObject GetFromPool()
    {
        pool.RemoveAll(obj => obj == null);

        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
                return obj;
        }

        GameObject newObj = Instantiate(prefab, transform);
        newObj.SetActive(false);
        pool.Add(newObj);
        return newObj;
    }
}
