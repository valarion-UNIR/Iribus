using Unity.Cinemachine;
using UnityEngine;

public class InteractableHablarMV : MonoBehaviour, IInteractMV
{
    public GameObject GameObject => gameObject;

    [SerializeField] DialogoConcreto dialogoNPC;
    [SerializeField] CinemachineCamera dialogoCamera;

    void Start()
    {
        Debug.Log("Start " + gameObject.name);
    }

    void Update()
    {
        
    }

    public void InteractMV()
    {
        GameManagerMetroidvania.Instance.StartDialogue(dialogoNPC, dialogoCamera);
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
