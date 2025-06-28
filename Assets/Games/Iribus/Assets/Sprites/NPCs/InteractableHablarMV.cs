using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class InteractableHablarMV : MonoBehaviour, IInteractMV
{
    public GameObject GameObject => gameObject;

    [SerializeField] List<DialogoConcreto> dialogoNPC;
    [SerializeField] CinemachineCamera dialogoCamera;

    private int indexDialogos = 0;

    void Start()
    {
        Debug.Log("Start " + gameObject.name);
    }

    public void SetIndexDialogos(int indexDialogos)
    {
        this.indexDialogos = indexDialogos;
    }

    public int GetIndexDialogos()
    {
        return indexDialogos;
    }

    public DialogoConcreto GetDialogosByIndex(int index)
    {
        return dialogoNPC[index];
    }

    public void InteractMV()
    {
        Debug.Log(indexDialogos);
        GameManagerMetroidvania.Instance.StartDialogue(dialogoNPC[indexDialogos], dialogoCamera, this);
    }

    public void Highlight()
    {
        Debug.Log("Jilightiaron");
    }

    public void Unhighlight()
    {
        Debug.Log("NOOOO Unjiligtiaron");
    }
}
