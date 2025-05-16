using System.Collections;
using TMPro;
using UnityEngine;

public class TextoStart : MonoBehaviour
{
    private TextMeshProUGUI texto;
    private Coroutine parpadeo;

    void Start()
    {
        texto = GetComponent<TextMeshProUGUI>();
        parpadeo = StartCoroutine(Parpadeo());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StopCoroutine(parpadeo);
            texto.alpha = 1f;
            GameManagerMetroidvania.Instance.EmpezarPartida();
        }
    }

    private IEnumerator Parpadeo()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.5f);
            texto.alpha = 0f;
            yield return new WaitForSeconds(0.5f);
            texto.alpha = 1f;
        }
    }
}
