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
    private float originalFov = 0f;
    private float fovReturnSpeed = 15f;

    [SerializeField]
    private Transform agarrarPlace;

    private IGrab objetoAgarrado; 

    [SerializeField]
    private LayerMask interactLayer;
    [SerializeField]
    private float rangoRaycast = 10f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        realCamera = GetComponent<Camera>();
        originalFov = realCamera.fieldOfView;
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

        if (realCamera.fieldOfView != originalFov)
        {
            realCamera.fieldOfView = Mathf.Lerp(realCamera.fieldOfView, originalFov, Time.deltaTime * fovReturnSpeed);
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            Interactuar();
        }
    }

    private void Interactuar()
    {
        Ray rayo = realCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        Debug.Log("HOLA MAMONGO");
        if (Physics.Raycast(rayo, out hit, rangoRaycast, interactLayer))
        {
            Debug.Log("HOLA BUEMAS");
            IInteract interactuar = hit.collider.GetComponent<IInteract>();

            if(interactuar == null) { return; }

            if (interactuar is IGrab agarrable)
            {
                if(objetoAgarrado == null)
                {
                    AgarrarItem(agarrable);
                }
                else
                {
                    agarrable.Interact();
                }
            }
            else if(interactuar is ISocket socket)
            {
                if (objetoAgarrado != null)
                {
                    if (socket.PlaceObject(objetoAgarrado))
                    {
                        SoltarObjeto();
                    }
                }
                else
                {
                    socket.Interact();
                }
            }
            else
            {
                interactuar.Interact();
            }
        }

    }

    void LateUpdate()
    {
        if (playerTransform != null)
        {
            transform.position = playerTransform.position + cameraOffset;
        }
        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
    }

    private void AgarrarItem(IGrab agarrar)
    {
        objetoAgarrado = agarrar;
        objetoAgarrado.GameObject.transform.parent = agarrarPlace;
        StartCoroutine(GrabObjectCoroutine());
    }

    private void SoltarObjeto()
    {
        objetoAgarrado = null;
    }

    private IEnumerator GrabObjectCoroutine()
    {
        Transform objTransform = objetoAgarrado.GameObject.transform;
        while (Vector3.Distance(objTransform.localPosition, Vector3.zero) > 0.05f)
        {
            objTransform.localPosition = Vector3.MoveTowards(objTransform.localPosition, Vector3.zero, 10f * Time.deltaTime);
            yield return null;
        }
        objTransform.localPosition = Vector3.zero;
    }


    /// <summary>
    /// Cambia el fov y setea la velocidad a la que volvera al fov original.
    /// </summary>
    /// /// <param name="changingFov">Valor del pov a cambiar.</param>
    /// /// <param name="vel">Velocidad a la que vuelve el pov. (default 15)</param>
    public void ShootFov(float changingFov, float vel = 15f)
    {
        realCamera.fieldOfView = changingFov;
        fovReturnSpeed = vel;
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