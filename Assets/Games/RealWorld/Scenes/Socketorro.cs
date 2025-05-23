using System;
using System.Collections;
using UnityEngine;

public class Socketorro : MonoBehaviour, ISocket
{
    public string itemID = "Socket";
    public string InteractuarID => itemID;
    public GameObject GameObject => gameObject;

    public Transform sitioSocket;

    public void Interactuar()
    {
        throw new System.NotImplementedException();
    }

    public bool PlacearObjeto(IAgarrar objeto)
    {
        Debug.Log("Se intenta placear");
        if (objeto.InteractuarID.Equals("Zeporro"))
        {
            objeto.GameObject.transform.parent = sitioSocket;
            StartCoroutine(DejarEnSocket(objeto));
            return true;
        }
        return false;
    }

    private IEnumerator DejarEnSocket(IAgarrar objeto)
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
