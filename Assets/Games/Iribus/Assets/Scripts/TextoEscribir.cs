using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TypingFinishedEvent : UnityEvent<string>
{ }

public class TextoEscribir : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;
    private string fullText;

    [SerializeField]
    private DialogoConcreto dialogosAEscribir;

    private int index = 0;
    private int maxDialogues;

    [SerializeField]
    private float delayEscribir = 0.05f;
    [SerializeField]
    private float waitText = 1f;
    [SerializeField]
    private float waitAutocompletable = 1f;

    [SerializeField]
    private TypingFinishedEvent OnTypingFinished;

    private void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        maxDialogues = dialogosAEscribir.dialogos.Length;
    }

    public void EscribirDialogo()
    {
        textMeshPro.text = FindAnyObjectByType<DialogueManager>().GetDialogue(dialogosAEscribir.dialogos[index].dialogoTexto);
        delayEscribir = dialogosAEscribir.dialogos[index].dialogoVelocidad;
        fullText = textMeshPro.text;
        textMeshPro.maxVisibleCharacters = 0;
        StartCoroutine(RevealText());
    }

    private IEnumerator RevealText()
    {

        yield return new WaitForSeconds(waitText);

        for (int i = 0; i <= fullText.Length; i++)
        {
            textMeshPro.maxVisibleCharacters = i;

            yield return new WaitForSeconds(delayEscribir);
        }

        yield return new WaitForSeconds(waitAutocompletable);
        CompleteSentence();
    }

    public void CompleteSentence()
    {
        //textoAEscribir.text = texto;
        //Debug.Log("Terminada");   
        OnTypingFinished?.Invoke(fullText);
        index++;
        if(index == maxDialogues)
        {
            return;
        }
        EscribirDialogo();
    }
}