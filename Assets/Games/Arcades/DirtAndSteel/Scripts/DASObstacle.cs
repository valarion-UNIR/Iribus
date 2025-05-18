using UnityEngine;

public class DASObstacle : MonoBehaviour
{

    [SerializeField] private float slowDownPercentage = 0.8f;
    [SerializeField] private float slowDownDuration = 0.8f;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            StartCoroutine(collision.gameObject.GetComponent<DASPlayerController>().ApplySuddenVelocityChange(slowDownPercentage, slowDownDuration));
            DASGameManager.Instance.CameraShakeStandard();
        }
    }
}
