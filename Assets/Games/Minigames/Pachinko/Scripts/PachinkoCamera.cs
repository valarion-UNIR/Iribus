using UnityEngine;

public class PachinkoCamera : SubGamePlayerController
{
    public override SubGame SubGame => SubGame.Pachinko;

    [SerializeField] private float topLimit;
    [SerializeField] private float bottomLimit;
    [SerializeField] private float speedMovement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        topLimit = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        float inputV = Input.Pachinko.Move.ReadValue<Vector2>().y;

        Vector3 currentPosition = transform.localPosition;
        currentPosition.y += inputV * speedMovement * Time.deltaTime;

        currentPosition.y = Mathf.Clamp(currentPosition.y, bottomLimit, topLimit);

        transform.localPosition = currentPosition;
    }
}
