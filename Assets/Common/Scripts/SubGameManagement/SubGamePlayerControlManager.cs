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
        set => SetValue(ref currentGame, value, OnInnerCurrentGameChanged, OnCurrentGameChanged);
    }

    private void OnInnerCurrentGameChanged(SubGame oldGame, SubGame newGame)
    {
        GetInput(oldGame).Disable();
        GetInput(newGame).Enable();
    }

    private SubGame currentGame = SubGame.RealWorld;

    #endregion

    private readonly Dictionary<SubGame, CustomInputSystem> inputs = new();
    private readonly Dictionary<SubGame, SceneScreen> screens = new();

    public (CustomInputSystem Input, SceneScreen Screen) this[SubGame subGame]
        => (GetInput(subGame), GetScreen(subGame));

    public CustomInputSystem GetInput(SubGame subGame)
    {
        if (inputs.ContainsKey(subGame))
            return inputs[subGame];
        var ret = inputs[subGame] = new CustomInputSystem();
        if (subGame == currentGame)
            ret.Enable();
        else
            ret.Disable();
        return ret;
    }

    public SceneScreen GetScreen(SubGame subGame)
        => screens.ContainsKey(subGame) ? screens[subGame] : null;
}
