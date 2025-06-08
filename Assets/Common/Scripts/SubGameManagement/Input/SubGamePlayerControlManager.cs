using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static ObservableExtensions;

public class SubGamePlayerControlManager : AbstractSubGameObjectCreationManager<CustomInputSystem, SubGamePlayerControlManager>
{
    #region CurentGame

    public event ObservableChangedCallback<SubGame> OnCurrentGameChanged;

    public SubGame CurrentGame
    {
        get => currentGame;
        set
        {
            if (objects.ContainsKey(value))
                SetValue(ref currentGame, value, OnInnerCurrentGameChanged, OnCurrentGameChanged);
        }
    }

    private void OnInnerCurrentGameChanged(SubGame oldGame, SubGame newGame)
    {
        oldGame.Disable();
        newGame.Enable();
    }

    [DoNotSerialize] private SubGame currentGame = SubGame.RealWorld;

    #endregion

    protected override CustomInputSystem CreateObject(SubGame subGame)
    {
        // If this is the first game loaded, make current
        if (objects.Count <= 0)
            currentGame = subGame;

        // If input already exist, return it 
        if (objects.ContainsKey(subGame))
            return objects[subGame];

        // Else, create it
        var ret = objects[subGame] = new CustomInputSystem();

        // Enabling/disabling acording to current game.
        if (subGame == currentGame)
            subGame.Enable();
        else
            subGame.Disable();

        if (subGame == SubGame.RealWorld)
            CurrentGame = subGame;

        return ret;
    }
}
