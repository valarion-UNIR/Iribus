using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private float mouseSensitivity = 1f;
    [SerializeField]
    private float smoothing = 2f;
    [SerializeField]
    private bool invertY = false;
    [SerializeField]
    private Vector3 cameraOffset = new Vector3(0, 0.9f, 0);

    private Vector2 mouseDelta;
    private Vector2 smoothedMouse;
    private Camera realCamera;

    private float currentYaw = 0f;
    private float currentPitch = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        realCamera = GetComponent<Camera>();
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        mouseDelta.x = mouseX * mouseSensitivity;
        mouseDelta.y = mouseY * mouseSensitivity * (invertY ? 1f : -1f);

        if (smoothing > 0f)
        {
            smoothedMouse.x = Mathf.Lerp(smoothedMouse.x, mouseDelta.x, 1f / smoothing);
            smoothedMouse.y = Mathf.Lerp(smoothedMouse.y, mouseDelta.y, 1f / smoothing);
        }
        else
        {
            smoothedMouse = mouseDelta;
        }

        currentYaw += smoothedMouse.x;
        currentPitch += smoothedMouse.y;
        currentPitch = Mathf.Clamp(currentPitch, -90f, 90f);
    }

    void LateUpdate()
    {
        if (playerTransform != null)
        {
            transform.position = playerTransform.position + cameraOffset;
        }
        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
    }

    void OnApplicationFocus(bool hasFocus)
    {
    if (hasFocus)
        Cursor.lockState = CursorLockMode.Locked;
    else
        Cursor.lockState = CursorLockMode.None;
    }

    public float GetCurrentYaw()
    {
        return currentYaw;
    }

    public Camera GetCamera()
    {
        return realCamera;
    }
}