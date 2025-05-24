using UnityEngine;
using System.Collections;

public class DASSlowdownManager : MonoBehaviour
{
    public static DASSlowdownManager Instance { get; private set; }

    [Range(0f, 1f)] public float slowFactor = 1f;
    public bool IsSlowed => slowFactor < 1f;

    private Coroutine activeSlowdown;

    public float DeltaTime => Time.deltaTime * slowFactor;
    public float FixedDeltaTime => Time.fixedDeltaTime * slowFactor;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void StartSlowdown(float factor, float duration)
    {
        if (activeSlowdown != null)
            StopCoroutine(activeSlowdown);

        activeSlowdown = StartCoroutine(SlowdownRoutine(factor, duration));
    }

    private IEnumerator SlowdownRoutine(float factor, float duration)
    {
        slowFactor = Mathf.Clamp01(factor);
        yield return new WaitForSecondsRealtime(duration);
        slowFactor = 1f;
        activeSlowdown = null;
    }
}
