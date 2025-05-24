using UnityEngine;

public class DASObstacleSpawned : DASObstacle
{
    private Vector3 target;
    private float speed = 5f;

    public void SetTarget(Vector3 targetPosition)
    {
        target = targetPosition;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * DASSlowdownManager.Instance.DeltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            gameObject.SetActive(false);
        }
    }
}
