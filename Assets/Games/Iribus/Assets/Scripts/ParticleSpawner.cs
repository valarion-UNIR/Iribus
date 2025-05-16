using UnityEngine;
using System.Collections;

public class ParticleSpawner : MonoBehaviour
{
    public ParticleLibrary library;

    public void SpawnByIndex(int index, Vector3 position, Quaternion rotation)
    {
        var prefab = library.GetEffect(index);
        if (prefab == null)
        {
            Debug.LogWarning($"No hay particulas {index}", this);
            return;
        }

        var instance = Instantiate(prefab, position, rotation);
        StartCoroutine(DestroyWhenComplete(instance));
    }

    private IEnumerator DestroyWhenComplete(ParticleSystem ps)
    {
        while (ps != null && ps.IsAlive(true))
            yield return null;

        if (ps != null)
            Destroy(ps.gameObject);
    }
}