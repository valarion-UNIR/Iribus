using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CuerdaEnemigo : MonoBehaviour
{
    [SerializeField] private Transform topAnchor;
    [SerializeField] private Transform bottomAnchor;
    private LineRenderer lr;
    [SerializeField] private GameObject particlesBreak;
    [SerializeField] private float breakDuration = 1.5f;
    [SerializeField] private int particleBursts = 10;

    private bool isBreaking = false;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();

        lr.positionCount = 2;
    }

    private void LateUpdate()
    {
        if (isBreaking) return;

        lr.SetPosition(0, topAnchor.position);
        lr.SetPosition(1, bottomAnchor.position);
    }

    public void BreakRope()
    {
        if (!isBreaking) StartCoroutine(ShrinkRopeAndSpawnParticles());
    }

    private IEnumerator ShrinkRopeAndSpawnParticles()
    {
        isBreaking = true;

        float timer = 0f;
        Vector3 start = topAnchor.position;
        Vector3 end = bottomAnchor.position;

        int burstCount = 0;

        while (timer < breakDuration)
        {
            float t = timer / breakDuration;

            Vector3 currentTop = Vector3.Lerp(start, end, t);
            lr.SetPosition(0, end);
            lr.SetPosition(1, currentTop);

            float expectedBursts = Mathf.FloorToInt(t * particleBursts);
            if (expectedBursts > burstCount)
            {
                Instantiate(particlesBreak, currentTop, Quaternion.identity);
                burstCount++;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        lr.enabled = false;

        Destroy(lr.gameObject); 
        // Destroy(this);
    }
}
