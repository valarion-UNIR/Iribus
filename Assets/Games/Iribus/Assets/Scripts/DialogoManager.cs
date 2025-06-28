using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class TypingFinishedEvent : UnityEvent<string>
{ }

public class DialogoManager : MonoBehaviour
{
    [SerializeField]
    private EscritorTexto textoDialogo;

    [SerializeField]
    private EscritorTexto textoPregunta;
    [SerializeField]
    private EscritorTexto textoRespuesta1;
    [SerializeField]
    private EscritorTexto textoRespuesta2;

    private TextMeshProUGUI textoRespuesta1Txt;
    private TextMeshProUGUI textoRespuesta2Txt;

    [SerializeField]
    private DialogoConcreto dialogosAEscribir;

    private InteractableHablarMV npc;

    private int index = 0;
    private int maxDialogues;

    private float delayEscribir = 0.05f;
    bool textoAutomatico;
    private float waitText = 1f;

    [SerializeField]
    private Image imagePortrait;

    private bool fasePreguntas = false;
    private bool faseAvanzada = false;

    private int indexRespuesta = 0;

    //[SerializeField]
    //private TypingFinishedEvent OnTypingFinished;

    private void Awake()
    {
        if(dialogosAEscribir != null) maxDialogues = dialogosAEscribir.dialogos.Length;

        textoRespuesta1Txt = textoRespuesta1.GetComponent<TextMeshProUGUI>();
        textoRespuesta2Txt = textoRespuesta2.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if(faseAvanzada)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                indexRespuesta = 1 - indexRespuesta;
            }

            if(indexRespuesta == 0)
            {
                textoRespuesta1Txt.color = Color.yellow;
                textoRespuesta2Txt.color = Color.white;
            }
            else
            {
                textoRespuesta1Txt.color = Color.white;
                textoRespuesta2Txt.color = Color.yellow;
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(faseAvanzada)
            {
                faseAvanzada = false;
                TerminarDialogo();
                return;
            }

            if (textoDialogo.GetEscribiendo())
            {
                textoDialogo.RevelarText();
            }
            else if(textoDialogo.textoCompletado)
            {
                textoDialogo.textoCompletado = false;
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
        string textoCompleto = GameManagerMetroidvania.Instance.GetDialogueManager().GetDialogue(dialogosAEscribir.dialogos[index].dialogoTexto);

        if(dialogosAEscribir.dialogos[index].spritePortrait == null)
        {
            imagePortrait.color = Color.clear;
        }
        else
        {
            imagePortrait.color = Color.white;
            imagePortrait.sprite = dialogosAEscribir.dialogos[index].spritePortrait;
        }

        textoDialogo.EscribirText(textoCompleto, 
            dialogosAEscribir.dialogos[index].dialogoVelocidad, 
            waitText,
            dialogosAEscribir.dialogos[index].autocompletable, 
            dialogosAEscribir.dialogos[index].skipeable);
    }

    public void EscribirPregunta()
    {
        string textoCompleto = GameManagerMetroidvania.Instance.GetDialogueManager().GetDialogue(dialogosAEscribir.pregunta.preguntaTexto);

        textoPregunta.EscribirText(textoCompleto, 
            dialogosAEscribir.pregunta.respuestaVelocidad, 
            waitText, 
            0.05f, 
            false);
    }

    public void EscribirRespuestas()
    {
        string textoCompleto = GameManagerMetroidvania.Instance.GetDialogueManager().GetDialogue(dialogosAEscribir.pregunta.respuestaTexto1);

        textoRespuesta1.EscribirText(textoCompleto, 
            dialogosAEscribir.pregunta.respuestaVelocidad, 
            waitText, 
            0f, 
            false);

        textoCompleto = GameManagerMetroidvania.Instance.GetDialogueManager().GetDialogue(dialogosAEscribir.pregunta.respuestaTexto2);

        textoRespuesta2.EscribirText(textoCompleto,
            dialogosAEscribir.pregunta.respuestaVelocidad,
            waitText,
            0f,
            false);
    }

    public void CompleteSentence()
    {

        if(fasePreguntas)
        {
            faseAvanzada = true;
            fasePreguntas = false;
            EscribirRespuestas();
            return;
        }

        index++;
        if(index >= maxDialogues)
        {
            if(!dialogosAEscribir.preguntaOcurre) 
            {
                Debug.Log("TERMINODRILOA");
                if (!dialogosAEscribir.loopeable)
                {
                    npc.SetIndexDialogos(dialogosAEscribir.nextIndex);
                }

                GameManagerMetroidvania.Instance.EndDialogue();
                return;
            }
            else
            {
                textoDialogo.OcultarTexto();
                imagePortrait.color = Color.clear;
                Debug.Log("PRENTUFIA");
                fasePreguntas = true;
                EscribirPregunta();
                return;
            }
        }
        EscribirDialogo();
    }

    public void SetDialogoEscribir(DialogoConcreto dialogo, InteractableHablarMV npc)
    {
        index = 0;
        dialogosAEscribir = dialogo;
        this.npc = npc;
        maxDialogues = dialogosAEscribir.dialogos.Length;
        Debug.Log(maxDialogues);
    }

    public void EmpezarDialogo()
    {
        if(dialogosAEscribir.dialogos.Length == 0)
        {
            CompleteSentence();
        }
        else
        {
            EscribirDialogo();
        }
    }

    private void TerminarDialogo()
    {
        faseAvanzada = false;
        if (indexRespuesta == 0)
        {
            npc.SetIndexDialogos(dialogosAEscribir.nextIndex);
        }
        else
        {
            npc.SetIndexDialogos(dialogosAEscribir.pregunta.nextIndex2);
        }
        ResetTextos();
        SetDialogoEscribir(npc.GetDialogosByIndex(npc.GetIndexDialogos()), npc);
        EmpezarDialogo();
    }
    
    private void ResetTextos()
    {
        textoDialogo.OcultarTexto();
        textoPregunta.OcultarTexto();
        textoRespuesta1.OcultarTexto();
        textoRespuesta2.OcultarTexto();
    }
}