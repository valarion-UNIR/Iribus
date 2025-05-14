using System.Collections;
using UnityEngine;

public class SlotPachinko : MonoBehaviour
{
    private GameObject sign;
    private GameObject trigger1;
    private GameObject trigger2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sign = transform.GetChild(0).gameObject;
        trigger1 = transform.GetChild(1).gameObject;
        trigger2 = transform.GetChild(2).gameObject;
        //StartCoroutine(StartMovingSlotSign(sign));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator StartMovingSlotSign(GameObject g)
    {
        Material m = g.GetComponent<MeshRenderer>().material;
        float speed = 0.5f;
        float cont = 0f;
        while (true)
        {
            cont += speed * Time.deltaTime;
            float offset = Mathf.Repeat(cont, 1f);

            m.mainTextureOffset = new Vector2(offset, 0f);

            yield return null;
        }
    }
}
