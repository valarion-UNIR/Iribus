using UnityEngine;
using Unity.Cinemachine;

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

    void Start()
    {
        currentOffset = cinemachineFollow.FollowOffset;
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
}
