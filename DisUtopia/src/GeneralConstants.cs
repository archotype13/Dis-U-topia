public static class GeneralConstants
{
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