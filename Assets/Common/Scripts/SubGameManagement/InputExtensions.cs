public static class InputExtensions
{
    public static void Enable(this SubGame game, bool enable = true)
    {
        var input = SubGamePlayerControlManager.instance.GetInput(game);

        if (enable)
            input.Enable();
        else
            input.Disable();

        // Enable/disable subgame input change
        if (game == SubGame.RealWorld)
            input.Subgames.Enable();
        else
            input.Subgames.Disable();
    }

    public static void Disable(this SubGame game)
        => game.Enable(false);
}