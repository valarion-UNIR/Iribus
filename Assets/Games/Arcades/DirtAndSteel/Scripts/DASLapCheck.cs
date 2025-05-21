using UnityEngine;

public class DASLapCheck : MonoBehaviour
{

    [HideInInspector] public DASLapManager lapManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            lapManager.CheckLapPoint(this);
    }
}
