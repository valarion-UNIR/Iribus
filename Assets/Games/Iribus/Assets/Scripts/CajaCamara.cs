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
                    puntoDrc.enabled = false;
                    puntoIzq.enabled = true;
                    pointManager.SetActiveCMCamera(puntoIzq);
                }
                else
                {
                    puntoIzq.enabled = false;
                    puntoDrc.enabled = true;
                    pointManager.SetActiveCMCamera(puntoDrc);
                }
            }
            else
            {
                if (exitingObjectPosition.y < triggerCenterPosition.y)
                {
                    //camara.Follow = puntoIzq;
                }
                else
                {
                    //camara.Follow = puntoDrc;
                }
            }

        }
    }
}