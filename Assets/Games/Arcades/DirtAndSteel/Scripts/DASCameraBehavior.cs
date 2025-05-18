using UnityEngine;
using Unity.Cinemachine;
using System.Collections;


public class DASCameraBehavior : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private CinemachineFollow cinemachineFollow;
    [SerializeField] private DASPlayerController player;

    [Header("Zoom Settings")]
    [SerializeField] private float minZoom = 5f; // Zoomed-in when slow
    [SerializeField] private float maxZoom = 10f; // Zoomed-out when fast
    [SerializeField] private float zoomLerpSpeed = 3f;

    [SerializeField] private float cameraOffsetAmmount = 3f;
    [SerializeField] private float cameraDampingAmmount = 3f;

    private Vector3 currentOffset;

    private CinemachineBasicMultiChannelPerlin noise;

    private float shakeTimer = 0f;
    private float initialAmplitude = 0f;
    private float initialFrequency = 0f;
    private Coroutine shakeCoroutine;

    void Start()
    {
        currentOffset = cinemachineFollow.FollowOffset;

        noise = gameObject.GetComponent<CinemachineBasicMultiChannelPerlin>();
        initialAmplitude = noise.AmplitudeGain;
        initialFrequency = noise.FrequencyGain;
    }

    void Update()
    {
        float speed = player.carRigidBody.linearVelocity.magnitude;

        // Normalize speed to 0-1
        float t = Mathf.Clamp01(speed / player.maxSpeed);

        // Interpolate zoom based on speed
        float targetZoom = Mathf.Lerp(minZoom, maxZoom, t);

        // Smooth transition
        float currentZoom = cinemachineCamera.Lens.OrthographicSize;
        cinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomLerpSpeed);

        Vector3 targetOffset = new Vector3(0f, 0f, cinemachineFollow.FollowOffset.z);
        
        if(player.accelInput != 0 || player.steerInput != 0)
            targetOffset = new Vector3(cameraOffsetAmmount * player.currentDirection.x, cameraOffsetAmmount * player.currentDirection.y, cinemachineFollow.FollowOffset.z);

        if (player.accelInput == -1)
        {
            targetOffset.x *= -1;
            targetOffset.y *= -1;
        }

        currentOffset = Vector3.Lerp(currentOffset, targetOffset, Time.deltaTime * cameraDampingAmmount);
        cinemachineFollow.FollowOffset = currentOffset;
    }

    public void TriggerShake(float duration, float magnitude, float frequency)
    {
        if(shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, magnitude, frequency));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude, float frequency)
    {
        if (noise == null)
            yield break;

        noise.AmplitudeGain = magnitude;
        noise.FrequencyGain = frequency;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float easeOut = 1f - Mathf.Pow(t, 2); // quadratic ease-out

            noise.AmplitudeGain = magnitude * easeOut;
            noise.FrequencyGain = frequency * easeOut;

            elapsed += Time.deltaTime;
            yield return null;
        }

        noise.AmplitudeGain = initialAmplitude;
        noise.FrequencyGain = initialFrequency;
    }

    public void TriggerStandardShake()
    {
        TriggerShake(0.5f, 10f, 10f);
    }

}
