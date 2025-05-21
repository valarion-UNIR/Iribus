using System;
using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{

    public static event Action OnNextScreenTriggered;

    [SerializeField] bool checkpoint = false;
    private BeaverController player;
    private float noReturnPosition;

    private bool triggered = false;

    private void Start()
    {
        player = BeaverManager.Instance.ReturnBeaver();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Beaver") && !triggered) 
        { 
            OnNextScreenTriggered?.Invoke(); 
            triggered = true;
            noReturnPosition = player.transform.position.y;

            if (checkpoint == true)
            {
                //Guardar
            }
        }
    }

    private void FixedUpdate()
    {
        if (triggered)
        {
            float clampedY = Mathf.Clamp(player.transform.position.y, noReturnPosition, 1000);
            player.transform.position = new Vector3(player.transform.position.x, clampedY, player.transform.position.z);
        }
    }
}
