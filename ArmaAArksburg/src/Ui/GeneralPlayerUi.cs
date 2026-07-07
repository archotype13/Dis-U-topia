public sealed class PlayerStatusBar : ScreenSurface
{
    
    public const int WINDOW_WIDTH = 120; // const for just determining internal spacing
    public const int NAME_WIDTH = 20;
    public const int SPEED_WIDTH = 20;
    public const int QUICKNESS_WIDTH = 20;
    public static ColoredGlyph Divider = new(Color.White, Color.DarkTurquoise, '|');

    private UiLabel PlayerName;
    private UiLabel SpeedLabel;
    private UiLabel QuicknessLabel;
    public override void Render(TimeSpan delta)
    {
        if (Engine.Instance!.GameManager.Player == null)
            return;
        
        Entity player = Engine.Instance!.GameManager.Player;
        PlayerName.UpdateText(player.Name);
        SpeedLabel.UpdateText($"Speed: {player.Ai!.Speed}");
        QuicknessLabel.UpdateText($"Quickness: {player.Ai!.Quickness}");

        base.Render(delta);
    }

    public PlayerStatusBar(int x, int y, int width, int height) : base(width, height)
    {
        Position = (x, y);
        Surface.DefaultBackground = Color.DarkTurquoise;
        Surface.DefaultForeground = Color.White;

        // name
        PlayerName = new(0, 0, NAME_WIDTH, "PLAYER NAME");
        Children.Add(PlayerName);
        Surface[NAME_WIDTH + 1].CopyAppearanceFrom(Divider);

        // quickness
        QuicknessLabel = new(WINDOW_WIDTH - QUICKNESS_WIDTH, 0, QUICKNESS_WIDTH, "Quickness:");
        Children.Add(QuicknessLabel);
        Surface[WINDOW_WIDTH - QUICKNESS_WIDTH - 2].CopyAppearanceFrom(Divider);

        // speed
        SpeedLabel = new(WINDOW_WIDTH - SPEED_WIDTH - QUICKNESS_WIDTH - 2, 0, SPEED_WIDTH, "Speed:");
        Children.Add(SpeedLabel);
        Surface[WINDOW_WIDTH - SPEED_WIDTH - QUICKNESS_WIDTH - 4].CopyAppearanceFrom(Divider);
    }
}

public sealed class UiLabel : ScreenSurface // used for one line stuff
{
    public void UpdateText(string text)
    {
        Surface.Clear();
        Surface.Print(0, 0, text);
    }

    public UiLabel(int x, int y, int width, string text) : base(width, 1) 
    {
        Position = (x, y);
        UpdateText(text);
    }
}