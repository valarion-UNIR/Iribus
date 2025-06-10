using UnityEngine;

public class DASLapCheck : MonoBehaviour
{

    [HideInInspector] public DASLapManager lapManager;
    [SerializeField] private float addRaceTimeAmmount;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && enabled)
        {
            lapManager.CheckLapPoint(this);

            DASGameManager.Instance.AddRaceTime(addRaceTimeAmmount);
            enabled = false;
        }
    }
}
