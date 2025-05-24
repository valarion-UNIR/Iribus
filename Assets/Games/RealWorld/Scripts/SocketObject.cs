using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class SocketObject : MonoBehaviour, ISocket
{
    public string itemID = "Socket";
    public string InteractId => itemID;
    public GameObject GameObject => gameObject;

    public Transform socketPosition;

    private Outline outline;

    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void Interact()
    {
        //throw new System.NotImplementedException();
    }

    public bool PlaceObject(IGrab objeto)
    {
        Debug.Log("Se intenta placear");
        if (objeto.InteractId.Equals(InteractId))
        {
            objeto.GameObject.transform.parent = socketPosition;
            StartCoroutine(PlaceInSocketCoroutine(objeto));
            return true;
        }
        return false;
    }

    private IEnumerator PlaceInSocketCoroutine(IGrab objeto)
    {
        Transform objTransform = objeto.GameObject.transform;
        while (Vector3.Distance(objTransform.localPosition, Vector3.zero) > 0.05f)
        {
            objTransform.localPosition = Vector3.MoveTowards(objTransform.localPosition, Vector3.zero, 10f * Time.deltaTime);
            yield return null;
        }
        objTransform.localPosition = Vector3.zero;
    }

    public void Hightlight(bool hightlight)
    {
        outline.enabled = hightlight && SubGamePlayerControlManager.instance.CurrentGame == SubGame.RealWorld;
    }
}
