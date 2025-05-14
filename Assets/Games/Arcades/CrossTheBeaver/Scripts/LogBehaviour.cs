using System.Collections;
using UnityEngine;

public class LogBehaviour : MonoBehaviour
{
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private bool right = true;

    private Vector3 lastPosition;
    public Vector3 DeltaPosition { get; private set; }
    public float Speed { get => speed; set => speed = value; }
    public bool Right { get => right; set => right = value; }

    private void Start()
    {
        lastPosition = transform.position;
        StartCoroutine(Inmolate(30/Speed + 1));
    }
    private void FixedUpdate()
    {
        if (right) { transform.position = transform.position + new Vector3(1, 0, 0) * speed * Time.deltaTime; }
        else { transform.position = transform.position + new Vector3(-1, 0, 0) * speed * Time.deltaTime; }

        DeltaPosition = transform.position - lastPosition;
        lastPosition = transform.position;
    }

    IEnumerator Inmolate(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
