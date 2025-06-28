using System.Collections;
using TMPro;
using UnityEngine;

public class EscritorTexto : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private string texto;

    [SerializeField]
    private float delayEscribir = 0.05f;
    [SerializeField]
    private float waitText = 1f;
    [SerializeField]
    private float waitAutocompletable = 1f;
    [SerializeField]
    bool textoSkippeable;

    [SerializeField]
    DialogoManager dialogoManager;

    private Coroutine escritorCoroutine;
    public bool textoCompletado = false;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        escritorCoroutine = null;
    }

    public void EscribirText(string texto, float delayEscribir, float waitText, float waitAutocompletable, bool textoSkippeable)
    {
        textMesh.maxVisibleCharacters = 0;

        this.texto = texto;
        this.delayEscribir = delayEscribir;
        this.waitText = waitText;
        this.waitAutocompletable = waitAutocompletable;
        this.textoSkippeable = textoSkippeable;

        textMesh.text = texto;
        escritorCoroutine = StartCoroutine(EscribirText());
    }

    private IEnumerator EscribirText()
    {
        textoCompletado = false;

        yield return new WaitForSeconds(waitText);

        for (int i = 0; i <= texto.Length; i++)
        {
            textMesh.maxVisibleCharacters = i;

            yield return new WaitForSeconds(delayEscribir);
        }

        if (waitAutocompletable != 0 && dialogoManager != null)
        {
            yield return new WaitForSeconds(waitAutocompletable);
            dialogoManager.CompleteSentence();
        }

        textoCompletado = true;
        escritorCoroutine = null;
    }

    public void RevelarText()
    {
        if (!textoSkippeable) return;
        StopCoroutine(escritorCoroutine);
        escritorCoroutine = null;

        textMesh.maxVisibleCharacters = texto.Length;
        textoCompletado = true;
    }

    public void OcultarTexto()
    {
        textoCompletado = false;
        textMesh.maxVisibleCharacters = 0;
    }

    public bool GetEscribiendo()
    {
        if (escritorCoroutine != null) return true;
        return false;
    }
}
