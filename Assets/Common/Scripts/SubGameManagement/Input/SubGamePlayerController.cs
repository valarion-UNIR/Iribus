 using System;
using UnityEngine;

public abstract class SubGamePlayerController : MonoBehaviour
{       
    public abstract SubGame SubGame { get; }

    public CustomInputSystem Input { get; private set; }

    /// <summary>
    /// On Awake, add this sub game to manager.
    /// </summary>
    protected virtual void Awake()
    {
        Input = SubGamePlayerControlManager.instance[SubGame];
    }

    private void Start()
    {
    }
}