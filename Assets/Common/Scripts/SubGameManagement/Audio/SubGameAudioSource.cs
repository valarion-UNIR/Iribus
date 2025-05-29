 using System;
using UnityEngine;

public abstract class SubGameAudioSource : MonoBehaviour
{       
    public abstract SubGame SubGame { get; }

    public AudioSourcePool Source { get; private set; }

    /// <summary>
    /// On Awake, add this sub game to manager.
    /// </summary>
    protected virtual void Awake()
    {
        Source = SubGameAudioManager.instance[SubGame];
    }
}