using UnityEngine;


public class DASCollisionable : MonoBehaviour
{

    [SerializeField] private float addRaceTimeAmmount;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
            DASGameManager.Instance.AddRaceTime(addRaceTimeAmmount);

    }
}
