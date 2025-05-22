using UnityEngine;

[CreateAssetMenu(fileName = "DialogoConcreto", menuName = "ScriptableObjects/DialogoConcreto")]
public class DialogoConcreto : ScriptableObject
{
    public string subGame;
    public bool skipeable;
    public DialogoModo[] dialogos;
}

[System.Serializable]
public class DialogoModo
{
    public string dialogoTexto;
    public float dialogoVelocidad;
}