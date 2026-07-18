using SadConsole.UI;

public static class GeneralConstants
{
    public const float EXPLORED_DARKEN = 0.75f;
    public enum DrawingOrders
    {
        DOORS,
        ITEMS,
        CORPSES,
        NPCS,
        PLAYER,
    }

    public const int PICKUP_DROP_WINDOW_WIDTH = 40;
    public const int MAX_ENTITY_ON_TILE_MESSAGE_LENGTH = 5; // maximum amount of entities until the log message for walking on a tile with just say "and x other things"

    public static Color GrayscaleColor(Color color)
    {
        byte value = (byte)(0.299f * color.R + 0.587f * color.G + 0.114f * color.B);
        return new(value, value, value, color.A);
    }

    public static void Init()
    {
        // set up theme
        Colors.Default.ControlHostForeground.SetColor(Color.White);
        Colors.Default.ControlHostBackground.SetColor(Color.DarkGreen);
        Colors.Default.ControlBackgroundNormal.SetColor(Color.DarkGreen);
        Colors.Default.ControlBackgroundMouseOver.SetColor(Color.DarkGreen);
        Colors.Default.RebuildAppearances();
    }
}