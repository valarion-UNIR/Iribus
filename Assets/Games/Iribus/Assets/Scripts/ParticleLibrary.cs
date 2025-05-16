using UnityEngine;

[CreateAssetMenu(fileName = "ParticleLibrary", menuName = "ScriptableObjects/ParticleLibrary")]
public class ParticleLibrary : ScriptableObject
{
    public ParticleSystem[] effects;

    public ParticleSystem GetEffect(int index)
    {
        if (index < 0 || index >= effects.Length) return null;
        return effects[index];
    }
}