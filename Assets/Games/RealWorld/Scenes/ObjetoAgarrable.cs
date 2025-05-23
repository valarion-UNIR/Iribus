using UnityEngine;

public class ObjetoAgarrable : MonoBehaviour, IAgarrar
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

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
