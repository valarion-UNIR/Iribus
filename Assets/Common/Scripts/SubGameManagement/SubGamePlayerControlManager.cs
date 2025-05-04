using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.ShaderGraph;
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
        GetInput(oldGame).Disable();
        GetInput(newGame).Enable();
    }

    [DoNotSerialize] private SubGame currentGame = SubGame.RealWorld;

    #endregion

    private readonly Dictionary<SubGame, CustomInputSystem> inputs = new();
    private readonly Dictionary<SubGame, SubGameScreen> screens = new();

    public (CustomInputSystem Input, SubGameScreen Screen) this[SubGame subGame]
        => (GetInput(subGame), GetScreen(subGame));

    public CustomInputSystem GetInput(SubGame subGame)
    {
        if (inputs.Count <= 0)
            currentGame = SubGame.RealWorld;

        if (inputs.ContainsKey(subGame))
            return inputs[subGame];
        var ret = inputs[subGame] = new CustomInputSystem();
        if (subGame == currentGame)
            ret.Enable();
        else
            ret.Disable();
        return ret;
    }

    public SubGameScreen GetScreen(SubGame subGame)
        => screens.ContainsKey(subGame) ? screens[subGame] : null;
}
