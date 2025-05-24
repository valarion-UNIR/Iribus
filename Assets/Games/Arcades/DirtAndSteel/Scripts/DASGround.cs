using UnityEngine;

public class DASGround : MonoBehaviour
{
    [SerializeField] private float speedLimit = 20f;
    [SerializeField] private float slowDownTime = 0.8f;


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            collision.gameObject.GetComponent<DASPlayerController>().LimitMaxSpeed(speedLimit, slowDownTime);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            collision.gameObject.GetComponent<DASPlayerController>().SetBaseMaxSpeed();
    }
}
