using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.ShaderGraph;
using UnityEngine;
using static ObservableExtensions;

public class SubGamePlayerControlManager : ScriptableSingleton<SubGamePlayerControlManager>
{
    #region CurentGame

    public event ObservableChangedCallback<SubGame> OnCurrentGameChanged;

    public SubGame CurrentGame
    {
        get => currentGame;
        set
        {
            if (inputs.ContainsKey(value))
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

    private readonly Dictionary<SubGame, CustomInputSystem> inputs = new();
    private readonly Dictionary<SubGame, SubGameScreen> screens = new();

    public (CustomInputSystem Input, SubGameScreen Screen) this[SubGame subGame]
        => (GetInput(subGame), GetScreen(subGame));

    public CustomInputSystem GetInput(SubGame subGame)
    {
        // If this is the first game loaded, make current
        if (inputs.Count <= 0)
            currentGame = subGame;

        // If input already exist, return it 
        if (inputs.ContainsKey(subGame))
            return inputs[subGame];

        // Else, create it
        var ret = inputs[subGame] = new CustomInputSystem();

        // Enabling/disabling acording to current game.
        if (subGame == currentGame)
            subGame.Enable();
        else
            subGame.Disable();

        return ret;
    }

    public SubGameScreen GetScreen(SubGame subGame)
        => screens.ContainsKey(subGame) ? screens[subGame] : null;
}
