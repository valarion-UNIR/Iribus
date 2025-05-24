using UnityEngine;

public class DASTurbo : MonoBehaviour
{

    [SerializeField] private float turboSpeedMultiplier = 1.25f;
    [SerializeField] private float turboDuration = 0.8f;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            collision.gameObject.GetComponent<DASPlayerController>().ApplySuddenVelocityChange(turboSpeedMultiplier, turboDuration);
    }
}
