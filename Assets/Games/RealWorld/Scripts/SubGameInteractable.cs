using Eflatun.SceneReference;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Outline))]
public class SubGameInteractable : MonoBehaviour, IInteract
{
    [SerializeField] private SubGame subGame;
    
    private Outline outline;
    private CinemachineCamera gameCamera;

    public SubGame SubGame => subGame;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize input for this subGame, just in case
        _ = SubGamePlayerControlManager.instance[subGame];

        outline = GetComponent<Outline>();
        outline.enabled = false;

        gameCamera = GetComponentInChildren<CinemachineCamera>();
        if(gameCamera != null)
            SubGamePlayerControlManager.instance.OnCurrentGameChanged += Instance_OnCurrentGameChanged; ;
    }

    private void Instance_OnCurrentGameChanged(SubGame oldValue, SubGame newValue)
    {
        gameCamera.enabled = newValue == subGame;
    }

    public void Interact()
    {
        SubGamePlayerControlManager.instance.CurrentGame = subGame;
    }

    public void Hightlight(bool hightlight)
    {
        outline.enabled = hightlight && SubGamePlayerControlManager.instance.CurrentGame == SubGame.RealWorld;
    }
}
