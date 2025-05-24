using UnityEngine;

public class LostTile : MonoBehaviour
{
    [SerializeField] private float timeToDisappear = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals(TagsIribus.SCKFloor))
        {
            Destroy(gameObject);
        }
    }
}
