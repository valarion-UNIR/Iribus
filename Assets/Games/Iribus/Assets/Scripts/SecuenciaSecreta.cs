using UnityEngine;
using System.Collections.Generic;

public class SecuenciaSecreta : MonoBehaviour
{
    [SerializeField] private List<int> expectedSequence = new List<int> { 1, 2, 2, 1, 3, 3, 4 };
    private List<int> currentSequence = new List<int>();

    [SerializeField] private Transform sitioExplosion;

    void Start()
    {
        currentSequence.Clear();
    }

    public void TocarCampana(int campana)
    {
        HandleFunctionCall(campana);
    }

    private void HandleFunctionCall(int functionId)
    {
        currentSequence.Add(functionId);

        for (int i = 0; i < currentSequence.Count; i++)
        {
            if (currentSequence[i] != expectedSequence[i])
            {
                Debug.Log("Requetemal");
                currentSequence.Clear();
                return;
            }
        }

        if (currentSequence.Count == expectedSequence.Count)
        {
            Debug.Log("Fenomenal");
            TriggerAction();
            currentSequence.Clear();
        }
    }

    private void TriggerAction()
    {
        GameManagerMetroidvania.Instance.GetParticleSpawner().SpawnByIndex(3, sitioExplosion.position, Quaternion.identity);
    }
}