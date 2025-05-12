using UnityEngine;

public class BeaverCameraController : MonoBehaviour
{
    public void GoUp(float distance)
    {
        Debug.Log("Subre la temperatura");
        transform.position += new Vector3(0, distance, 0);
    }
}
