using UnityEngine;

public class StackerTile : MonoBehaviour
{
    [SerializeField] private float leftLimit = -14.75f;
    [SerializeField] private float rightLimit = 14.75f;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float failThreshold;

    [SerializeField] private GameObject lostTile;

    private float elapsed;
    private static Vector3 startPosition;
    private float distance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        MovementTile();
    }

    private void MovementTile()
    {
        elapsed += Time.deltaTime;
        float t = Mathf.PingPong(elapsed * moveSpeed, 1f);
        float x = Mathf.Lerp(leftLimit, rightLimit, t);
        distance = x - startPosition.x;
        transform.position = new Vector3(x, startPosition.y, startPosition.z);
    }


    public void ScaleTile()
    {
        if (Mathf.Abs(distance) > failThreshold)
        {
            float lostLength = Mathf.Abs(distance);
            if (transform.localScale.x < lostLength)
            {
                gameObject.AddComponent<Rigidbody>();
                StackerSpawner.instance.gameOverBool = true;
                StackerSpawner.instance.GameOver();
                return;
            }

            GameObject _lostTile = Instantiate(lostTile, transform);
            _lostTile.transform.localScale = new Vector3(lostLength, transform.localScale.y, transform.localScale.z);
            _lostTile.transform.position = new Vector3(transform.position.x
                + (distance > 0 ? 1 : -1) * (transform.localScale.x - lostLength) / 2,
                transform.position.y, transform.position.z);
            _lostTile.GetComponent<MeshRenderer>().material.SetColor("_OutlineColor",
                GetComponent<MeshRenderer>().material.GetColor("_OutlineColor"));

            transform.localScale -= new Vector3(lostLength, 0, 0);
            transform.Translate((distance > 0 ? -1 : 1) * lostLength / 2, 0, 0);
        }
        else
        {
            transform.Translate(-distance, 0, 0);
        }

        Destroy(this);
    }

    public void SetStats((float, float ) x)
    {
        moveSpeed = x.Item1;
        failThreshold = x.Item2;
    }
}
