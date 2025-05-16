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
    }

    public void EscribirDialogo(string dialogo)
    {
        textMeshPro.text = dialogo;
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
    }
}