using UnityEngine;

public class PachinkoRowMovement : MonoBehaviour
{
    [Header("Limits")]
    [SerializeField] private float leftLimit;
    [SerializeField] private float rightLimit;

    [Header("Configuration")]
    [SerializeField] private float speed;
    [SerializeField] private bool firstMoveLeft;
    [SerializeField] private bool canMove;
    private int direction; // 1 = derecha y -1 = izquierda


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (firstMoveLeft)
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            Movement();
        }
    }

    private void Movement()
    {
        transform.localPosition += Vector3.right * direction * speed * Time.deltaTime;

        if (transform.localPosition.x >= rightLimit)
        {
            direction = -1;
        }
        else if (transform.localPosition.x <= leftLimit)
        {
            direction = 1;
        }
    }
}
