using UnityEngine;

public class LostTile : MonoBehaviour
{
    [SerializeField] private float timeToDisappear = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, timeToDisappear);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
