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
    bool textoAutomatico;
    [SerializeField]
    private float waitText = 1f;
    [SerializeField]
    private float waitAutocompletable = 1f;

    [SerializeField]
    private TypingFinishedEvent OnTypingFinished;

    private bool textoCompletado = false;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        if(dialogosAEscribir != null) maxDialogues = dialogosAEscribir.dialogos.Length;
    }

    private void Update()
    {
        if(textoCompletado)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                CompleteSentence();
            }
        }
    }

    public void EscribirDialogo()
    {
        if (dialogosAEscribir == null)
        {
            Debug.Log("No dialogues que escribir");
            return;
        }
        textMeshPro.text = GameManagerMetroidvania.Instance.GetDialogueManager().GetDialogue(dialogosAEscribir.dialogos[index].dialogoTexto);
        delayEscribir = dialogosAEscribir.dialogos[index].dialogoVelocidad;
        fullText = textMeshPro.text;
        textMeshPro.maxVisibleCharacters = 0;
        StartCoroutine(RevealText());
    }

    private IEnumerator RevealText()
    {
        textoCompletado = false;

        yield return new WaitForSeconds(waitText);

        for (int i = 0; i <= fullText.Length; i++)
        {
            textMeshPro.maxVisibleCharacters = i;

            yield return new WaitForSeconds(delayEscribir);
        }
        if (textoAutomatico)
        {
            yield return new WaitForSeconds(waitAutocompletable);
            CompleteSentence();
        }

        textoCompletado = true;
    }

    public void CompleteSentence()
    {
        //textoAEscribir.text = texto;
        //Debug.Log("Terminada");   
        //OnTypingFinished?.Invoke(fullText);
        index++;
        if(index == maxDialogues)
        {
            ResetTextoEscribir();
            GameManagerMetroidvania.Instance.EndDialogue();
            return;
        }
        EscribirDialogo();
    }

    public void ResetTextoEscribir()
    {
        textMeshPro.text = "";
        index = 0;
    }


    public void SetDialogoEscribir(DialogoConcreto dialogo)
    {
        dialogosAEscribir = dialogo;
        maxDialogues = dialogosAEscribir.dialogos.Length;
        Debug.Log(maxDialogues);
    }
}