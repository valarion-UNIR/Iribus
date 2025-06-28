using UnityEngine;

[CreateAssetMenu(fileName = "DialogoConcreto", menuName = "ScriptableObjects/DialogoConcreto")]
public class DialogoConcreto : ScriptableObject
{
    public string subGame;
    public bool loopeable;
    public int nextIndex;
    public DialogoModo[] dialogos;

    public bool preguntaOcurre;
    public Pregunta pregunta;
}

[System.Serializable]
public class DialogoModo
{
    public string dialogoTexto;
    public float dialogoVelocidad = 0.05f;
    public Sprite spritePortrait;
    public bool skipeable;
    public float autocompletable = 0;
}

[System.Serializable]
public class Pregunta
{
    public string preguntaTexto;
    public float respuestaVelocidad = 0.05f;
    public string respuestaTexto1;
    public string respuestaTexto2;
    public int nextIndex2;
}