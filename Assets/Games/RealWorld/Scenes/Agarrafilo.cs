using UnityEngine;

public class Agarrafilo : MonoBehaviour, IAgarrar
{
    public string itemID = "Objeto";
    public string InteractuarID => itemID;
    public GameObject GameObject => gameObject;

    public bool Combinar(IAgarrar objeto)
    {
        throw new System.NotImplementedException();
    }

    public void Interactuar()
    {
        throw new System.NotImplementedException();
    }

    public void Usar()
    {
        throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
