using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class InteractableHablarMV : MonoBehaviour, IInteractMV
{
    public GameObject GameObject => gameObject;

    [SerializeField] List<DialogoConcreto> dialogoNPC;
    [SerializeField] CinemachineCamera dialogoCamera;
    [SerializeField] GameObject interactCosa;

    [SerializeField] List<DialogoEventos> dialogoEventos;

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
        interactCosa.SetActive(false);
        GameManagerMetroidvania.Instance.StartDialogue(dialogoNPC[indexDialogos], dialogoCamera, this);
    }

    public void Highlight()
    {
        Debug.Log("Jilightiaron");
        interactCosa.SetActive(true);
    }

    public void Unhighlight()
    {
        Debug.Log("NOOOO Unjiligtiaron");
        interactCosa.SetActive(false);
    }

    public float LlamarDialogoEvento(string eventoID)
    {
        foreach(DialogoEventos evento in dialogoEventos)
        {
            if (evento.eventoID.Equals(eventoID))
            {
                evento.evento.Invoke();
                return evento.duracionEvento;
            }
        }
        return 0f;
    }
}

[System.Serializable]
public class DialogoEventos
{
    public string eventoID;
    public float duracionEvento;
    public UnityEvent evento;
}
