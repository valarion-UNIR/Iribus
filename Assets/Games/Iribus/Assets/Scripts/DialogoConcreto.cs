using UnityEngine;

[CreateAssetMenu(fileName = "DialogoConcreto", menuName = "ScriptableObjects/DialogoConcreto")]
public class DialogoConcreto : ScriptableObject
{
    public string subGame;

    public DialogoModo[] dialogos;
}

[System.Serializable]
public class DialogoModo
{
    public int dialogoIndex;
    public float dialogoVelocidad;
}