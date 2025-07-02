using UnityEngine;

public class PrizeChecker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case ("Health"):
                Debug.Log("Health up");
                ProgressManager.Instance.addHealth(1);
                break;

            case ("Ammo"):
                Debug.Log("Ammo up");
                ProgressManager.Instance.addFirecrackers(1);
                break;

            case ("Unlock"):
                Debug.Log("Unlock");
                ProgressManager.Instance.setJoystickPicked(true);
                break;
        }

        Destroy(other.gameObject);
    }

}
