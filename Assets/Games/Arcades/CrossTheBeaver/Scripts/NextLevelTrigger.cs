using System;
using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    public static event Action OnNextScreenTriggered;

    private Collider2D collider;
    private BeaverController player;
    private float noReturnPosition;

    private bool triggered = false;

    private void Start()
    {
        collider = GetComponent<Collider2D>();
        player = BeaverManager.Instance.ReturnBeaver();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Beaver") && !triggered) 
        { 
            OnNextScreenTriggered?.Invoke(); 
            Debug.Log("Llega al triger");
            triggered = true;
            noReturnPosition = player.transform.position.y;
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
