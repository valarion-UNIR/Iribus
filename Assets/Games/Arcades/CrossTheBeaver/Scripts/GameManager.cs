using UnityEngine;

public class BeaverManager : MonoBehaviour
{
    public static BeaverManager Instance { get; private set; }


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }



}
