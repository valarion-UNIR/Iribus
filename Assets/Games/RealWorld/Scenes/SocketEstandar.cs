using System;
using System.Collections;
using UnityEngine;

public class SocketEstandar : MonoBehaviour, ISocket
{
    public string itemID = "Socket";
    public string InteractuarID => itemID;
    public GameObject GameObject => gameObject;

    public Transform sitioSocket;

    public void Interactuar()
    {
        throw new System.NotImplementedException();
    }

    public bool PlaceObject(IAgarrar objeto)
    {
        Debug.Log("Se intenta placear");
        if (objeto.InteractuarID.Equals(InteractuarID))
        {
            objeto.GameObject.transform.parent = sitioSocket;
            StartCoroutine(PlaceInSocketCoroutine(objeto));
            return true;
        }
        return false;
    }

    private IEnumerator PlaceInSocketCoroutine(IAgarrar objeto)
    {
        Transform objTransform = objeto.GameObject.transform;
        while (Vector3.Distance(objTransform.localPosition, Vector3.zero) > 0.05f)
        {
            objTransform.localPosition = Vector3.MoveTowards(objTransform.localPosition, Vector3.zero, 10f * Time.deltaTime);
            yield return null;
        }
        objTransform.localPosition = Vector3.zero;
    }


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
