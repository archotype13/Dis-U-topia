public static class GeneralConstants
{
    public const float EXPLORED_DARKEN = 0.75f;
    public enum DrawingOrders
    {
        DOORS,
        CORPSES,
        NPCS,
        PLAYER,
    }

    public static Color GrayscaleColor(Color color)
    {
        byte value = (byte)(0.299f * color.R + 0.587f * color.G + 0.114f * color.B);
        return new(value, value, value, color.A);
    }
}