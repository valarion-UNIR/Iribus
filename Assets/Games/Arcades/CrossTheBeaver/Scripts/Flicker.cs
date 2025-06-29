using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Flicker : MonoBehaviour
{
    private TMP_Text text;
    private Color color1 = new Color(255, 255, 255, 1);
    private Color color2 = new Color(255, 255, 255, 0);

    private void Start()
    {
        text = GetComponent<TMP_Text>();
        StartCoroutine(FlickerText());
    }
    private void OnEnable()
    {
        StartCoroutine(FlickerText());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
    private IEnumerator FlickerText()
    {
        while (true)
        {
            text.color = color1;
            yield return new WaitForSeconds(1f);
            text.color = color2;
            yield return new WaitForSeconds(1f);
        }
    }
}
