using Unity.Cinemachine;
using UnityEngine;

public class CajaCamara : MonoBehaviour
{
    [SerializeField] private CinemachineCamera puntoIzq;
    [SerializeField] private CinemachineCamera puntoDrc;
    [SerializeField] private bool vertical = false;

    [SerializeField] CheckPointManager pointManager;

    //private void Start()
    //{
    //    if (puntoIzq == null)
    //    {
    //        puntoIzq = GameManager.Instance.GetPlayerCamera();
    //    }
    //    else if (puntoDrc == null)
    //    {
    //        puntoDrc = GameManager.Instance.GetPlayerCamera();
    //    }
    //}

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Vector2 exitingObjectPosition = other.transform.position;
            Vector2 triggerCenterPosition = transform.position;

            if (!vertical)
            {
                if (exitingObjectPosition.x < triggerCenterPosition.x)
                {
                    if(puntoIzq.Follow != null)
                    {
                        puntoIzq.Follow = other.transform;
                        Debug.Log("Cambiando follow");
                    }
                    puntoDrc.enabled = false;
                    puntoIzq.enabled = true;
                    pointManager.SetActiveCMCamera(puntoIzq);
                }
                else
                {
                    if (puntoDrc.Follow != null)
                    {
                        puntoDrc.Follow = other.transform;
                        Debug.Log("Cambiando follow");
                    }
                    puntoIzq.enabled = false;
                    puntoDrc.enabled = true;
                    pointManager.SetActiveCMCamera(puntoDrc);
                }
            }
            else
            {
                if (exitingObjectPosition.y < triggerCenterPosition.y)
                {
                    if (puntoIzq.Follow != null)
                    {
                        puntoIzq.Follow = other.transform;
                        Debug.Log("Cambiando follow");
                    }
                    puntoDrc.enabled = false;
                    puntoIzq.enabled = true;
                    pointManager.SetActiveCMCamera(puntoIzq);
                }
                else
                {
                    if (puntoDrc.Follow != null)
                    {
                        puntoDrc.Follow = other.transform;
                        Debug.Log("Cambiando follow");
                    }
                    puntoIzq.enabled = false;
                    puntoDrc.enabled = true;
                    pointManager.SetActiveCMCamera(puntoDrc);
                }
            }
        }
    }
}