using UnityEngine;

public class BeaverCameraController : MonoBehaviour
{
    Vector3 startPosition;
    private void Start()
    {
        startPosition = transform.position;
    }
    public void GoUp(float distance)
    {
        Debug.Log("Subre la temperatura");
        transform.position += new Vector3(0, distance, 0);
    }

    public void ResetCam()
    {
        transform.position = startPosition;
    }

}
